namespace NetDaemon.HassModel.Entities;

/// <inheritdoc/>
public sealed record EntityGeneric<TAttributes> : IEntity<TAttributes>
    where TAttributes : class
{
    /// <inheritdoc/>
    public IHaContext HaContext { get; }

    /// <inheritdoc/>
    public string EntityId { get; }

    public IEntityStateMapper<TAttributes> EntityStateMapper { get; }

    internal EntityGeneric(IHaContext haContext, string entityId, IEntityStateMapper<TAttributes> entityStateMapper)
    {
        HaContext = haContext;
        EntityId = entityId;
        EntityStateMapper = entityStateMapper;
    }

    /// <inheritdoc/>
    public IEntityState<TAttributes>? EntityState => EntityStateMapper.MapHassState(HaContext.GetHassState(EntityId));

    /// <inheritdoc/>
    public string? State => EntityState?.State;

    /// <inheritdoc/>
    public TAttributes? Attributes => EntityState is null ? EntityStateMapper.ParseAttributes(null) : EntityState.Attributes;

    /// <inheritdoc/>
    public IObservable<IStateChange<TAttributes>> StateAllChanges() =>
        HaContext.HassStateAllChanges().Select(e => EntityStateMapper.MapHassStateChange(HaContext, e)).Where(e => e.Entity.EntityId == EntityId);

    /// <inheritdoc/>
    public IObservable<IStateChange<TAttributes>> StateChanges() =>
        StateAllChanges().Where(c => c.New?.State != c.Old?.State);

    /// <inheritdoc/>
    public void CallService(string service, object? data = null)
    {
        ArgumentNullException.ThrowIfNull(service, nameof(service));

        var (serviceDomain, serviceName) = service.SplitAtDot();

        serviceDomain ??= EntityId.SplitAtDot().Left ?? throw new InvalidOperationException("EntityId must be formatted 'domain.name'");

        HaContext.CallService(serviceDomain, serviceName, ServiceTarget.FromEntity(EntityId), data);
    }
}