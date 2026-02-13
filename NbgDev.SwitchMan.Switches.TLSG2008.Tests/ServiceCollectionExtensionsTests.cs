using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NbgDev.SwitchMan.Switches.Contract;
using Shouldly;

namespace NbgDev.SwitchMan.Switches.TLSG2008.Tests;

[TestFixture]
public class ServiceCollectionExtensionsTests
{
    [Test]
    public void AddTlSg2008SwitchAccess_ShouldRegisterISwitchAccessService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Add logging to satisfy dependency

        // Act
        services.AddTlSg2008SwitchAccess();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<ISwitchAccessService>();
        service.ShouldNotBeNull();
        service.ShouldBeOfType<TlSg2008SwitchAccessService>();
    }

    [Test]
    public void AddTlSg2008SwitchAccess_ShouldReturnServiceCollectionForChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddTlSg2008SwitchAccess();

        // Assert
        result.ShouldBe(services);
    }
}
