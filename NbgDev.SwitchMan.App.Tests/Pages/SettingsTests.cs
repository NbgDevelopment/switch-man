using NbgDev.SwitchMan.App.Components.Pages;
using Shouldly;

namespace NbgDev.SwitchMan.App.Tests.Pages;

[TestFixture]
public class SettingsTests
{
    [TestCase("https://omada.example.com:8043")]
    [TestCase("https://omada.example.com")]
    [TestCase("http://localhost")]
    [TestCase("http://localhost:8080")]
    [TestCase("https://192.168.1.100:8043")]
    [TestCase("http://192.168.1.100")]
    public void IsValidControllerUrl_ShouldReturnTrue_ForValidUrls(string url)
    {
        Settings.IsValidControllerUrl(url).ShouldBeTrue();
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase("not-a-url")]
    [TestCase("https://omada.example.com:8043/")]
    [TestCase("https://omada.example.com:8043/api")]
    [TestCase("https://omada.example.com:8043?foo=bar")]
    [TestCase("https://omada.example.com:8043#fragment")]
    [TestCase("ftp://omada.example.com:8043")]
    [TestCase("omada.example.com:8043")]
    public void IsValidControllerUrl_ShouldReturnFalse_ForInvalidUrls(string url)
    {
        Settings.IsValidControllerUrl(url).ShouldBeFalse();
    }
}
