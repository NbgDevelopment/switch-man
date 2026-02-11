using System.Text.Json;
using NbgDev.SwitchMan.App.Models;

namespace NbgDev.SwitchMan.App.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly string _configFilePath;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(IConfiguration configuration, ILogger<ConfigurationService> logger)
    {
        _logger = logger;
        
        // Read config path from configuration (can be overridden by environment variable)
        var configPath = configuration.GetValue<string>("SwitchMan:ConfigPath") ?? "config";
        
        // Ensure the config directory exists
        Directory.CreateDirectory(configPath);
        
        _configFilePath = Path.Combine(configPath, "vlans.json");
        _logger.LogInformation("Configuration file path: {ConfigFilePath}", _configFilePath);
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
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(vlans, options);
            File.WriteAllText(_configFilePath, json);
            _logger.LogInformation("Saved {Count} VLANs to configuration file.", vlans.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving configuration file.");
            throw;
        }
    }
}
