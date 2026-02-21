namespace NbgDev.SwitchMan.Switches.OmadaController.OmadaApi;

internal record OmadaAuthResponse(OmadaAuthResult Result);

internal record OmadaAuthResult(string AccessToken, string TokenType, int ExpiresIn, string RefreshToken);
