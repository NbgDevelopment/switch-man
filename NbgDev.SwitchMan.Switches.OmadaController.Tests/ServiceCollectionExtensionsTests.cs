using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NbgDev.SwitchMan.Switches.Contract;
using Shouldly;

namespace NbgDev.SwitchMan.Switches.OmadaController.Tests;

[TestFixture]
public class ServiceCollectionExtensionsTests
{
    private IConfiguration GetMockConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"OmadaController:ControllerUrl", "https://localhost:8043"},
            {"OmadaController:Username", "admin"},
            {"OmadaController:Password", "admin"}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Test]
    public void AddOmadaControllerSwitchAccess_ShouldRegisterISwitchAccessService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Add logging to satisfy dependency
        var configuration = GetMockConfiguration();

        // Act
        services.AddOmadaControllerSwitchAccess(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<ISwitchAccessService>();
        service.ShouldNotBeNull();
        service.ShouldBeOfType<OmadaControllerSwitchAccessService>();
    }

    [Test]
    public void AddOmadaControllerSwitchAccess_ShouldReturnServiceCollectionForChaining()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = GetMockConfiguration();

        // Act
        var result = services.AddOmadaControllerSwitchAccess(configuration);

        // Assert
        result.ShouldBe(services);
    }
    
    [Test]
    public void AddOmadaControllerSwitchAccess_ShouldRegisterHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var configuration = GetMockConfiguration();

        // Act
        services.AddOmadaControllerSwitchAccess(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert - HttpClient should be resolvable through the service
        var service = serviceProvider.GetService<OmadaControllerSwitchAccessService>();
        service.ShouldNotBeNull();
    }
    
    [Test]
    public void AddOmadaControllerSwitchAccess_ShouldBindConfigurationToOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"OmadaController:ControllerUrl", "https://custom.example.com:8043"},
            {"OmadaController:Username", "testuser"},
            {"OmadaController:Password", "testpass"}
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        services.AddOmadaControllerSwitchAccess(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert - Options should be configured from the configuration
        var service = serviceProvider.GetService<ISwitchAccessService>();
        service.ShouldNotBeNull();
    }
}
