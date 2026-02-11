using System.Text.Json;
using NbgDev.SwitchMan.App.Models;

namespace NbgDev.SwitchMan.App.Services;

public class ConfigurationService : IConfigurationService
{
    private const string DefaultConfigPath = "config";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _configFilePath;
    private readonly string _switchesFilePath;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(IConfiguration configuration, ILogger<ConfigurationService> logger)
    {
        _logger = logger;
        
        // Read config path from configuration (can be overridden by environment variable)
        var configPath = configuration.GetValue<string>("SwitchMan:ConfigPath") ?? DefaultConfigPath;
        
        try
        {
            // Ensure the config directory exists
            Directory.CreateDirectory(configPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create configuration directory at '{ConfigPath}'.", configPath);
            throw new InvalidOperationException($"Failed to create configuration directory at '{configPath}'. Please check permissions.", ex);
        }
        
        _configFilePath = Path.Combine(configPath, "vlans.json");
        _switchesFilePath = Path.Combine(configPath, "switches.json");
        _logger.LogInformation("Configuration file path: {ConfigFilePath}", _configFilePath);
        _logger.LogInformation("Switches file path: {SwitchesFilePath}", _switchesFilePath);
    }

    public List<Vlan> LoadConfiguration()
    {
        try
        {
            if (!File.Exists(_configFilePath))
            {
                _logger.LogInformation("Configuration file not found. Starting with empty configuration.");
                return new List<Vlan>();
            }

            var json = File.ReadAllText(_configFilePath);
            var vlans = JsonSerializer.Deserialize<List<Vlan>>(json) ?? new List<Vlan>();
            _logger.LogInformation("Loaded {Count} VLANs from configuration file.", vlans.Count);
            return vlans;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading configuration file. Starting with empty configuration.");
            return new List<Vlan>();
        }
    }

    public void SaveConfiguration(IEnumerable<Vlan> vlans)
    {
        try
        {
            var json = JsonSerializer.Serialize(vlans, JsonOptions);
            File.WriteAllText(_configFilePath, json);
            _logger.LogInformation("Saved {Count} VLANs to configuration file.", vlans.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save VLAN configuration to file.");
            throw new InvalidOperationException("Failed to save VLAN configuration. Please check file permissions and disk space.", ex);
        }
    }

    public List<Switch> LoadSwitches()
    {
        try
        {
            if (!File.Exists(_switchesFilePath))
            {
                _logger.LogInformation("Switches configuration file not found. Starting with empty configuration.");
                return new List<Switch>();
            }

            var json = File.ReadAllText(_switchesFilePath);
            var switches = JsonSerializer.Deserialize<List<Switch>>(json) ?? new List<Switch>();
            _logger.LogInformation("Loaded {Count} switches from configuration file.", switches.Count);
            return switches;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading switches configuration file. Starting with empty configuration.");
            return new List<Switch>();
        }
    }

    public void SaveSwitches(IEnumerable<Switch> switches)
    {
        try
        {
            var json = JsonSerializer.Serialize(switches, JsonOptions);
            File.WriteAllText(_switchesFilePath, json);
            _logger.LogInformation("Saved {Count} switches to configuration file.", switches.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save switches configuration to file.");
            throw new InvalidOperationException("Failed to save switches configuration. Please check file permissions and disk space.", ex);
        }
    }
}
