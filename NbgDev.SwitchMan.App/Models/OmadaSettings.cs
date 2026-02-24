namespace NbgDev.SwitchMan.App.Models;

public class OmadaSettings
{
    public string ControllerUrl { get; set; } = string.Empty;
    public string OmadaId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public bool AllowInvalidCertificate { get; set; } = false;

    public bool IsConfigured =>
        !string.IsNullOrEmpty(ControllerUrl) &&
        !string.IsNullOrEmpty(OmadaId) &&
        !string.IsNullOrEmpty(ClientId) &&
        !string.IsNullOrEmpty(ClientSecret);
}
