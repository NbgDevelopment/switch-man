namespace NbgDev.SwitchMan.Switches.OmadaController.OmadaApi;

/// <summary>
/// 
/// </summary>
/// <param name="SiteId">Site ID</param>
/// <param name="Name">Name of the site should contain 1 to 64 characters.</param>
internal record SiteSummaryInfo(string SiteId, string Name);