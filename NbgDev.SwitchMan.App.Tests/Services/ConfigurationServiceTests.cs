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
    private IConfiguration _mockConfiguration;
    private ILogger<ConfigurationService> _mockLogger;
    private string _testConfigPath;
    private string _testConfigFilePath;

    [SetUp]
    public void SetUp()
    {
        _mockConfiguration = Substitute.For<IConfiguration>();
        _mockLogger = Substitute.For<ILogger<ConfigurationService>>();
        
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
        var service = new ConfigurationService(_mockConfiguration, _mockLogger);

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
        Should.NotThrow(() => new ConfigurationService(mockConfig, _mockLogger));
    }

    [Test]
    public void LoadConfiguration_ShouldReturnEmptyList_WhenFileDoesNotExist()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _mockLogger);

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
        var service = new ConfigurationService(_mockConfiguration, _mockLogger);
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
        var service = new ConfigurationService(_mockConfiguration, _mockLogger);
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
        var service = new ConfigurationService(_mockConfiguration, _mockLogger);
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
        var service = new ConfigurationService(_mockConfiguration, _mockLogger);
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
        var service = new ConfigurationService(_mockConfiguration, _mockLogger);
        var initialVlans = new List<Vlan> { new Vlan("Initial", 1) };
        var updatedVlans = new List<Vlan> { new Vlan("Updated", 2) };

        // Act
        service.SaveConfiguration(initialVlans);
        service.SaveConfiguration(updatedVlans);

        // Assert
        var json = File.ReadAllText(_testConfigFilePath);
        var loadedVlans = JsonSerializer.Deserialize<List<Vlan>>(json);
        
        loadedVlans.Count.ShouldBe(1);
        loadedVlans.ShouldContain(v => v.Name == "Updated" && v.VlanId == 2);
    }

    [Test]
    public void SaveConfiguration_ShouldSaveEmptyList()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration, _mockLogger);
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
        var service = new ConfigurationService(_mockConfiguration, _mockLogger);
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
        for (int i = 0; i < originalVlans.Count; i++)
        {
            loadedVlans[i].Name.ShouldBe(originalVlans[i].Name);
            loadedVlans[i].VlanId.ShouldBe(originalVlans[i].VlanId);
        }
    }
}
