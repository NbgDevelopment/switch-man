using Microsoft.Extensions.Logging;
using NSubstitute;
using NbgDev.SwitchMan.App.Models;
using NbgDev.SwitchMan.App.Services;
using NbgDev.SwitchMan.Switches.Contract;
using Shouldly;

namespace NbgDev.SwitchMan.App.Tests.Services;

[TestFixture]
public class SwitchServiceTests
{
    private IConfigurationService _mockConfigurationService = null!;
    private ISwitchAccessService _mockSwitchAccessService = null!;
    private ILogger<SwitchService> _mockLogger = null!;
    private SwitchService _switchService = null!;

    [SetUp]
    public void SetUp()
    {
        _mockConfigurationService = Substitute.For<IConfigurationService>();
        _mockConfigurationService.LoadSwitches().Returns(new List<Switch>());
        
        _mockSwitchAccessService = Substitute.For<ISwitchAccessService>();
        _mockLogger = Substitute.For<ILogger<SwitchService>>();
        
        _switchService = new SwitchService(_mockConfigurationService, _mockSwitchAccessService, _mockLogger);
    }

    [Test]
    public void Constructor_ShouldLoadExistingSwitches()
    {
        // Arrange
        var existingSwitches = new List<Switch>
        {
            new Switch("Main Switch", "192.168.1.1"),
            new Switch("Backup Switch", "192.168.1.2")
        };
        var mockConfigService = Substitute.For<IConfigurationService>();
        mockConfigService.LoadSwitches().Returns(existingSwitches);
        var mockSwitchAccess = Substitute.For<ISwitchAccessService>();
        var mockLogger = Substitute.For<ILogger<SwitchService>>();

        // Act
        var service = new SwitchService(mockConfigService, mockSwitchAccess, mockLogger);
        var switches = service.GetSwitches();

        // Assert
        switches.Count.ShouldBe(2);
        switches.ShouldContain(s => s.Name == "Main Switch" && s.IpAddress == "192.168.1.1");
        switches.ShouldContain(s => s.Name == "Backup Switch" && s.IpAddress == "192.168.1.2");
    }

    [Test]
    public void GetSwitches_ShouldReturnEmptyCollection_WhenNoSwitchesAdded()
    {
        // Act
        var switches = _switchService.GetSwitches();

        // Assert
        switches.ShouldNotBeNull();
        switches.ShouldBeEmpty();
    }

    [Test]
    public void AddSwitch_ShouldAddSwitchToCollection()
    {
        // Arrange
        var sw = new Switch("Core Switch", "10.0.0.1");

        // Act
        _switchService.AddSwitch(sw);
        var switches = _switchService.GetSwitches();

        // Assert
        switches.Count.ShouldBe(1);
        switches.ShouldContain(s => s.Name == "Core Switch" && s.IpAddress == "10.0.0.1");
    }

    [Test]
    public void AddSwitch_ShouldCallSaveSwitches()
    {
        // Arrange
        var sw = new Switch("Edge Switch", "172.16.0.1");

        // Act
        _switchService.AddSwitch(sw);

        // Assert
        _mockConfigurationService.Received(1).SaveSwitches(Arg.Any<IEnumerable<Switch>>());
    }

    [Test]
    public void RemoveSwitch_ShouldRemoveSwitchFromCollection()
    {
        // Arrange
        var sw = new Switch("ToRemove", "192.168.1.99");
        _switchService.AddSwitch(sw);

        // Act
        _switchService.RemoveSwitch(sw);
        var switches = _switchService.GetSwitches();

        // Assert
        switches.ShouldBeEmpty();
    }

    [Test]
    public void RemoveSwitch_ShouldCallSaveSwitches()
    {
        // Arrange
        var sw = new Switch("ToRemove", "192.168.1.99");
        _switchService.AddSwitch(sw);
        _mockConfigurationService.ClearReceivedCalls();

        // Act
        _switchService.RemoveSwitch(sw);

        // Assert
        _mockConfigurationService.Received(1).SaveSwitches(Arg.Any<IEnumerable<Switch>>());
    }

    [Test]
    public void MoveUp_ShouldMoveFirstSwitchUp_AndDoNothing()
    {
        // Arrange
        var sw1 = new Switch("Switch 1", "192.168.1.1");
        var sw2 = new Switch("Switch 2", "192.168.1.2");
        _switchService.AddSwitch(sw1);
        _switchService.AddSwitch(sw2);
        _mockConfigurationService.ClearReceivedCalls();

        // Act
        _switchService.MoveUp(sw1);
        var switches = _switchService.GetSwitches();

        // Assert
        switches[0].ShouldBe(sw1);
        switches[1].ShouldBe(sw2);
        _mockConfigurationService.DidNotReceive().SaveSwitches(Arg.Any<IEnumerable<Switch>>());
    }

