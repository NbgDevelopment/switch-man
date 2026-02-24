using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using NbgDev.SwitchMan.App.Models;

namespace NbgDev.SwitchMan.App.Services;

public class ConfigurationService : IConfigurationService
{
    private const string DefaultConfigPath = "config";
    private const string DataProtectionPurpose = "NbgDev.SwitchMan.OmadaClientSecret";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _configFilePath;
    private readonly string _switchesFilePath;
    private readonly string _omadaFilePath;
    private readonly IDataProtector _dataProtector;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(IConfiguration configuration, IDataProtectionProvider dataProtectionProvider, ILogger<ConfigurationService> logger)
    {
        _logger = logger;
        _dataProtector = dataProtectionProvider.CreateProtector(DataProtectionPurpose);

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
        _omadaFilePath = Path.Combine(configPath, "omada.json");
        _logger.LogInformation("Configuration file path: {ConfigFilePath}", _configFilePath);
        _logger.LogInformation("Switches file path: {SwitchesFilePath}", _switchesFilePath);
        _logger.LogInformation("Omada settings file path: {OmadaFilePath}", _omadaFilePath);
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

    public OmadaSettings? LoadOmadaSettings()
    {
        try
        {
            if (!File.Exists(_omadaFilePath))
            {
                _logger.LogInformation("Omada settings file not found.");
                return null;
            }

            var json = File.ReadAllText(_omadaFilePath);
            var fileModel = JsonSerializer.Deserialize<OmadaSettingsFile>(json, JsonOptions);
            if (fileModel is null) return null;

            var clientSecret = string.Empty;
            if (!string.IsNullOrEmpty(fileModel.ProtectedClientSecret))
            {
                try
                {
                    clientSecret = _dataProtector.Unprotect(fileModel.ProtectedClientSecret);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to decrypt Omada client secret.");
                    return null;
                }
            }

            return new OmadaSettings
            {
                ControllerUrl = fileModel.ControllerUrl,
                OmadaId = fileModel.OmadaId,
                ClientId = fileModel.ClientId,
                ClientSecret = clientSecret,
                AllowInvalidCertificate = fileModel.AllowInvalidCertificate
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Omada settings from file.");
            return null;
        }
    }

    public void SaveOmadaSettings(OmadaSettings settings)
    {
        try
        {
            var protectedSecret = _dataProtector.Protect(settings.ClientSecret);
            var fileModel = new OmadaSettingsFile(
                settings.ControllerUrl,
                settings.OmadaId,
                settings.ClientId,
                protectedSecret,
                settings.AllowInvalidCertificate);

            var json = JsonSerializer.Serialize(fileModel, JsonOptions);
            File.WriteAllText(_omadaFilePath, json);
            _logger.LogInformation("Saved Omada settings to configuration file.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save Omada settings to file.");
            throw new InvalidOperationException("Failed to save Omada settings. Please check file permissions and disk space.", ex);
        }
    }

    private record OmadaSettingsFile(
        string ControllerUrl,
        string OmadaId,
        string ClientId,
        string ProtectedClientSecret,
        bool AllowInvalidCertificate);
}
