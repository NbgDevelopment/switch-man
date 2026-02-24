using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NbgDev.SwitchMan.Switches.Contract;
using NbgDev.SwitchMan.Switches.Contract.Models;
using NbgDev.SwitchMan.Switches.OmadaController.OmadaApi;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace NbgDev.SwitchMan.Switches.OmadaController;

/// <summary>
/// Implementation of ISwitchAccessService for TP-Link Omada Controller OpenAPI
/// </summary>
public class OmadaControllerSwitchAccessService : ISwitchAccessService
{
    // Omada API documentation: https://omada-northbound-docs.tplinkcloud.com/#/versions
    // Currently used version: 5.15.24

    private readonly ILogger<OmadaControllerSwitchAccessService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _controllerUrl;
    private readonly string _omadaId;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private string? _accessToken;
    private DateTime _tokenExpiration = DateTime.MinValue;

    public OmadaControllerSwitchAccessService(
        ILogger<OmadaControllerSwitchAccessService> logger,
        HttpClient httpClient,
        IOptions<OmadaControllerOptions> options)
    {
        _logger = logger;
        _httpClient = httpClient;

        var controllerOptions = options.Value;
        _controllerUrl = controllerOptions.ControllerUrl;
        _omadaId = controllerOptions.OmadaId;
        _clientId = controllerOptions.ClientId;
        _clientSecret = controllerOptions.ClientSecret;
    }

    public async Task<int> GetPortCountAsync(string ipAddress)
    {
        try
        {
            _logger.LogInformation("Getting port count for switch at {IpAddress} via Omada Controller", ipAddress);

            // Ensure we're authenticated
            await EnsureAuthenticatedAsync();

            // Get switch information from Omada Controller
            var switchInfo = await GetSwitchInfoAsync(IPAddress.Parse(ipAddress));

            if (switchInfo is not null)
            {
                var portCount = switchInfo.PortList.Length;
                _logger.LogInformation("Switch at {IpAddress} has {PortCount} ports", ipAddress, portCount);
                return portCount;
            }

            _logger.LogWarning("Could not determine port count for switch at {IpAddress}", ipAddress);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting port count for switch at {IpAddress}", ipAddress);
            throw;
        }
    }

    public async Task<IEnumerable<PortInfo>> GetPortVlansAsync(string ipAddress)
    {
        try
        {
            _logger.LogInformation("Getting port VLAN information for switch at {IpAddress} via Omada Controller", ipAddress);

            // Ensure we're authenticated
            await EnsureAuthenticatedAsync();

            var siteId = await GetSiteId();

            // Get switch port configuration from Omada Controller
            var switchInfo = await GetSwitchInfoAsync(IPAddress.Parse(ipAddress));

            var portInfoList = new List<PortInfo>();

            if (switchInfo is not null)
            {
                var networkProfiles = await GetNetworkProfiles(siteId);
                var networks = await GetNetworks(siteId);

                foreach (var port in switchInfo.PortList)
                {
                    int portNumber = port.Port;
                    try
                    {
                        var profile = networkProfiles.SingleOrDefault(p => p.Id == port.ProfileId);
                        var network = networks.SingleOrDefault(n => n.Id == profile?.NativeNetworkId);

                        var vlanId = network?.Vlan ?? 1; // Default to VLAN 1 if not found
                        var vlanName = network?.Name ?? string.Empty;

                        portInfoList.Add(new PortInfo(portNumber, port.Name, vlanId, vlanName));
                        _logger.LogDebug("Port {Port} on switch {IpAddress} is assigned to VLAN {VlanId}",
                            portNumber, ipAddress, vlanId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not get VLAN ID for port {Port} on switch {IpAddress}",
                            portNumber, ipAddress);
                        // Add default VLAN 1 if query fails
                        portInfoList.Add(new PortInfo(portNumber, port.Name, 1, string.Empty));
                    }
                }
            }

            _logger.LogInformation("Retrieved VLAN information for {Count} ports on switch {IpAddress}",
                portInfoList.Count, ipAddress);
            return portInfoList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting port VLAN information for switch at {IpAddress}", ipAddress);
            throw;
        }
    }

