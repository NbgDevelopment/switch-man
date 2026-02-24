using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NbgDev.SwitchMan.App.Models;
using NbgDev.SwitchMan.App.Services;
using Shouldly;

namespace NbgDev.SwitchMan.App.Tests.Services;

[TestFixture]
public class ConfigurationServiceTests
{
    private IConfiguration _mockConfiguration = null!;
    private IDataProtectionProvider _dataProtectionProvider = null!;
    private ILogger<ConfigurationService> _mockLogger = null!;
    private string _testConfigPath = null!;

    [SetUp]
    public void SetUp()
    {
        _mockConfiguration = Substitute.For<IConfiguration>();
        _mockLogger = Substitute.For<ILogger<ConfigurationService>>();
        _dataProtectionProvider = new EphemeralDataProtectionProvider();
        
        // Use a unique temp directory for each test
        _testConfigPath = Path.Combine(Path.GetTempPath(), $"switchman_test_{Guid.NewGuid()}");
        
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

    [Test]
    public void LoadSwitches_ShouldReturnEmptyList_WhenFileDoesNotExist()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);

        // Act
        var switches = service.LoadSwitches();

        // Assert
        switches.ShouldNotBeNull();
        switches.ShouldBeEmpty();
    }

    [Test]
    public void SaveSwitches_AndLoadSwitches_ShouldRoundTrip()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);
        var original = new List<Switch>
        {
            new Switch("Core Switch", "10.0.0.1"),
            new Switch("Edge Switch", "192.168.1.1")
        };

        // Act
        service.SaveSwitches(original);
        var loaded = service.LoadSwitches();

        // Assert
        loaded.ShouldNotBeNull();
        loaded.Count.ShouldBe(2);
        loaded.ShouldContain(s => s.Name == "Core Switch" && s.IpAddress == "10.0.0.1");
        loaded.ShouldContain(s => s.Name == "Edge Switch" && s.IpAddress == "192.168.1.1");
    }

    [Test]
    public void SaveSwitches_ShouldPersistSwitchesToFile()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);
        var switches = new List<Switch>
        {
            new Switch("Test Switch", "172.16.0.1")
        };

        // Act
        service.SaveSwitches(switches);

        // Assert - file should exist and contain switch data
        var switchesFilePath = Path.Combine(_testConfigPath, "switches.json");
        File.Exists(switchesFilePath).ShouldBeTrue();
        var json = File.ReadAllText(switchesFilePath);
        json.ShouldContain("Test Switch");
        json.ShouldContain("172.16.0.1");
    }

    [Test]
    public void SaveSwitches_ThenRemove_ShouldUpdatePersistedData()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _dataProtectionProvider, _mockLogger);
        var switches = new List<Switch>
        {
            new Switch("Switch A", "10.0.0.1"),
            new Switch("Switch B", "10.0.0.2")
        };
        service.SaveSwitches(switches);

        // Act - save with only one switch
        service.SaveSwitches(new List<Switch> { new Switch("Switch A", "10.0.0.1") });
        var loaded = service.LoadSwitches();

        // Assert
        loaded.Count.ShouldBe(1);
        loaded[0].Name.ShouldBe("Switch A");
    }
}
