namespace NbgDev.SwitchMan.Switches.OmadaController.OmadaApi;

/// <param name="TotalRows">Total rows of all items.</param>
/// <param name="CurrentPage">Current page number.</param>
/// <param name="CurrentSize">Number of entries per page.</param>
internal record GridVoLanProfileOpenApiVo(long TotalRows, int CurrentPage, int CurrentSize, LanProfileOpenApiVo[] Data);
