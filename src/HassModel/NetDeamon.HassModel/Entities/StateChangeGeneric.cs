namespace NetDaemon.HassModel.Entities;

/// <inheritdoc/>
public sealed record StateChangeGeneric<TAttributes>
(
    IEntity<TAttributes> Entity,
    IEntityState<TAttributes>? Old,
    IEntityState<TAttributes>? New
) : IStateChange<TAttributes>
    where TAttributes : class;