    public async Task SetPortVlanAsync(string ipAddress, PortInfo port, string vlanId)
    {
        try
        {
            _logger.LogInformation("Setting VLAN {VlanId} for port {Port} on switch at {IpAddress} via Omada Controller",
                vlanId, port.PortNumber, ipAddress);

            // Ensure we're authenticated
            await EnsureAuthenticatedAsync();

            var siteId = await GetSiteId();

            // Get switch port configuration from Omada Controller
            var switchInfo = await GetSwitchInfoAsync(IPAddress.Parse(ipAddress));

            if (switchInfo is null)
            {
                throw new Exception($"Switch with IP {ipAddress} not found in Omada Controller");
            }

            var request = new HttpRequestMessage(HttpMethod.Put,
                $"{_controllerUrl}/openapi/v1/{_omadaId}/sites/{siteId}/switches/{switchInfo.Mac}/ports/{port.PortNumber}/profile");

            request.Content = JsonContent.Create(new SwitchProfileId(vlanId));
            await AuthorizeRequest(request);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OperationResponseWithoutResult>();

            if (result is null || result.ErrorCode != 0)
            {
                throw new Exception($"Failed to set port VLAN on Omada Controller: {result?.Msg ?? "No result or message"}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting port VLAN for switch at {IpAddress} and port {Port}", ipAddress, port.PortNumber);
            throw;
        }
    }

    public async Task<IEnumerable<VlanInfo>> GetVlansAsync(string ipAddress)
    {
        try
        {
            _logger.LogInformation("Getting VLANs for switch at {IpAddress} via Omada Controller", ipAddress);

            await EnsureAuthenticatedAsync();

            // VLANs are site-wide in Omada; ipAddress is used for logging and interface consistency
            var siteId = await GetSiteId();
            var profiles = await GetNetworkProfiles(siteId);

            var result = profiles.Select(p => new VlanInfo(p.Id, p.Name)).ToList();

            _logger.LogInformation("Retrieved {Count} VLANs for switch at {IpAddress}", result.Count, ipAddress);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting VLANs for switch at {IpAddress}", ipAddress);
            throw;
        }
    }

    private async Task EnsureAuthenticatedAsync()
    {
        // Check if we have a valid token
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiration)
        {
            return; // Token is still valid
        }

        try
        {
            _logger.LogInformation("Authenticating with Omada Controller at {Url} using OAuth 2.0", _controllerUrl);

            // OAuth 2.0 client credentials grant
            var tokenRequest = new
            {
                omadacId = _omadaId,
                client_id = _clientId,
                client_secret = _clientSecret
            };

            var response = await _httpClient.PostAsJsonAsync($"{_controllerUrl}/openapi/authorize/token?grant_type=client_credentials", tokenRequest);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OmadaAuthResponse>(JsonSerializerOptions.Web);

            // Parse OAuth 2.0 token response
            if (result?.Result is not null)
            {
                _accessToken = result.Result.AccessToken;

                // Get token expiration (default to 2 hours if not provided)
                var expiresIn = 7200; // Default: 2 hours
                if (result.Result.ExpiresIn > 0)
                {
                    expiresIn = result.Result.ExpiresIn;
                    _logger.LogInformation("Access token expires in {ExpiresInSeconds} seconds", expiresIn);
                }
                else
                {
                    _logger.LogWarning("Token response did not contain 'expires_in' field. Defaulting to 2 hours expiration.");
                }

                // Set expiration with a 60-second buffer to refresh before actual expiration
                _tokenExpiration = DateTime.UtcNow.AddSeconds(expiresIn - 60);

                _logger.LogInformation("Successfully authenticated with Omada Controller. Token expires in {ExpiresIn} seconds", expiresIn);
            }
            else
            {
                _logger.LogError("Token response: {Response}", result?.ToString() ?? "null");
                throw new InvalidOperationException("Failed to obtain access token from Omada Controller. Response did not contain 'access_token' field.");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error during authentication with Omada Controller. Check ClientId and ClientSecret.");
            throw new InvalidOperationException("Failed to authenticate with Omada Controller. Verify ClientId and ClientSecret are correct.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to authenticate with Omada Controller");
            throw;
        }
    }

    private async Task AuthorizeRequest(HttpRequestMessage request)
    {
        await EnsureAuthenticatedAsync();
        if (!string.IsNullOrEmpty(_accessToken))
        {
            request.Headers.Add("Authorization", $"Bearer AccessToken={_accessToken}");
        }
    }

    private async Task<string> GetSiteId()
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{_controllerUrl}/openapi/v1/{_omadaId}/sites?page=1&pageSize=10");
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

        await AuthorizeRequest(request);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        //var result = await response.Content.ReadFromJsonAsync<OperationResponseGridVoSiteSummaryInfo>();
        var json = await response.Content.ReadAsStringAsync();
        var result =
            JsonSerializer.Deserialize<OperationResponseGridVoSiteSummaryInfo>(json, JsonSerializerOptions.Web);
        // -44112
        if (result is null || result.ErrorCode != 0)
        {
            throw new Exception($"Failed to retrieve site information from Omada Controller: {result?.Msg ?? "No result or message"}");
        }

        var siteInfo = result.Result.Data.FirstOrDefault();

        if (siteInfo is null)
        {
            throw new Exception("No site found.");
        }

        return siteInfo.SiteId;
    }

