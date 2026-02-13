using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NbgDev.SwitchMan.Switches.Contract;
using Shouldly;

namespace NbgDev.SwitchMan.Switches.TLSG108PE.Tests;

[TestFixture]
public class ServiceCollectionExtensionsTests
{
    [Test]
    public void AddTlSg108PeSwitchAccess_ShouldRegisterISwitchAccessService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(); // Add logging to satisfy dependency

        // Act
        services.AddTlSg108PeSwitchAccess();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<ISwitchAccessService>();
        service.ShouldNotBeNull();
        service.ShouldBeOfType<TlSg108PeSwitchAccessService>();
    }

    [Test]
    public void AddTlSg108PeSwitchAccess_ShouldReturnServiceCollectionForChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddTlSg108PeSwitchAccess();

        // Assert
        result.ShouldBe(services);
    }
}
