using System.ComponentModel.DataAnnotations;

namespace NbgDev.SwitchMan.Switches.OmadaController;

/// <summary>
/// Configuration options for TP-Link Omada Controller OpenAPI access
/// </summary>
public class OmadaControllerOptions
{
    /// <summary>
    /// The URL of the Omada Controller
    /// </summary>
    [Required]
    [Url]
    public string ControllerUrl { get; set; } = "https://localhost:8043";

    /// <summary>
    /// Client ID for OAuth 2.0 authentication with Omada Controller OpenAPI.
    /// Get this from Settings > Platform Integration > Open API in the Omada Controller.
    /// </summary>
    [Required]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client Secret for OAuth 2.0 authentication with Omada Controller OpenAPI.
    /// Get this from Settings > Platform Integration > Open API in the Omada Controller.
    /// </summary>
    [Required]
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Whether to allow invalid SSL certificates (e.g., self-signed certificates).
    /// Set to true for development/testing with self-signed certificates.
    /// WARNING: Setting this to true in production environments is a security risk.
    /// </summary>
    public bool AllowInvalidCertificate { get; set; } = false;
}
