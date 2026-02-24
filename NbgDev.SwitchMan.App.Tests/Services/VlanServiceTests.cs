using NSubstitute;
using NbgDev.SwitchMan.App.Models;
using NbgDev.SwitchMan.App.Services;
using Shouldly;

namespace NbgDev.SwitchMan.App.Tests.Services;

[TestFixture]
public class VlanServiceTests
{
    private IConfigurationService _mockConfigurationService = null!;
    private VlanService _vlanService = null!;

    [SetUp]
    public void SetUp()
    {
        _mockConfigurationService = Substitute.For<IConfigurationService>();
        _mockConfigurationService.LoadConfiguration().Returns(new List<Vlan>());
        _vlanService = new VlanService(_mockConfigurationService);
    }

    [Test]
    public void Constructor_ShouldLoadExistingConfiguration()
    {
        // Arrange
        var existingVlans = new List<Vlan>
        {
            new Vlan("Management"),
            new Vlan("Guest")
        };
        var mockConfigService = Substitute.For<IConfigurationService>();
        mockConfigService.LoadConfiguration().Returns(existingVlans);

        // Act
        var service = new VlanService(mockConfigService);
        var vlans = service.GetVlans();

        // Assert
        vlans.Count.ShouldBe(2);
        vlans.ShouldContain(v => v.Name == "Management");
        vlans.ShouldContain(v => v.Name == "Guest");
    }

    [Test]
    public void GetVlans_ShouldReturnEmptyCollection_WhenNoVlansAdded()
    {
        // Act
        var vlans = _vlanService.GetVlans();

        // Assert
        vlans.ShouldNotBeNull();
        vlans.ShouldBeEmpty();
    }

    [Test]
    public void AddVlan_ShouldAddVlanToCollection()
    {
        // Arrange
        var vlan = new Vlan("Production");

        // Act
        _vlanService.AddVlan(vlan);
        var vlans = _vlanService.GetVlans();

        // Assert
        vlans.Count.ShouldBe(1);
        vlans.ShouldContain(v => v.Name == "Production");
    }

    [Test]
    public void AddVlan_ShouldCallSaveConfiguration()
    {
        // Arrange
        var vlan = new Vlan("Development");

        // Act
        _vlanService.AddVlan(vlan);

        // Assert
        _mockConfigurationService.Received(1).SaveConfiguration(Arg.Any<IEnumerable<Vlan>>());
    }

    [Test]
    public void RemoveVlan_ShouldRemoveVlanFromCollection()
    {
        // Arrange
        var vlan = new Vlan("ToRemove");
        _vlanService.AddVlan(vlan);

        // Act
        _vlanService.RemoveVlan(vlan);
        var vlans = _vlanService.GetVlans();

        // Assert
        vlans.ShouldBeEmpty();
    }

    [Test]
    public void RemoveVlan_ShouldCallSaveConfiguration()
    {
        // Arrange
        var vlan = new Vlan("ToRemove");
        _vlanService.AddVlan(vlan);
        _mockConfigurationService.ClearReceivedCalls();

        // Act
        _vlanService.RemoveVlan(vlan);

        // Assert
        _mockConfigurationService.Received(1).SaveConfiguration(Arg.Any<IEnumerable<Vlan>>());
    }

    [Test]
    public void UpdateVlan_ShouldUpdateVlanInCollection()
    {
        // Arrange
        var oldVlan = new Vlan("OldName");
        var newVlan = new Vlan("NewName");
        _vlanService.AddVlan(oldVlan);

        // Act
        _vlanService.UpdateVlan(oldVlan, newVlan);
        var vlans = _vlanService.GetVlans();

        // Assert
        vlans.Count.ShouldBe(1);
        vlans.ShouldContain(v => v.Name == "NewName");
    }

    [Test]
    public void UpdateVlan_ShouldCallSaveConfiguration()
    {
        // Arrange
        var oldVlan = new Vlan("OldName");
        var newVlan = new Vlan("NewName");
        _vlanService.AddVlan(oldVlan);
        _mockConfigurationService.ClearReceivedCalls();

        // Act
        _vlanService.UpdateVlan(oldVlan, newVlan);

        // Assert
        _mockConfigurationService.Received(1).SaveConfiguration(Arg.Any<IEnumerable<Vlan>>());
    }

    [Test]
    public void UpdateVlan_ShouldDoNothing_WhenOldVlanNotFound()
    {
        // Arrange
        var oldVlan = new Vlan("NonExistent");
        var newVlan = new Vlan("NewName");

        // Act
        _vlanService.UpdateVlan(oldVlan, newVlan);
        var vlans = _vlanService.GetVlans();

        // Assert
        vlans.ShouldBeEmpty();
    }
}
