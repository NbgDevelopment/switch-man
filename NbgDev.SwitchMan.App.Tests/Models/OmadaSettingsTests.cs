using NbgDev.SwitchMan.App.Models;
using Shouldly;

namespace NbgDev.SwitchMan.App.Tests.Models;

[TestFixture]
public class OmadaSettingsTests
{
    [Test]
    public void IsConfigured_ShouldReturnTrue_WhenAllFieldsAreSet()
    {
        // Arrange
        var settings = new OmadaSettings
        {
            ControllerUrl = "https://omada.example.com:8043",
            OmadaId = "omada-id",
            ClientId = "client-id",
            ClientSecret = "secret"
        };

        // Act & Assert
        settings.IsConfigured.ShouldBeTrue();
    }

    [Test]
    public void IsConfigured_ShouldReturnFalse_WhenControllerUrlIsEmpty()
    {
        // Arrange
        var settings = new OmadaSettings
        {
            ControllerUrl = string.Empty,
            OmadaId = "omada-id",
            ClientId = "client-id",
            ClientSecret = "secret"
        };

        // Act & Assert
        settings.IsConfigured.ShouldBeFalse();
    }

    [Test]
    public void IsConfigured_ShouldReturnFalse_WhenOmadaIdIsEmpty()
    {
        // Arrange
        var settings = new OmadaSettings
        {
            ControllerUrl = "https://omada.example.com:8043",
            OmadaId = string.Empty,
            ClientId = "client-id",
            ClientSecret = "secret"
        };

        // Act & Assert
        settings.IsConfigured.ShouldBeFalse();
    }

    [Test]
    public void IsConfigured_ShouldReturnFalse_WhenClientIdIsEmpty()
    {
        // Arrange
        var settings = new OmadaSettings
        {
            ControllerUrl = "https://omada.example.com:8043",
            OmadaId = "omada-id",
            ClientId = string.Empty,
            ClientSecret = "secret"
        };

        // Act & Assert
        settings.IsConfigured.ShouldBeFalse();
    }

    [Test]
    public void IsConfigured_ShouldReturnFalse_WhenClientSecretIsEmpty()
    {
        // Arrange
        var settings = new OmadaSettings
        {
            ControllerUrl = "https://omada.example.com:8043",
            OmadaId = "omada-id",
            ClientId = "client-id",
            ClientSecret = string.Empty
        };

        // Act & Assert
        settings.IsConfigured.ShouldBeFalse();
    }

    [Test]
    public void IsConfigured_ShouldReturnFalse_WhenAllFieldsAreDefault()
    {
        // Arrange
        var settings = new OmadaSettings();

        // Act & Assert
        settings.IsConfigured.ShouldBeFalse();
    }

    [Test]
    public void AllowInvalidCertificate_ShouldDefaultToFalse()
    {
        // Arrange & Act
        var settings = new OmadaSettings();

        // Assert
        settings.AllowInvalidCertificate.ShouldBeFalse();
    }
}
