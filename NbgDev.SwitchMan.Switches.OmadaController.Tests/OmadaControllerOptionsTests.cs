using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;

namespace NbgDev.SwitchMan.Switches.OmadaController.Tests;

[TestFixture]
public class OmadaControllerOptionsTests
{
    [Test]
    public void Options_ShouldBindFromConfiguration()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"OmadaController:ControllerUrl", "https://test.example.com:8043"},
            {"OmadaController:Username", "testuser"},
            {"OmadaController:Password", "testpassword"}
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        
        var services = new ServiceCollection();
        services.Configure<OmadaControllerOptions>(
            configuration.GetSection("OmadaController"));
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var options = serviceProvider.GetRequiredService<IOptions<OmadaControllerOptions>>();
        var value = options.Value;

        // Assert
        value.ShouldNotBeNull();
        value.ControllerUrl.ShouldBe("https://test.example.com:8043");
        value.Username.ShouldBe("testuser");
        value.Password.ShouldBe("testpassword");
    }
    
    [Test]
    public void Options_ShouldHaveDefaultValues()
    {
        // Arrange - Empty configuration
        var configuration = new ConfigurationBuilder().Build();
        
        var services = new ServiceCollection();
        services.Configure<OmadaControllerOptions>(
            configuration.GetSection("OmadaController"));
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var options = serviceProvider.GetRequiredService<IOptions<OmadaControllerOptions>>();
        var value = options.Value;

        // Assert - Should use default values
        value.ShouldNotBeNull();
        value.ControllerUrl.ShouldBe("https://localhost:8043");
        value.Username.ShouldBe("admin");
        value.Password.ShouldBe("admin");
    }
    
    [Test]
    public void Options_ShouldSupportPartialConfiguration()
    {
        // Arrange - Only override some values
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"OmadaController:ControllerUrl", "https://custom.example.com:8043"}
            // Username and Password will use defaults
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        
        var services = new ServiceCollection();
        services.Configure<OmadaControllerOptions>(
            configuration.GetSection("OmadaController"));
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var options = serviceProvider.GetRequiredService<IOptions<OmadaControllerOptions>>();
        var value = options.Value;

        // Assert
        value.ShouldNotBeNull();
        value.ControllerUrl.ShouldBe("https://custom.example.com:8043");
        value.Username.ShouldBe("admin"); // Default
        value.Password.ShouldBe("admin"); // Default
    }
}
