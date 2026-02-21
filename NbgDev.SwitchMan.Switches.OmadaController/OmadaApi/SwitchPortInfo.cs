namespace NbgDev.SwitchMan.Switches.OmadaController.OmadaApi;

/// <param name="Port">Port ID</param>
/// <param name="Name">Port name</param>
/// <param name="ProfileId">Profile ID</param>
/// <param name="ProfileName">Profile Name</param>
internal record SwitchPortInfo(int Port, string Name, string ProfileId, string ProfileName);
