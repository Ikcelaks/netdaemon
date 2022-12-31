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
using NetDaemon.HassModel.Tests.TestHelpers;
using NetDaemon.HassModel.Tests.TestHelpers.HassClient;

namespace NetDaemon.HassModel.Tests.Entities;

public class IEntityTest
{
    [Fact]
    public void ShouldGetStateFromHaContext()
    {
        // ARRANGE
        var haContextMock = new Mock<IHaContext>();
        var entityId = "domain.test_entity";
        var mapper = DefaultEntityStateMappers.TypedAttributes<AttributesWithName>();

        var hassState = new HassState
            {
                EntityId = entityId,
                State = "state initial",
                AttributesJson = new { name = "name initial" }.AsJsonElement()
            };

        haContextMock.Setup(t => t.GetHassState(It.IsAny<string>())).Returns(hassState);
        
        // ACT
        var target = mapper.Entity(haContextMock.Object, entityId);
       
        // ASSERT
        target.State.Should().Be("state initial");
        target.Attributes!.Name.Should().Be("name initial");

        target.EntityState!.State.Should().Be("state initial");
        target.EntityState!.Attributes!.Name.Should().Be("name initial");

        // Act2: update the state
        var newHassState = new HassState
            {
                EntityId = entityId,
                State = "NewState",
                AttributesJson = new { name = "SecondName" }.AsJsonElement()
            };

        haContextMock.Setup(t => t.GetHassState(entityId)).Returns(newHassState);

        // Assert
        target.State.Should().Be("NewState");
        target.Attributes!.Name.Should().Be("SecondName");

        target.EntityState!.State.Should().Be("NewState");
        target.EntityState!.Attributes!.Name.Should().Be("SecondName");
    }

    [Fact]
    public void ShouldShowStateChangesFromContext()
    {
        var haContextMock = new Mock<IHaContext>();
        var hassStateChangesSubject = new Subject<HassStateChangedEventData>();
        var entityId = "domain.test_entity";
        var mapper = DefaultEntityStateMappers.TypedAttributes<AttributesWithName>();

        var hassState = new HassState
            {
                EntityId = entityId,
                State = "state initial",
                AttributesJson = new { name = "name initial" }.AsJsonElement()
            };

        haContextMock.Setup(t => t.GetHassState(It.IsAny<string>())).Returns(hassState);

        haContextMock.Setup(h => h.HassStateAllChanges()).Returns(hassStateChangesSubject);

        var target = mapper.Entity(haContextMock.Object, entityId);
        var stateChangeObserverMock = new Mock<IObserver<IStateChange<AttributesWithName>>>();
        var stateAllChangeObserverMock = new Mock<IObserver<IStateChange<AttributesWithName>>>();

        target.StateAllChanges().Subscribe(stateAllChangeObserverMock.Object);
        target.StateChanges().Subscribe(stateChangeObserverMock.Object);

        hassStateChangesSubject.OnNext(
            new HassStateChangedEventData
            {
                EntityId = entityId,
                NewState = new HassState
                    {
                        EntityId = entityId,
                        State = "old"
                    },
                OldState = new HassState
                    {
                        EntityId = entityId,
                        State = "new"
                    }
            });

        hassStateChangesSubject.OnNext(
            new HassStateChangedEventData
            {
                EntityId = entityId,
                NewState = new HassState
                    {
                        EntityId = entityId,
                        State = "same"
                    },
                OldState = new HassState
                    {
                        EntityId = entityId,
                        State = "same"
                    }
            });

        stateChangeObserverMock.Verify(o => o.OnNext(It.IsAny<IStateChange<AttributesWithName>>()), Times.Once);
        stateAllChangeObserverMock.Verify(o => o.OnNext(It.IsAny<IStateChange<AttributesWithName>>()), Times.Exactly(2));
    }

    [Fact]
    public void ShouldCallServiceOnContext()
    {
        var entityId = "domain.testEntity";
        var haContextMock = new Mock<IHaContext>();
        var mapper = DefaultEntityStateMappers.TypedAttributes<AttributesWithName>();

        var entity = mapper.Entity(haContextMock.Object, entityId);
        var data = "payload";

        entity.CallService("service", data);

        haContextMock.Verify(h => h.CallService("domain", "service", It.Is<ServiceTarget>(t => t.EntityIds!.Single() == entity.EntityId), data), Times.Once);
    }

