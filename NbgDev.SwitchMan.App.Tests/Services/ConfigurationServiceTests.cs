using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NbgDev.SwitchMan.App.Models;
using NbgDev.SwitchMan.App.Services;
using Shouldly;
using System.Text.Json;

namespace NbgDev.SwitchMan.App.Tests.Services;

[TestFixture]
public class ConfigurationServiceTests
{
    private IConfiguration _mockConfiguration = null!;
    private IDataProtectionProvider _dataProtectionProvider = null!;
    private ILogger<ConfigurationService> _mockLogger = null!;
    private string _testConfigPath = null!;
    private string _testConfigFilePath = null!;

    [SetUp]
    public void SetUp()
    {
        _mockConfiguration = Substitute.For<IConfiguration>();
        _mockLogger = Substitute.For<ILogger<ConfigurationService>>();
        _dataProtectionProvider = new EphemeralDataProtectionProvider();
        
        // Use a unique temp directory for each test
        _testConfigPath = Path.Combine(Path.GetTempPath(), $"switchman_test_{Guid.NewGuid()}");
        _testConfigFilePath = Path.Combine(_testConfigPath, "vlans.json");
        
        _mockConfiguration.GetSection("SwitchMan:ConfigPath").Value.Returns(_testConfigPath);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up test directory
        if (Directory.Exists(_testConfigPath))
        {
            Directory.Delete(_testConfigPath, true);
        }
    }

    [Test]
    public void Constructor_ShouldCreateConfigDirectory()
    {
        // Act
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);

