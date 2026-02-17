using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NbgDev.SwitchMan.Switches.Contract;
using NbgDev.SwitchMan.Switches.Contract.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace NbgDev.SwitchMan.Switches.OmadaController;

/// <summary>
/// Implementation of ISwitchAccessService for TP-Link Omada Controller OpenAPI
/// </summary>
public class OmadaControllerSwitchAccessService : ISwitchAccessService
{
    private readonly ILogger<OmadaControllerSwitchAccessService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _controllerUrl;
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
            var switchInfo = await GetSwitchInfoAsync(ipAddress);
            
            if (switchInfo.HasValue && switchInfo.Value.TryGetProperty("ports", out var portsElement))
            {
                var portCount = portsElement.GetArrayLength();
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
            
            // Get switch port configuration from Omada Controller
            var switchInfo = await GetSwitchInfoAsync(ipAddress);
            var portInfoList = new List<PortInfo>();
            
            if (switchInfo.HasValue && switchInfo.Value.TryGetProperty("ports", out var portsElement))
            {
                int portNumber = 1;
                foreach (var portElement in portsElement.EnumerateArray())
                {
                    try
                    {
                        // Extract PVID (Port VLAN ID) from port configuration
                        var vlanId = 1; // Default VLAN
                        if (portElement.TryGetProperty("pvid", out var pvidElement))
                        {
                            vlanId = pvidElement.GetInt32();
                        }
                        else if (portElement.TryGetProperty("vlan", out var vlanElement))
                        {
                            vlanId = vlanElement.GetInt32();
                        }
                        
                        portInfoList.Add(new PortInfo(portNumber, vlanId));
                        _logger.LogDebug("Port {Port} on switch {IpAddress} is assigned to VLAN {VlanId}", 
                            portNumber, ipAddress, vlanId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not get VLAN ID for port {Port} on switch {IpAddress}", 
                            portNumber, ipAddress);
                        // Add default VLAN 1 if query fails
                        portInfoList.Add(new PortInfo(portNumber, 1));
                    }
                    
                    portNumber++;
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
                grant_type = "client_credentials",
                client_id = _clientId,
                client_secret = _clientSecret
            };
            
            var response = await _httpClient.PostAsJsonAsync($"{_controllerUrl}/openapi/authorize/token", tokenRequest);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            
            // Parse OAuth 2.0 token response
            if (result.TryGetProperty("access_token", out var accessTokenElement))
            {
                _accessToken = accessTokenElement.GetString();
                
                // Get token expiration (default to 2 hours if not provided)
                var expiresIn = 7200; // Default: 2 hours
                if (result.TryGetProperty("expires_in", out var expiresInElement))
                {
                    expiresIn = expiresInElement.GetInt32();
                }
                
                // Set expiration with a 60-second buffer to refresh before actual expiration
                _tokenExpiration = DateTime.UtcNow.AddSeconds(expiresIn - 60);
                
                _logger.LogInformation("Successfully authenticated with Omada Controller. Token expires in {ExpiresIn} seconds", expiresIn);
            }
            else
            {
                _logger.LogError("Token response: {Response}", result.ToString());
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

    private async Task<JsonElement?> GetSwitchInfoAsync(string ipAddress)
    {
        try
        {
            // Query Omada Controller OpenAPI for switch information
            // Using OpenAPI v1 endpoint
            
            var request = new HttpRequestMessage(HttpMethod.Get, 
                $"{_controllerUrl}/openapi/v1/sites/default/switches?ip={ipAddress}");
            
            if (!string.IsNullOrEmpty(_accessToken))
            {
                request.Headers.Add("Authorization", $"Bearer {_accessToken}");
            }
            
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            
            // OpenAPI response structure
            if (result.TryGetProperty("result", out var resultElement) &&
                resultElement.TryGetProperty("data", out var dataElement) &&
                dataElement.GetArrayLength() > 0)
            {
                return dataElement[0];
            }
            
            _logger.LogWarning("No switch found at {IpAddress} in Omada Controller", ipAddress);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get switch information for {IpAddress} from Omada Controller", ipAddress);
            throw;
        }
    }
}
