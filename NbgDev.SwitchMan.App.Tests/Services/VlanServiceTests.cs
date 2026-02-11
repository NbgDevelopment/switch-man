using FluentAssertions;
using Moq;
using NbgDev.SwitchMan.App.Models;
using NbgDev.SwitchMan.App.Services;

namespace NbgDev.SwitchMan.App.Tests.Services;

public class VlanServiceTests
{
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly VlanService _vlanService;

    public VlanServiceTests()
    {
        _mockConfigurationService = new Mock<IConfigurationService>();
        _mockConfigurationService.Setup(x => x.LoadConfiguration()).Returns(new List<Vlan>());
        _vlanService = new VlanService(_mockConfigurationService.Object);
    }

    [Fact]
    public void Constructor_ShouldLoadExistingConfiguration()
    {
        // Arrange
        var existingVlans = new List<Vlan>
        {
            new Vlan("Management", 10),
            new Vlan("Guest", 20)
        };
        var mockConfigService = new Mock<IConfigurationService>();
        mockConfigService.Setup(x => x.LoadConfiguration()).Returns(existingVlans);

        // Act
        var service = new VlanService(mockConfigService.Object);
        var vlans = service.GetVlans();

        // Assert
        vlans.Should().HaveCount(2);
        vlans.Should().Contain(v => v.Name == "Management" && v.VlanId == 10);
        vlans.Should().Contain(v => v.Name == "Guest" && v.VlanId == 20);
    }

    [Fact]
    public void GetVlans_ShouldReturnEmptyCollection_WhenNoVlansAdded()
    {
        // Act
        var vlans = _vlanService.GetVlans();

        // Assert
        vlans.Should().NotBeNull();
        vlans.Should().BeEmpty();
    }

    [Fact]
    public void AddVlan_ShouldAddVlanToCollection()
    {
        // Arrange
        var vlan = new Vlan("Production", 100);

        // Act
        _vlanService.AddVlan(vlan);
        var vlans = _vlanService.GetVlans();

        // Assert
        vlans.Should().ContainSingle();
        vlans.Should().Contain(v => v.Name == "Production" && v.VlanId == 100);
    }

    [Fact]
    public void AddVlan_ShouldCallSaveConfiguration()
    {
        // Arrange
        var vlan = new Vlan("Development", 50);

        // Act
        _vlanService.AddVlan(vlan);

        // Assert
        _mockConfigurationService.Verify(x => x.SaveConfiguration(It.IsAny<IEnumerable<Vlan>>()), Times.Once);
    }

    [Fact]
    public void AddVlan_ShouldThrowException_WhenVlanIdAlreadyExists()
    {
        // Arrange
        var vlan1 = new Vlan("VLAN1", 100);
        var vlan2 = new Vlan("VLAN2", 100); // Same VLAN ID

        // Act
        _vlanService.AddVlan(vlan1);
        var act = () => _vlanService.AddVlan(vlan2);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("A VLAN with ID 100 already exists.");
    }

    [Fact]
    public void RemoveVlan_ShouldRemoveVlanFromCollection()
    {
        // Arrange
        var vlan = new Vlan("ToRemove", 200);
        _vlanService.AddVlan(vlan);

        // Act
        _vlanService.RemoveVlan(vlan);
        var vlans = _vlanService.GetVlans();

        // Assert
        vlans.Should().BeEmpty();
    }

    [Fact]
    public void RemoveVlan_ShouldCallSaveConfiguration()
    {
        // Arrange
        var vlan = new Vlan("ToRemove", 200);
        _vlanService.AddVlan(vlan);
        _mockConfigurationService.Reset();

        // Act
        _vlanService.RemoveVlan(vlan);

        // Assert
        _mockConfigurationService.Verify(x => x.SaveConfiguration(It.IsAny<IEnumerable<Vlan>>()), Times.Once);
    }

    [Fact]
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
        vlans.Should().ContainSingle();
        vlans.Should().Contain(v => v.Name == "NewName" && v.VlanId == 300);
    }

    [Fact]
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
        vlans.Should().ContainSingle();
        vlans.Should().Contain(v => v.VlanId == 400);
    }

    [Fact]
    public void UpdateVlan_ShouldThrowException_WhenNewVlanIdAlreadyExists()
    {
        // Arrange
        var vlan1 = new Vlan("VLAN1", 100);
        var vlan2 = new Vlan("VLAN2", 200);
        _vlanService.AddVlan(vlan1);
        _vlanService.AddVlan(vlan2);

        var updatedVlan = new Vlan("VLAN1_Updated", 200); // Trying to change to existing VLAN ID

        // Act
        var act = () => _vlanService.UpdateVlan(vlan1, updatedVlan);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("A VLAN with ID 200 already exists.");
    }

    [Fact]
    public void UpdateVlan_ShouldNotThrowException_WhenVlanIdRemainsTheSame()
    {
        // Arrange
        var oldVlan = new Vlan("OldName", 100);
        var newVlan = new Vlan("NewName", 100); // Same VLAN ID
        _vlanService.AddVlan(oldVlan);

        // Act
        var act = () => _vlanService.UpdateVlan(oldVlan, newVlan);

        // Assert
        act.Should().NotThrow();
        var vlans = _vlanService.GetVlans();
        vlans.Should().Contain(v => v.Name == "NewName" && v.VlanId == 100);
    }

    [Fact]
    public void UpdateVlan_ShouldCallSaveConfiguration()
    {
        // Arrange
        var oldVlan = new Vlan("OldName", 300);
        var newVlan = new Vlan("NewName", 300);
        _vlanService.AddVlan(oldVlan);
        _mockConfigurationService.Reset();

        // Act
        _vlanService.UpdateVlan(oldVlan, newVlan);

        // Assert
        _mockConfigurationService.Verify(x => x.SaveConfiguration(It.IsAny<IEnumerable<Vlan>>()), Times.Once);
    }

    [Fact]
    public void UpdateVlan_ShouldDoNothing_WhenOldVlanNotFound()
    {
        // Arrange
        var oldVlan = new Vlan("NonExistent", 999);
        var newVlan = new Vlan("NewName", 999);

        // Act
        _vlanService.UpdateVlan(oldVlan, newVlan);
        var vlans = _vlanService.GetVlans();

        // Assert
        vlans.Should().BeEmpty();
    }
}