    private async Task<IReadOnlyList<DeviceInfo>> GetDevicesInSite(string siteId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{_controllerUrl}/openapi/v1/{_omadaId}/sites/{siteId}/devices?page=1&pageSize=100");

        await AuthorizeRequest(request);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OperationResponseGridVoDeviceInfo>();

        if (result is null || result.ErrorCode != 0)
        {
            throw new Exception($"Failed to retrieve site device list from Omada Controller: {result?.Msg ?? "No result or message"}");
        }

        var devices = result.Result.Data;

        return devices;
    }

    private async Task<SwitchOverviewInfo?> GetSwitchInfoAsync(IPAddress ipAddress)
    {
        var siteId = await GetSiteId();

        var devices = await GetDevicesInSite(siteId);

        var requestedSwitch = devices.SingleOrDefault(d => d.IpAddress.Equals(ipAddress));

        if (requestedSwitch is null)
        {
            return null;
        }

        if (requestedSwitch.Type != "switch")
        {
            throw new ArgumentException($"The device with the IP {ipAddress} is of type {requestedSwitch.Type}");
        }

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{_controllerUrl}/openapi/v1/{_omadaId}/sites/{siteId}/switches/{requestedSwitch.Mac}?page=1&pageSize=100");

        await AuthorizeRequest(request);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OperationResponseSwitchOverviewInfo>();

        if (result is null || result.ErrorCode != 0)
        {
            throw new Exception($"Failed to retrieve switch information from Omada Controller: {result?.Msg ?? "No result or message"}");
        }

        return result.Result;
    }

    private async Task<IReadOnlyList<LanNetworkQueryOpenApiVo>> GetNetworks(string siteId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{_controllerUrl}/openapi/v1/{_omadaId}/sites/{siteId}/lan-networks?page=1&pageSize=100");

        await AuthorizeRequest(request);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OperationResponseLanNetworkOpenApiGridVoLanNetworkQueryOpenApiVo>();

        if (result is null || result.ErrorCode != 0)
        {
            throw new Exception($"Failed to retrieve site network list from Omada Controller: {result?.Msg ?? "No result or message"}");
        }

        return result.Result.Data;
    }

    private async Task<IReadOnlyList<LanProfileOpenApiVo>> GetNetworkProfiles(string siteId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{_controllerUrl}/openapi/v1/{_omadaId}/sites/{siteId}/lan-profiles?page=1&pageSize=100");

        await AuthorizeRequest(request);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OperationResponseGridVoLanProfileOpenApiVo>();

        if (result is null || result.ErrorCode != 0)
        {
            throw new Exception($"Failed to retrieve site network profile list from Omada Controller: {result?.Msg ?? "No result or message"}");
        }

        return result.Result.Data;
    }
}
