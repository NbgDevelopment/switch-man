using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NbgDev.SwitchMan.Switches.Contract;
using NbgDev.SwitchMan.Switches.Contract.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace NbgDev.SwitchMan.Switches.OmadaController;

/// <summary>
/// Implementation of ISwitchAccessService for TP-Link Omada Controller
/// </summary>
public class OmadaControllerSwitchAccessService : ISwitchAccessService
{
    private readonly ILogger<OmadaControllerSwitchAccessService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _controllerUrl;
    private readonly string _username;
    private readonly string _password;
    private string? _token;

    public OmadaControllerSwitchAccessService(
        ILogger<OmadaControllerSwitchAccessService> logger,
        HttpClient httpClient,
        IOptions<OmadaControllerOptions> options)
    {
        _logger = logger;
        _httpClient = httpClient;
        
        var controllerOptions = options.Value;
        _controllerUrl = controllerOptions.ControllerUrl;
        _username = controllerOptions.Username;
        _password = controllerOptions.Password;
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
        if (!string.IsNullOrEmpty(_token))
        {
            return; // Already authenticated
        }

        try
        {
            _logger.LogInformation("Authenticating with Omada Controller at {Url}", _controllerUrl);
            
            var loginRequest = new
            {
                username = _username,
                password = _password
            };
            
            var response = await _httpClient.PostAsJsonAsync($"{_controllerUrl}/api/v2/login", loginRequest);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (result.TryGetProperty("result", out var resultElement) &&
                resultElement.TryGetProperty("token", out var tokenElement))
            {
                _token = tokenElement.GetString();
                _logger.LogInformation("Successfully authenticated with Omada Controller");
            }
            else
            {
                throw new InvalidOperationException("Failed to obtain authentication token from Omada Controller");
            }
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
            // Query Omada Controller API for switch information
            // Note: The actual API endpoint and structure depends on the Omada Controller version
            // This is a simplified implementation
            
            var request = new HttpRequestMessage(HttpMethod.Get, 
                $"{_controllerUrl}/api/v2/sites/default/switches?ip={ipAddress}");
            
            if (!string.IsNullOrEmpty(_token))
            {
                request.Headers.Add("Authorization", $"Bearer {_token}");
            }
            
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
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
