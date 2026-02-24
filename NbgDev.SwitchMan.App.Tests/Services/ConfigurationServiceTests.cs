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
}
