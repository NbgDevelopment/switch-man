using System.ComponentModel.DataAnnotations;

namespace NbgDev.SwitchMan.Switches.OmadaController;

/// <summary>
/// Configuration options for TP-Link Omada Controller access
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
    /// Username for authenticating with the Omada Controller
    /// </summary>
    [Required]
    public string Username { get; set; } = "admin";

    /// <summary>
    /// Password for authenticating with the Omada Controller
    /// </summary>
    [Required]
    public string Password { get; set; } = "admin";
}
