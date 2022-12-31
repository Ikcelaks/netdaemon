using System.Collections.Generic;
using System.Text.Json.Serialization;
using NetDaemon.HassModel.Entities;
using NetDaemon.HassModel.Entities.Core;
using NetDaemon.HassModel.Internal;

namespace NetDaemon.HassModel.Tests.Entities;

public interface IZwaveServiceTarget {}

public interface ILockServiceTarget {}

public record LightAttributes([property:JsonPropertyName("brightness")]double? Brightness) : ILightService, IOnOffState;

public record LockAttributes([property:JsonPropertyName("whatever")]string? Whatever) : ILockServiceTarget;

public record ZwaveLockAttributes([property:JsonPropertyName("jammed")]string? Whatever) : ILockServiceTarget, IZwaveServiceTarget;

public static class TestServiceTargetExtensions
{
    public static void LockLock<TAttributes>(this IEntity<TAttributes> entity)
        where TAttributes : class, ILockServiceTarget
        => entity.CallService("lock_lock", "whatever");
        
    public static void ZwaveSetUserCode<TAttributes>(this IEntity<TAttributes> entity)
        where TAttributes : class, ILockServiceTarget, IZwaveServiceTarget
        => entity.CallService("zwavejs_setusercode", "whatever");
}