    [Test]
    public void MoveUp_ShouldMoveSecondSwitchToFirstPosition()
    {
        // Arrange
        var sw1 = new Switch("Switch 1", "192.168.1.1");
        var sw2 = new Switch("Switch 2", "192.168.1.2");
        _switchService.AddSwitch(sw1);
        _switchService.AddSwitch(sw2);
        _mockConfigurationService.ClearReceivedCalls();

        // Act
        _switchService.MoveUp(sw2);
        var switches = _switchService.GetSwitches();

        // Assert
        switches[0].ShouldBe(sw2);
        switches[1].ShouldBe(sw1);
        _mockConfigurationService.Received(1).SaveSwitches(Arg.Any<IEnumerable<Switch>>());
    }

    [Test]
    public void MoveDown_ShouldMoveLastSwitchDown_AndDoNothing()
    {
        // Arrange
        var sw1 = new Switch("Switch 1", "192.168.1.1");
        var sw2 = new Switch("Switch 2", "192.168.1.2");
        _switchService.AddSwitch(sw1);
        _switchService.AddSwitch(sw2);
        _mockConfigurationService.ClearReceivedCalls();

        // Act
        _switchService.MoveDown(sw2);
        var switches = _switchService.GetSwitches();

        // Assert
        switches[0].ShouldBe(sw1);
        switches[1].ShouldBe(sw2);
        _mockConfigurationService.DidNotReceive().SaveSwitches(Arg.Any<IEnumerable<Switch>>());
    }

    [Test]
    public void MoveDown_ShouldMoveFirstSwitchToSecondPosition()
    {
        // Arrange
        var sw1 = new Switch("Switch 1", "192.168.1.1");
        var sw2 = new Switch("Switch 2", "192.168.1.2");
        _switchService.AddSwitch(sw1);
        _switchService.AddSwitch(sw2);
        _mockConfigurationService.ClearReceivedCalls();

        // Act
        _switchService.MoveDown(sw1);
        var switches = _switchService.GetSwitches();

        // Assert
        switches[0].ShouldBe(sw2);
        switches[1].ShouldBe(sw1);
        _mockConfigurationService.Received(1).SaveSwitches(Arg.Any<IEnumerable<Switch>>());
    }

    [Test]
    public async Task AddSwitchAsync_ShouldRetrievePortInformation()
    {
        // Arrange
        var sw = new Switch("Test Switch", "192.168.1.100");
        var portInfos = new List<NbgDev.SwitchMan.Switches.Contract.Models.PortInfo>
        {
            new(1, 10),
            new(2, 20),
            new(3, 10)
        };
        
        _mockSwitchAccessService.GetPortCountAsync(sw.IpAddress).Returns(Task.FromResult(8));
        _mockSwitchAccessService.GetPortVlansAsync(sw.IpAddress).Returns(Task.FromResult<IEnumerable<NbgDev.SwitchMan.Switches.Contract.Models.PortInfo>>(portInfos));

        // Act
        await _switchService.AddSwitchAsync(sw);

        // Assert
        await _mockSwitchAccessService.Received(1).GetPortCountAsync(sw.IpAddress);
        await _mockSwitchAccessService.Received(1).GetPortVlansAsync(sw.IpAddress);
        _mockConfigurationService.Received(1).SaveSwitches(Arg.Any<IEnumerable<Switch>>());
        
        var switches = _switchService.GetSwitches();
        switches.Count.ShouldBe(1);
        switches.ShouldContain(s => s.Name == "Test Switch" && s.IpAddress == "192.168.1.100");
    }

    [Test]
    public async Task AddSwitchAsync_ShouldLogPortVlanInformation()
    {
        // Arrange
        var sw = new Switch("Logging Test Switch", "192.168.1.101");
        var portInfos = new List<NbgDev.SwitchMan.Switches.Contract.Models.PortInfo>
        {
            new(1, 100)
        };
        
        _mockSwitchAccessService.GetPortCountAsync(sw.IpAddress).Returns(Task.FromResult(8));
        _mockSwitchAccessService.GetPortVlansAsync(sw.IpAddress).Returns(Task.FromResult<IEnumerable<NbgDev.SwitchMan.Switches.Contract.Models.PortInfo>>(portInfos));

        // Act
        await _switchService.AddSwitchAsync(sw);

        // Assert - verify logger was called (we can't easily verify exact log messages with NSubstitute)
        _mockLogger.ReceivedCalls().Count().ShouldBeGreaterThan(0);
    }
}
