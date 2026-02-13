using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Microsoft.Extensions.Logging;
using NbgDev.SwitchMan.Switches.Contract;
using NbgDev.SwitchMan.Switches.Contract.Models;
using System.Net;

namespace NbgDev.SwitchMan.Switches.TLSG108PE;

/// <summary>
/// Implementation of ISwitchAccessService for TP-Link TL-SG108PE switches
/// </summary>
public class TlSg108PeSwitchAccessService : ISwitchAccessService
{
    private readonly ILogger<TlSg108PeSwitchAccessService> _logger;
    
    // SNMP OIDs for TL-SG108PE
    // Standard IF-MIB OID for number of interfaces
    private const string IfNumberOid = "1.3.6.1.2.1.2.1.0";
    
    // Standard dot1qVlanStaticRowStatus OID for VLAN configuration
    // Port VLAN membership: 1.3.6.1.2.1.17.7.1.4.3.1.2
    private const string Dot1qPvid = "1.3.6.1.2.1.17.7.1.4.5.1.1";

    public TlSg108PeSwitchAccessService(ILogger<TlSg108PeSwitchAccessService> logger)
    {
        _logger = logger;
    }

    public async Task<int> GetPortCountAsync(string ipAddress)
    {
        try
        {
            _logger.LogInformation("Getting port count from switch at {IpAddress}", ipAddress);
            
            var endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), 161);
            var community = new OctetString("public");
            var oid = new ObjectIdentifier(IfNumberOid);
            
            var result = await Task.Run(() => 
            {
                var message = Messenger.Get(VersionCode.V1, endpoint, community, new List<Variable> { new Variable(oid) }, 1500);
                return message;
            });

            if (result.Count > 0)
            {
                var portCount = int.Parse(result[0].Data.ToString());
                _logger.LogInformation("Switch has {PortCount} interfaces", portCount);
                
                // TL-SG108PE has 8 physical ports, but SNMP might report additional virtual interfaces
                // Return the actual physical port count (8 for this model)
                return 8;
            }

            _logger.LogWarning("No response received from switch");
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting port count from switch at {IpAddress}", ipAddress);
            throw;
        }
    }

    public async Task<IEnumerable<PortInfo>> GetPortVlansAsync(string ipAddress)
    {
        try
        {
            _logger.LogInformation("Getting port VLAN information from switch at {IpAddress}", ipAddress);
            
            var endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), 161);
            var community = new OctetString("public");
            var portInfoList = new List<PortInfo>();

            // For TL-SG108PE, we query the PVID (Port VLAN ID) for each port
            // Ports are typically numbered 1-8
            for (int port = 1; port <= 8; port++)
            {
                try
                {
                    // Query PVID for this port
                    // OID format: dot1qPvid.port
                    var oid = new ObjectIdentifier($"{Dot1qPvid}.{port}");
                    
                    var result = await Task.Run(() =>
                    {
                        var message = Messenger.Get(VersionCode.V1, endpoint, community, new List<Variable> { new Variable(oid) }, 1500);
                        return message;
                    });

                    if (result.Count > 0)
                    {
                        var vlanId = int.Parse(result[0].Data.ToString());
                        portInfoList.Add(new PortInfo(port, vlanId));
                        _logger.LogDebug("Port {Port} is assigned to VLAN {VlanId}", port, vlanId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not get VLAN ID for port {Port}", port);
                    // Add default VLAN 1 if query fails
                    portInfoList.Add(new PortInfo(port, 1));
                }
            }

            _logger.LogInformation("Retrieved VLAN information for {Count} ports", portInfoList.Count);
            return portInfoList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting port VLAN information from switch at {IpAddress}", ipAddress);
            throw;
        }
    }
}
