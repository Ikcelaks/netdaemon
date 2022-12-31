
using System.Collections.Generic;
using System.Globalization;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using NetDaemon.Client.HomeAssistant.Model;
using NetDaemon.HassModel.Entities;
using NetDaemon.HassModel.Entities.Core;
using NetDaemon.HassModel.Internal;
using NetDaemon.HassModel.Tests.TestHelpers;
using NetDaemon.HassModel.Tests.TestHelpers.HassClient;

namespace NetDaemon.HassModel.Tests.Entities;

// Sample code to create with code generator
//
// This was put together in a rush to demonstrate the technique. I didn't reference the format required for the data parameter in CallService,
// so this is almost certainly an invalid call, but it should demonstrate the technique. I'll clean up and broaden all these test in the coming
// days as my time frees up.
//
// The basic idea for the code generator is to create extension methods on Attributes type. I believe that this is very similar to the current behavior.

public class IEntityDomainServiceExtensionsTest
{
    public IEntityStateMapper<LightAttributes> LightMapper = DefaultEntityStateMappers.TypedAttributes<LightAttributes>();

    public IEntityStateMapper<ZwaveLockAttributes> ZwaveLockMapper = DefaultEntityStateMappers.TypedAttributes<ZwaveLockAttributes>();
    
    public IEntityStateMapper<LockAttributes> LockMapper = DefaultEntityStateMappers.TypedAttributes<LockAttributes>();
    
    [Fact]
    public void CanCallDomainServiceOnStateChange()
    {
        // Arrange
        var entityId = "light.one";
        var haContextMock = new Mock<IHaContext>();
        var hassStateChangesSubject = new Subject<HassStateChangedEventData>();
        haContextMock.Setup(h => h.HassStateAllChanges()).Returns(hassStateChangesSubject);

        // Demonstrate usage of global using
        TestLightEntity target = LightMapper.Entity(haContextMock.Object, entityId);
        var brightness = 50d;

        // Act
        target.StateChanges().Subscribe(e => e.Entity.TurnOn(brightness: brightness)); // Uses the generic extension method, because of the parameter set

        hassStateChangesSubject.OnNext(
            new HassStateChangedEventData
            {
                EntityId = entityId,
                NewState = new HassState
                    {
                        EntityId = entityId,
                        State = "off"
                    },
                OldState = new HassState
                    {
                        EntityId = entityId,
                        State = "on"
                    }
            });

        // Assert
        haContextMock.Verify(h => h.CallService("light", "turn_on_generic", It.Is<ServiceTarget>(t => t.EntityIds!.Single() == target.EntityId), brightness), Times.Once);
    }
    
    [Fact]
    public void CanSupportMultipleServiceDomainsOnSameEntity()
    {
        // Arrange
        var entityId1 = "lock.zwavelock";
        var entityId2 = "lock.lock";
        var haContextMock = new Mock<IHaContext>();
        var hassStateChangesSubject = new Subject<HassStateChangedEventData>();
        haContextMock.Setup(h => h.HassStateAllChanges()).Returns(hassStateChangesSubject);

        // Demonstrate usage of global using
        var target1 = ZwaveLockMapper.Entity(haContextMock.Object, entityId1);
        var target2 = LockMapper.Entity(haContextMock.Object, entityId2);

        // Act
        target1.LockLock();
        target1.ZwaveSetUserCode();
        target2.LockLock();
        // Correctly cannot call ZwaveJsSetUserCode on non-Zwave lock
        // target2.ZwaveSetUserCode();

        // Assert
        haContextMock.Verify(h => h.CallService("lock", "lock_lock", It.Is<ServiceTarget>(t => t.EntityIds!.Single() == target1.EntityId), "whatever"), Times.Once);
        haContextMock.Verify(h => h.CallService("zwavejs", "zwavejs_setusercode", It.Is<ServiceTarget>(t => t.EntityIds!.Single() == target1.EntityId), "whatever"), Times.Once);
        haContextMock.Verify(h => h.CallService("lock", "lock_lock", It.Is<ServiceTarget>(t => t.EntityIds!.Single() == target2.EntityId), "whatever"), Times.Once);
    }
}