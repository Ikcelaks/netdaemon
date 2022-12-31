namespace NetDaemon.HassModel.Entities.Core;

public interface ILightService {}

public interface IOnOffState {}

public interface INumericState {}

public interface IDateTimeState {}

public class BaseAttributes : Dictionary<string, object> {}

public class NumericBaseAttributes : Dictionary<string, object>, INumericState {}

public class DateTimeBaseAttributes : Dictionary<string, object>, IDateTimeState {}

public static class StateExtensions
{
    public static bool IsOn<TAttributes>(this IEntity<TAttributes> entity)
        where TAttributes : class, IOnOffState
        => entity.State is not null && entity.State == "on";
    public static bool IsOn<TAttributes>(this IEntityState<TAttributes> entityState)
        where TAttributes : class, IOnOffState
        => entityState.State is not null && entityState.State == "on";

    public static bool IsOff<TAttributes>(this IEntity<TAttributes> entity)
        where TAttributes : class, IOnOffState
        => entity.State is not null && entity.State == "off";
    public static bool IsOff<TAttributes>(this IEntityState<TAttributes> entityState)
        where TAttributes : class, IOnOffState
        => entityState.State is not null && entityState.State == "off";
    
    public static double? NumericState<TAttributes>(this IEntity<TAttributes> entity)
        where TAttributes : class, INumericState
        => FormatHelpers.ParseAsDouble(entity.State);
    public static double? NumericState<TAttributes>(this IEntityState<TAttributes> entityState)
        where TAttributes : class, INumericState
        => FormatHelpers.ParseAsDouble(entityState.State);
    
    public static DateTime? DateTimeState<TAttributes>(this IEntity<TAttributes> entity)
        where TAttributes : class, INumericState
        => entity.State is null ? null : DateTime.Parse(entity.State);
    public static DateTime? DateTimeState<TAttributes>(this IEntityState<TAttributes> entityState)
        where TAttributes : class, INumericState
        => entityState.State is null ? null : DateTime.Parse(entityState.State);
}

public static class LightServiceExtensions
{
    public static void TurnOn<TAttributes>(this IEntity<TAttributes> light, double? brightness = null)
        where TAttributes : class, ILightService
        => light.CallService("turn_on_generic", brightness);
}