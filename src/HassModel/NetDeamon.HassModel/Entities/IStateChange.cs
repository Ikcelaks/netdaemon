namespace NetDaemon.HassModel.Entities;

/// <summary>
/// Represents a state change event for a strongly typed entity and state
/// </summary>
/// <typeparam name="TState"></typeparam>
/// <typeparam name="TAttributes"></typeparam>
public interface IStateChange<TAttributes>
    where TAttributes : class
{
    /// <summary>
    /// The strongly typed entity object
    /// </summary>
    /// <value></value>
    IEntity<TAttributes> Entity { get; }

    /// <summary>
    /// The strongly typed old state
    /// </summary>
    /// <value></value>
    IEntityState<TAttributes>? Old { get; }

    /// <summary>
    /// The strongly typed new state
    /// </summary>
    /// <value></value>
    IEntityState<TAttributes>? New { get; }
}