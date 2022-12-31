namespace NetDaemon.HassModel.Entities;

/// <inheritdoc/>
public sealed class EntityStateMapper<TAttributes> : IEntityStateMapper<TAttributes>
    where TAttributes : class
{
    private readonly Func<JsonElement?, TAttributes?> _attributesParser;

    /// <summary>
    /// Create an EnitityStateMapper by providing parsinge functions for the state and attributes properties
    /// </summary>
    /// <param name="stateParser"></param>
    /// <param name="attributesParser"></param>
    public EntityStateMapper(Func<JsonElement?, TAttributes?> attributesParser)
    {
        _attributesParser = attributesParser;
    }

    /// <inheritdoc/>
    public TAttributes? ParseAttributes(JsonElement? rawAttributes) => _attributesParser(rawAttributes);

    /// <inheritdoc/>
    public IEntityState<TAttributes>? MapHassState(HassState? hassState)
    {
        if (hassState == null) return null;

        return new EntityStateGeneric<TAttributes>(hassState.EntityId, this)
        {
            State = hassState.State,
            AttributesJson = hassState.AttributesJson,
            LastChanged = hassState.LastChanged,
            LastUpdated = hassState.LastUpdated,
            Context = hassState.Context == null
                ? null
                : new Context
                {
                    Id = hassState.Context.Id,
                    UserId = hassState.Context.UserId,
                    ParentId = hassState.Context.UserId
                }
        };
    }
    
    /// <inheritdoc/>
    public IStateChange<TAttributes> MapHassStateChange(IHaContext haContext, HassStateChangedEventData hassStateChange)
        => new StateChangeGeneric<TAttributes>
        (
            Entity(haContext, hassStateChange.EntityId),
            MapHassState(hassStateChange.OldState),
            MapHassState(hassStateChange.NewState)
        );

    // /// <inheritdoc/>
    // public IStateChange<TAttributes> Map(IStateChange stateChange) => new StateChangeGeneric<TAttributes>(stateChange, this);

    /// <inheritdoc/>
    public IEntity<TAttributes> Map<TAttributesOld>(IEntity<TAttributesOld> entity)
        where TAttributesOld : class
        => new EntityGeneric<TAttributes>(entity.HaContext, entity.EntityId, this);

    /// <inheritdoc/>
    public IEntity<TAttributes> Entity(IHaContext haContext, string entityId) => new EntityGeneric<TAttributes>(haContext, entityId, this);
    
    /// <inheritdoc/>
    public IEntityStateMapper<TAttributesNew> WithAttributesAs<TAttributesNew>(Func<JsonElement?, TAttributesNew?> customAttributesParser)
        where TAttributesNew : class
        => new EntityStateMapper<TAttributesNew>(customAttributesParser);

    /// <inheritdoc/>
    public IEntityStateMapper<TAttributesNew> WithAttributesAs<TAttributesNew>()
        where TAttributesNew : class
        => WithAttributesAs<TAttributesNew>(DefaultEntityStateMappers.AttributesAsClass<TAttributesNew>);
}