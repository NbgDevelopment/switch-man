using Microsoft.AspNetCore.Components;
using NbgDev.SwitchMan.App.Models;
using NbgDev.SwitchMan.App.Services;

namespace NbgDev.SwitchMan.App.Components.Pages;

public partial class Settings
{
    [Inject]
    private IConfigurationService ConfigurationService { get; set; } = null!;

    // Omada configuration form fields
    private string omadaControllerUrl = string.Empty;
    private string omadaId = string.Empty;
    private string omadaClientId = string.Empty;
    private string omadaClientSecret = string.Empty;
    private bool omadaAllowInvalidCertificate = false;
    private string omadaErrorMessage = string.Empty;
    private string omadaSuccessMessage = string.Empty;
    private bool omadaIsConfigured = false;

    protected override void OnInitialized()
    {
        var settings = ConfigurationService.LoadOmadaSettings();
        if (settings is not null)
        {
            omadaControllerUrl = settings.ControllerUrl;
            omadaId = settings.OmadaId;
            omadaClientId = settings.ClientId;
            omadaClientSecret = settings.ClientSecret;
            omadaAllowInvalidCertificate = settings.AllowInvalidCertificate;
            omadaIsConfigured = settings.IsConfigured;
        }
    }

    internal static bool IsValidControllerUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return false;

        if (uri.Scheme != "http" && uri.Scheme != "https")
            return false;

        if (!string.IsNullOrEmpty(uri.Query))
            return false;

        if (!string.IsNullOrEmpty(uri.Fragment))
            return false;

        // Reconstruct expected URL without any path (only scheme://host or scheme://host:port)
        var expectedUrl = uri.IsDefaultPort
            ? $"{uri.Scheme}://{uri.Host}"
            : $"{uri.Scheme}://{uri.Host}:{uri.Port}";

        return url == expectedUrl;
    }

    private void SaveOmadaSettings()
    {
        omadaErrorMessage = string.Empty;
        omadaSuccessMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(omadaControllerUrl))
        {
            omadaErrorMessage = "Controller URL is required.";
            return;
        }

        if (!IsValidControllerUrl(omadaControllerUrl))
        {
            omadaErrorMessage = "Controller URL must contain only the protocol (http or https), the hostname and optionally a port. Example: https://omada.example.com:8043";
            return;
        }

        if (string.IsNullOrWhiteSpace(omadaId))
        {
            omadaErrorMessage = "Omada ID is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(omadaClientId))
        {
            omadaErrorMessage = "Client ID is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(omadaClientSecret))
        {
            omadaErrorMessage = "Client Secret is required.";
            return;
        }

        try
        {
            var settings = new OmadaSettings
            {
                ControllerUrl = omadaControllerUrl.Trim(),
                OmadaId = omadaId.Trim(),
                ClientId = omadaClientId.Trim(),
                ClientSecret = omadaClientSecret,
                AllowInvalidCertificate = omadaAllowInvalidCertificate
            };
            ConfigurationService.SaveOmadaSettings(settings);
            omadaIsConfigured = settings.IsConfigured;
            omadaSuccessMessage = "Omada Controller configuration saved successfully.";
        }
        catch (InvalidOperationException ex)
        {
            omadaErrorMessage = ex.Message;
        }
    }
}

