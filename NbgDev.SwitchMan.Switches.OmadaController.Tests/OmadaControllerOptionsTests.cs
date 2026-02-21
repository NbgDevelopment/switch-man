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
            {"OmadaController:OmadaId", "test-omada-id"},
            {"OmadaController:ClientId", "test-client-id"},
            {"OmadaController:ClientSecret", "test-client-secret"}
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
        value.OmadaId.ShouldBe("test-omada-id");
        value.ClientId.ShouldBe("test-client-id");
        value.ClientSecret.ShouldBe("test-client-secret");
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
        value.OmadaId.ShouldBe(string.Empty);
        value.ClientId.ShouldBe(string.Empty);
        value.ClientSecret.ShouldBe(string.Empty);
    }
    
    [Test]
    public void Options_ShouldSupportPartialConfiguration()
    {
        // Arrange - Only override some values
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"OmadaController:ControllerUrl", "https://custom.example.com:8043"}
            // OmadaId, ClientId and ClientSecret will use defaults
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
        value.OmadaId.ShouldBe(string.Empty); // Default
        value.ClientId.ShouldBe(string.Empty); // Default
        value.ClientSecret.ShouldBe(string.Empty); // Default
    }
    
    [Test]
    public void Options_AllowInvalidCertificate_ShouldDefaultToFalse()
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

        // Assert - Should default to false (secure by default)
        value.ShouldNotBeNull();
        value.AllowInvalidCertificate.ShouldBeFalse();
    }
    
    [Test]
    public void Options_AllowInvalidCertificate_ShouldBindFromConfiguration()
    {
        // Arrange - Configure to allow invalid certificates
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"OmadaController:ControllerUrl", "https://test.example.com:8043"},
            {"OmadaController:OmadaId", "test-omada-id"},
            {"OmadaController:ClientId", "test-client-id"},
            {"OmadaController:ClientSecret", "test-client-secret"},
            {"OmadaController:AllowInvalidCertificate", "true"}
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
        value.AllowInvalidCertificate.ShouldBeTrue();
    }
}