    [Fact]
    public void AsNumericThanWithAttributesAs()
    {
        var entityId = "sensor.temperature";
        var haContextMock = new HaContextMock();
        haContextMock.Setup(m => m.GetHassState(entityId)).Returns(
            new HassState {
                EntityId = entityId,
                State = "12.3",
                AttributesJson = new { set_point = 21.5, units = "Celcius"}.AsJsonElement()
            }
        );
        var hassStateChangesSubject = new Subject<HassStateChangedEventData>();
        haContextMock.Setup(h => h.HassStateAllChanges()).Returns(hassStateChangesSubject);
        
        var baseMapper = DefaultEntityStateMappers.Base;

        var entity = baseMapper.Entity(haContextMock.Object, entityId);

        // Assert
        entity.State.Should().Be("12.3");
        // entity.State.NumericState isn't allowed by the typechecker

        // Act: WithNewAttributesAs
        var numericWithAttributesMapper = DefaultEntityStateMappers.TypedAttributes<TestSensorAttributes>();
        var withAttributes = numericWithAttributesMapper.Map(entity);
        withAttributes.StateAllChanges().Where(e => e.New?.NumericState() > 1.2 && e.Entity != null);
        var stateChangeObserverMock = new Mock<IObserver<IStateChange<TestSensorAttributes>>>();
        var conditionalStateChangeObserverMock = new Mock<IObserver<IStateChange<TestSensorAttributes>>>();

        // Assert
        withAttributes.NumericState()!.Value!.Should().Be(12.3d);
        withAttributes.EntityState!.NumericState()!.Value!.Should().Be(12.3d);

        withAttributes.Attributes!.Units.Should().Be("Celcius");
        withAttributes.Attributes!.SetPoint.Should().Be(21.5);
        withAttributes.EntityState!.Attributes!.Units.Should().Be("Celcius");
        withAttributes.EntityState!.Attributes!.SetPoint.Should().Be(21.5);
        withAttributes.StateAllChanges().Subscribe(stateChangeObserverMock.Object);
        withAttributes.StateAllChanges().Where(e => e.New?.NumericState() > 7.2 && e.Entity != null).Subscribe(conditionalStateChangeObserverMock.Object);
        hassStateChangesSubject.OnNext(
            new HassStateChangedEventData
            {
                EntityId = entityId,
                NewState = new HassState
                    {
                        EntityId = entityId,
                        State = "1.5"
                    },
                OldState = new HassState
                    {
                        EntityId = entityId,
                        State = "21.5"
                    }
            });
        stateChangeObserverMock.Verify(o => o.OnNext(It.IsAny<IStateChange<TestSensorAttributes>>()), Times.Once);
        conditionalStateChangeObserverMock.Verify(o => o.OnNext(It.IsAny<IStateChange<TestSensorAttributes>>()), Times.Never);
    }

    [Fact]
    public void NumericShouldShowStateChangesFromContext()
    {
        var haContextMock = new HaContextMock();
        var entityId = "domain.testEntity";
        var mapper = DefaultEntityStateMappers.NumericBase;

        var target = mapper.Entity(haContextMock.Object, entityId);

        haContextMock.Setup(m => m.GetHassState(entityId)).Returns(new HassState() { EntityId = entityId, State = "3.14" });

        var hassStateAllChangesSubject = new Subject<HassStateChangedEventData>();
        haContextMock.Setup(h => h.HassStateAllChanges()).Returns(hassStateAllChangesSubject);

        var stateAllChangeObserverMock = target.StateAllChanges().SubscribeMock();
        var stateChangeObserverMock = target.StateChanges().SubscribeMock();

        hassStateAllChangesSubject.OnNext(new HassStateChangedEventData
                {
                    EntityId = entityId,
                    OldState = new HassState { EntityId = entityId, State = "1" },
                    NewState = new HassState { EntityId = entityId, State = "1" }
                });

        hassStateAllChangesSubject.OnNext(new HassStateChangedEventData
                {
                    EntityId = entityId,
                    OldState = new HassState { EntityId = entityId, State = "1" },
                    NewState = new HassState { EntityId = entityId, State = "2" }
                });

        // Assert
        stateChangeObserverMock.Verify(o => o.OnNext(It.Is<IStateChange<NumericBaseAttributes>>
        (e => e.Entity.NumericState().Equals(3.14) &&
              e.Old!.NumericState().Equals(1.0) &&
              e.New!.NumericState().Equals(2.0))), Times.Once);
        stateChangeObserverMock.VerifyNoOtherCalls();

        stateAllChangeObserverMock.Verify(o => o.OnNext(It.Is<IStateChange<NumericBaseAttributes>>
        (e => e.Entity.NumericState().Equals(3.14) &&
              e.Old!.NumericState().Equals(1.0) &&
              e.New!.NumericState().Equals(2.0))), Times.Once);

        stateAllChangeObserverMock.Verify(o => o.OnNext(It.Is<IStateChange<NumericBaseAttributes>>
        (e => e.Entity.NumericState().Equals(3.14) &&
              e.Old!.NumericState().Equals(1.0) &&
              e.New!.NumericState().Equals(1.0))), Times.Once);
        stateAllChangeObserverMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void StatePropertyShouldBeCultureUnaware()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("De-de");

        var entityId = "sensor.temperature";

        var haContextMock = new HaContextMock();
        haContextMock.Setup(m => m.GetHassState(entityId)).Returns(new HassState { EntityId = entityId, State = "12.5" });

        var numericEntity = DefaultEntityStateMappers.NumericBase.Entity(haContextMock.Object, entityId);

        numericEntity.NumericState().Should().Be(12.5);
        numericEntity.EntityState!.NumericState().Should().Be(12.5);

        var withAttributesAs = numericEntity.MappedBy(DefaultEntityStateMappers.TypedAttributes<TestSensorAttributes>());
        withAttributesAs.NumericState().Should().Be(12.5);
        withAttributesAs.EntityState!.NumericState().Should().Be(12.5);
    }

    // Attribute records
    public record AttributesWithName([property:JsonPropertyName("name")]string? Name);
    public record TestSensorAttributes([property:JsonPropertyName("set_point")]double? SetPoint, [property:JsonPropertyName("units")]string? Units)
        : INumericState;
}