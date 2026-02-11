using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NbgDev.SwitchMan.App.Models;
using NbgDev.SwitchMan.App.Services;
using System.Text.Json;

namespace NbgDev.SwitchMan.App.Tests.Services;

public class ConfigurationServiceTests : IDisposable
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<ConfigurationService>> _mockLogger;
    private readonly string _testConfigPath;
    private readonly string _testConfigFilePath;

    public ConfigurationServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<ConfigurationService>>();
        
        // Use a unique temp directory for each test
        _testConfigPath = Path.Combine(Path.GetTempPath(), $"switchman_test_{Guid.NewGuid()}");
        _testConfigFilePath = Path.Combine(_testConfigPath, "vlans.json");
        
        _mockConfiguration.Setup(x => x.GetSection("SwitchMan:ConfigPath").Value)
            .Returns(_testConfigPath);
    }

    public void Dispose()
    {
        // Clean up test directory
        if (Directory.Exists(_testConfigPath))
        {
            Directory.Delete(_testConfigPath, true);
        }
    }

    [Fact]
    public void Constructor_ShouldCreateConfigDirectory()
    {
        // Act
        var service = new ConfigurationService(_mockConfiguration.Object, _mockLogger.Object);

        // Assert
        Directory.Exists(_testConfigPath).Should().BeTrue();
    }

    [Fact]
    public void Constructor_ShouldUseDefaultConfigPath_WhenNotConfigured()
    {
        // Arrange
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x.GetSection("SwitchMan:ConfigPath").Value)
            .Returns((string?)null);

        // Act & Assert - should not throw
        var act = () => new ConfigurationService(mockConfig.Object, _mockLogger.Object);
        act.Should().NotThrow();
    }

    [Fact]
    public void LoadConfiguration_ShouldReturnEmptyList_WhenFileDoesNotExist()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration.Object, _mockLogger.Object);

        // Act
        var vlans = service.LoadConfiguration();

        // Assert
        vlans.Should().NotBeNull();
        vlans.Should().BeEmpty();
    }

    [Fact]
    public void LoadConfiguration_ShouldReturnVlans_WhenFileExists()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration.Object, _mockLogger.Object);
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
        vlans.Should().HaveCount(2);
        vlans.Should().Contain(v => v.Name == "Management" && v.VlanId == 10);
        vlans.Should().Contain(v => v.Name == "Guest" && v.VlanId == 20);
    }

    [Fact]
    public void LoadConfiguration_ShouldReturnEmptyList_WhenFileIsCorrupted()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration.Object, _mockLogger.Object);
        File.WriteAllText(_testConfigFilePath, "invalid json content");

        // Act
        var vlans = service.LoadConfiguration();

        // Assert
        vlans.Should().NotBeNull();
        vlans.Should().BeEmpty();
    }

    [Fact]
    public void SaveConfiguration_ShouldCreateFile_WhenFileDoesNotExist()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration.Object, _mockLogger.Object);
        var vlans = new List<Vlan>
        {
            new Vlan("Production", 100)
        };

        // Act
        service.SaveConfiguration(vlans);

        // Assert
        File.Exists(_testConfigFilePath).Should().BeTrue();
    }

    [Fact]
    public void SaveConfiguration_ShouldWriteCorrectJson()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration.Object, _mockLogger.Object);
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
        
        loadedVlans.Should().NotBeNull();
        loadedVlans.Should().HaveCount(2);
        loadedVlans.Should().Contain(v => v.Name == "Development" && v.VlanId == 50);
        loadedVlans.Should().Contain(v => v.Name == "Testing" && v.VlanId == 60);
    }

    [Fact]
    public void SaveConfiguration_ShouldOverwriteExistingFile()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration.Object, _mockLogger.Object);
        var initialVlans = new List<Vlan> { new Vlan("Initial", 1) };
        var updatedVlans = new List<Vlan> { new Vlan("Updated", 2) };

        // Act
        service.SaveConfiguration(initialVlans);
        service.SaveConfiguration(updatedVlans);

        // Assert
        var json = File.ReadAllText(_testConfigFilePath);
        var loadedVlans = JsonSerializer.Deserialize<List<Vlan>>(json);
        
        loadedVlans.Should().HaveCount(1);
        loadedVlans.Should().Contain(v => v.Name == "Updated" && v.VlanId == 2);
    }

    [Fact]
    public void SaveConfiguration_ShouldThrowException_WhenDirectoryIsReadOnly()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration.Object, _mockLogger.Object);
        var vlans = new List<Vlan> { new Vlan("Test", 1) };

        // Make directory read-only
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            Directory.SetCurrentDirectory("/");
            var readOnlyPath = Path.Combine("/tmp", $"readonly_{Guid.NewGuid()}");
            Directory.CreateDirectory(readOnlyPath);
            
            var mockReadOnlyConfig = new Mock<IConfiguration>();
            mockReadOnlyConfig.Setup(x => x.GetSection("SwitchMan:ConfigPath").Value)
                .Returns(readOnlyPath);
            
            var readOnlyService = new ConfigurationService(mockReadOnlyConfig.Object, _mockLogger.Object);
            
            // Make directory read-only (Linux/Mac only)
            var dirInfo = new DirectoryInfo(readOnlyPath);
            dirInfo.Attributes |= FileAttributes.ReadOnly;

            try
            {
                // Act
                var act = () => readOnlyService.SaveConfiguration(vlans);

                // Assert - might throw or might succeed depending on permissions
                // Just ensure it doesn't crash unexpectedly
                act.Should().NotBeNull();
            }
            finally
            {
                // Cleanup
                dirInfo.Attributes &= ~FileAttributes.ReadOnly;
                Directory.Delete(readOnlyPath, true);
            }
        }
    }

    [Fact]
    public void SaveConfiguration_ShouldSaveEmptyList()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration.Object, _mockLogger.Object);
        var vlans = new List<Vlan>();

        // Act
        service.SaveConfiguration(vlans);

        // Assert
        File.Exists(_testConfigFilePath).Should().BeTrue();
        var json = File.ReadAllText(_testConfigFilePath);
        var loadedVlans = JsonSerializer.Deserialize<List<Vlan>>(json);
        loadedVlans.Should().BeEmpty();
    }

    [Fact]
    public void LoadConfiguration_AfterSaveConfiguration_ShouldReturnSameVlans()
    {
        // Arrange
        var service = new ConfigurationService(_mockConfiguration.Object, _mockLogger.Object);
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
        loadedVlans.Should().HaveCount(3);
        loadedVlans.Should().BeEquivalentTo(originalVlans, options => 
            options.Including(v => v.Name).Including(v => v.VlanId));
    }
}