        // Assert
        Directory.Exists(_testConfigPath).ShouldBeTrue();
    }

    [Test]
    public void Constructor_ShouldUseDefaultConfigPath_WhenNotConfigured()
    {
        // Arrange
        var mockConfig = Substitute.For<IConfiguration>();
        mockConfig.GetSection("SwitchMan:ConfigPath").Value.Returns((string?)null);

        // Act & Assert - should not throw
        Should.NotThrow(() => new ConfigurationService(mockConfig, _dataProtectionProvider, _mockLogger));
    }

    [Test]
    public void LoadConfiguration_ShouldReturnEmptyList_WhenFileDoesNotExist()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);

        // Act
        var vlans = service.LoadConfiguration();

        // Assert
        vlans.ShouldNotBeNull();
        vlans.ShouldBeEmpty();
    }

    [Test]
    public void LoadConfiguration_ShouldReturnVlans_WhenFileExists()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);
        var testVlans = new List<Vlan>
        {
            new Vlan("Management", 10),
            new Vlan("Guest", 20)
        };
        
        var json = JsonSerializer.Serialize(testVlans, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_testConfigFilePath, json);

        // Act
        var vlans = service.LoadConfiguration();

        // Assert
        vlans.Count.ShouldBe(2);
        vlans.ShouldContain(v => v.Name == "Management" && v.VlanId == 10);
        vlans.ShouldContain(v => v.Name == "Guest" && v.VlanId == 20);
    }

    [Test]
    public void LoadConfiguration_ShouldReturnEmptyList_WhenFileIsCorrupted()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);
        File.WriteAllText(_testConfigFilePath, "invalid json content");

        // Act
        var vlans = service.LoadConfiguration();

        // Assert
        vlans.ShouldNotBeNull();
        vlans.ShouldBeEmpty();
    }

    [Test]
    public void SaveConfiguration_ShouldCreateFile_WhenFileDoesNotExist()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);
        var vlans = new List<Vlan>
        {
            new Vlan("Production", 100)
        };

        // Act
        service.SaveConfiguration(vlans);

        // Assert
        File.Exists(_testConfigFilePath).ShouldBeTrue();
    }

    [Test]
    public void SaveConfiguration_ShouldWriteCorrectJson()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);
        var vlans = new List<Vlan>
        {
            new Vlan("Development", 50),
            new Vlan("Testing", 60)
        };

        // Act
        service.SaveConfiguration(vlans);

        // Assert
        var json = File.ReadAllText(_testConfigFilePath);
        var loadedVlans = JsonSerializer.Deserialize<List<Vlan>>(json);
        
        loadedVlans.ShouldNotBeNull();
        loadedVlans.Count.ShouldBe(2);
        loadedVlans.ShouldContain(v => v.Name == "Development" && v.VlanId == 50);
        loadedVlans.ShouldContain(v => v.Name == "Testing" && v.VlanId == 60);
    }

    [Test]
    public void SaveConfiguration_ShouldOverwriteExistingFile()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);
        var initialVlans = new List<Vlan> { new Vlan("Initial", 1) };
        var updatedVlans = new List<Vlan> { new Vlan("Updated", 2) };

        // Act
        service.SaveConfiguration(initialVlans);
        service.SaveConfiguration(updatedVlans);

        // Assert
        var json = File.ReadAllText(_testConfigFilePath);
        var loadedVlans = JsonSerializer.Deserialize<List<Vlan>>(json);
        
        loadedVlans.ShouldNotBeNull();
        loadedVlans.Count.ShouldBe(1);
        loadedVlans.ShouldContain(v => v.Name == "Updated" && v.VlanId == 2);
    }

    [Test]
    public void SaveConfiguration_ShouldSaveEmptyList()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);
        var vlans = new List<Vlan>();

        // Act
        service.SaveConfiguration(vlans);

        // Assert
        File.Exists(_testConfigFilePath).ShouldBeTrue();
        var json = File.ReadAllText(_testConfigFilePath);
        var loadedVlans = JsonSerializer.Deserialize<List<Vlan>>(json);
        loadedVlans.ShouldBeEmpty();
    }

    [Test]
    public void LoadConfiguration_AfterSaveConfiguration_ShouldReturnSameVlans()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);
        var originalVlans = new List<Vlan>
        {
            new Vlan("VLAN1", 10),
            new Vlan("VLAN2", 20),
            new Vlan("VLAN3", 30)
        };

        // Act
        service.SaveConfiguration(originalVlans);
        var loadedVlans = service.LoadConfiguration();

        // Assert
        loadedVlans.Count.ShouldBe(3);
        foreach (var original in originalVlans)
        {
            loadedVlans.ShouldContain(v => v.Name == original.Name && v.VlanId == original.VlanId);
        }
    }

    [Test]
    public void LoadOmadaSettings_ShouldReturnNull_WhenFileDoesNotExist()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);

        // Act
        var settings = service.LoadOmadaSettings();

        // Assert
        settings.ShouldBeNull();
    }

    [Test]
    public void SaveOmadaSettings_AndLoadOmadaSettings_ShouldRoundTrip()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);
        var original = new OmadaSettings
        {
            ControllerUrl = "https://omada.example.com:8043",
            OmadaId = "test-omada-id",
            ClientId = "test-client-id",
            ClientSecret = "super-secret-value",
            AllowInvalidCertificate = true
        };

        // Act
        service.SaveOmadaSettings(original);
        var loaded = service.LoadOmadaSettings();

        // Assert
        loaded.ShouldNotBeNull();
        loaded.ControllerUrl.ShouldBe(original.ControllerUrl);
        loaded.OmadaId.ShouldBe(original.OmadaId);
        loaded.ClientId.ShouldBe(original.ClientId);
        loaded.ClientSecret.ShouldBe(original.ClientSecret);
        loaded.AllowInvalidCertificate.ShouldBe(original.AllowInvalidCertificate);
    }

    [Test]
    public void SaveOmadaSettings_ShouldEncryptClientSecret()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);
        var settings = new OmadaSettings
        {
            ControllerUrl = "https://omada.example.com:8043",
            OmadaId = "test-omada-id",
            ClientId = "test-client-id",
            ClientSecret = "my-secret",
            AllowInvalidCertificate = false
        };

        // Act
        service.SaveOmadaSettings(settings);

        // Assert - file should not contain the plain-text secret
        var omadaFilePath = Path.Combine(_testConfigPath, "omada.json");
        var json = File.ReadAllText(omadaFilePath);
        json.ShouldNotContain("my-secret");
        json.ShouldContain("ProtectedClientSecret");
    }

    [Test]
    public void LoadOmadaSettings_ShouldReturnIsConfiguredTrue_WhenAllFieldsSet()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);
        var settings = new OmadaSettings
        {
            ControllerUrl = "https://omada.example.com:8043",
            OmadaId = "omada-id",
            ClientId = "client-id",
            ClientSecret = "secret",
            AllowInvalidCertificate = false
        };
        service.SaveOmadaSettings(settings);

        // Act
        var loaded = service.LoadOmadaSettings();

        // Assert
        loaded.ShouldNotBeNull();
        loaded.IsConfigured.ShouldBeTrue();
    }
}
