using Microsoft.Extensions.DependencyInjection;
using NbgDev.SwitchMan.Switches.Contract;
using Shouldly;

namespace NbgDev.SwitchMan.Switches.OmadaController.Tests;

[TestFixture]
public class ServiceCollectionExtensionsTests
{
    [Test]
    public void AddOmadaControllerSwitchAccess_ShouldRegisterISwitchAccessService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Add logging to satisfy dependency

        // Act
        services.AddOmadaControllerSwitchAccess();
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

        // Act
        var result = services.AddOmadaControllerSwitchAccess();

        // Assert
        result.ShouldBe(services);
    }
    
    [Test]
    public void AddOmadaControllerSwitchAccess_ShouldRegisterHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddOmadaControllerSwitchAccess();
        var serviceProvider = services.BuildServiceProvider();

        // Assert - HttpClient should be resolvable through the service
        var service = serviceProvider.GetService<OmadaControllerSwitchAccessService>();
        service.ShouldNotBeNull();
    }
}
