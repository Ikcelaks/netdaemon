namespace NetDaemon.HassModel.Entities;

/// <summary>
/// Maps the raw state and attributes of an IEntity object to strongly typed objects / values
/// </summary>
/// <typeparam name="TState">A type that is deserializable from a nullable string to hold the state of the entity</typeparam>
/// <typeparam name="TAttributes">Json deserializable reference type to hold the entities attributes</typeparam>
public interface IEntityStateMapper<TAttributes>
    where TAttributes : class
{
    /// <summary>
    /// Parse a nullable JsonElement into the strongly type attribute class
    /// </summary>
    /// <param name="rawAttributes"></param>
    /// <returns></returns>
    TAttributes? ParseAttributes(JsonElement? rawAttributes);

    /// <summary>
    /// Map a HassState object to IEntityState
    /// </summary>
    /// <param name="hassState"></param>
    /// <returns></returns>
    IEntityState<TAttributes>? MapHassState(HassState? hassState);

    /// <summary>
    /// Map a HassStateChangedEventData object to IStateChange
    /// </summary>
    /// <param name="haContext"></param>
    /// <param name="hassStateChange"></param>
    /// <returns></returns>
    IStateChange<TAttributes> MapHassStateChange(IHaContext haContext, HassStateChangedEventData hassStateChange);

    // /// <summary>
    // /// Map a raw state change object into a strongly typed one
    // /// </summary>
    // /// <param name="stateChange"></param>
    // /// <returns></returns>
    // IStateChange<TState, TAttributes> Map(IStateChange stateChange);

    /// <summary>
    /// Map a raw state change object into a strongly typed one
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    IEntity<TAttributes> Map<TAttributesOld>(IEntity<TAttributesOld> entity)
        where TAttributesOld : class;

    /// <summary>
    /// Create a new Entity instance
    /// </summary>
    /// <param name="haContext"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    IEntity<TAttributes> Entity(IHaContext haContext, string entityId);

    /// <summary>
    /// Create a new IEntityStateMapper that has the same state type and parser
    /// with a new attributes class
    /// </summary>
    /// <typeparam name="TAttributesNew"></typeparam>
    /// <returns></returns>
    IEntityStateMapper<TAttributesNew> WithAttributesAs<TAttributesNew>(Func<JsonElement?, TAttributesNew?> customAttributesParser)
        where TAttributesNew : class;

    /// <summary>
    /// Create a new IEntityStateMapper that has the same state type and parser
    /// with a new attributes class using the default attributes parser
    /// </summary>
    /// <typeparam name="TAttributesNew"></typeparam>
    /// <returns></returns>
    IEntityStateMapper<TAttributesNew> WithAttributesAs<TAttributesNew>()
        where TAttributesNew : class;
}