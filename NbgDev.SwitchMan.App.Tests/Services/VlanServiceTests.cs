using NSubstitute;
using NbgDev.SwitchMan.App.Models;
using NbgDev.SwitchMan.App.Services;
using Shouldly;

namespace NbgDev.SwitchMan.App.Tests.Services;

[TestFixture]
public class VlanServiceTests
{
    private IConfigurationService _mockConfigurationService;
    private VlanService _vlanService;

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
            new Vlan("Management", 10),
            new Vlan("Guest", 20)
        };
        var mockConfigService = Substitute.For<IConfigurationService>();
        mockConfigService.LoadConfiguration().Returns(existingVlans);

        // Act
        var service = new VlanService(mockConfigService);
        var vlans = service.GetVlans();

        // Assert
        vlans.Count.ShouldBe(2);
        vlans.ShouldContain(v => v.Name == "Management" && v.VlanId == 10);
        vlans.ShouldContain(v => v.Name == "Guest" && v.VlanId == 20);
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
        var vlan = new Vlan("Production", 100);

        // Act
        _vlanService.AddVlan(vlan);
        var vlans = _vlanService.GetVlans();

        // Assert
        vlans.Count.ShouldBe(1);
        vlans.ShouldContain(v => v.Name == "Production" && v.VlanId == 100);
    }

    [Test]
    public void AddVlan_ShouldCallSaveConfiguration()
    {
        // Arrange
        var vlan = new Vlan("Development", 50);

        // Act
        _vlanService.AddVlan(vlan);

        // Assert
        _mockConfigurationService.Received(1).SaveConfiguration(Arg.Any<IEnumerable<Vlan>>());
    }

    [Test]
    public void AddVlan_ShouldThrowException_WhenVlanIdAlreadyExists()
    {
        // Arrange
        var vlan1 = new Vlan("VLAN1", 100);
        var vlan2 = new Vlan("VLAN2", 100); // Same VLAN ID

        // Act
        _vlanService.AddVlan(vlan1);

        // Assert
        Should.Throw<InvalidOperationException>(() => _vlanService.AddVlan(vlan2))
            .Message.ShouldBe("A VLAN with ID 100 already exists.");
    }

    [Test]
    public void RemoveVlan_ShouldRemoveVlanFromCollection()
    {
        // Arrange
        var vlan = new Vlan("ToRemove", 200);
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
        var vlan = new Vlan("ToRemove", 200);
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
        var oldVlan = new Vlan("OldName", 300);
        var newVlan = new Vlan("NewName", 300);
        _vlanService.AddVlan(oldVlan);

        // Act
        _vlanService.UpdateVlan(oldVlan, newVlan);
        var vlans = _vlanService.GetVlans();

        // Assert
        vlans.Count.ShouldBe(1);
        vlans.ShouldContain(v => v.Name == "NewName" && v.VlanId == 300);
    }

    [Test]
    public void UpdateVlan_ShouldUpdateVlanIdWhenNotDuplicate()
    {
        // Arrange
        var oldVlan = new Vlan("VLAN", 300);
        var newVlan = new Vlan("VLAN", 400);
        _vlanService.AddVlan(oldVlan);

        // Act
        _vlanService.UpdateVlan(oldVlan, newVlan);
        var vlans = _vlanService.GetVlans();

        // Assert
        vlans.Count.ShouldBe(1);
        vlans.ShouldContain(v => v.VlanId == 400);
    }

    [Test]
    public void UpdateVlan_ShouldThrowException_WhenNewVlanIdAlreadyExists()
    {
        // Arrange
        var vlan1 = new Vlan("VLAN1", 100);
        var vlan2 = new Vlan("VLAN2", 200);
        _vlanService.AddVlan(vlan1);
        _vlanService.AddVlan(vlan2);

        var updatedVlan = new Vlan("VLAN1_Updated", 200); // Trying to change to existing VLAN ID

        // Assert
        Should.Throw<InvalidOperationException>(() => _vlanService.UpdateVlan(vlan1, updatedVlan))
            .Message.ShouldBe("A VLAN with ID 200 already exists.");
    }

    [Test]
    public void UpdateVlan_ShouldNotThrowException_WhenVlanIdRemainsTheSame()
    {
        // Arrange
        var oldVlan = new Vlan("OldName", 100);
        var newVlan = new Vlan("NewName", 100); // Same VLAN ID
        _vlanService.AddVlan(oldVlan);

        // Act & Assert
        Should.NotThrow(() => _vlanService.UpdateVlan(oldVlan, newVlan));
        var vlans = _vlanService.GetVlans();
        vlans.ShouldContain(v => v.Name == "NewName" && v.VlanId == 100);
    }

    [Test]
    public void UpdateVlan_ShouldCallSaveConfiguration()
    {
        // Arrange
        var oldVlan = new Vlan("OldName", 300);
        var newVlan = new Vlan("NewName", 300);
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
        var oldVlan = new Vlan("NonExistent", 999);
        var newVlan = new Vlan("NewName", 999);

        // Act
        _vlanService.UpdateVlan(oldVlan, newVlan);
        var vlans = _vlanService.GetVlans();

        // Assert
        vlans.ShouldBeEmpty();
    }
}
