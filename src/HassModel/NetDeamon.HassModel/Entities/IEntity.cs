namespace NetDaemon.HassModel.Entities;

/// <summary>Represents a Home Assistant entity with its state, changes and services</summary>
public interface IEntity<TAttributes>
    where TAttributes : class
{
    /// <summary>
    /// The IHAContext
    /// </summary>
    IHaContext HaContext { get; }

    /// <summary>
    /// Entity id being handled by this entity
    /// </summary>
    string EntityId { get; }

    IEntityStateMapper<TAttributes> EntityStateMapper { get; }

    /// <summary>
    /// Calls a service using this entity as the target
    /// </summary>
    /// <param name="service">Name of the service to call. If the Domain of the service is the same as the domain of the Entity it can be omitted</param>
    /// <param name="data">Data to provide</param>
    void CallService(string service, object? data = null);

    /// <summary>
    /// The full state of this Entity
    /// </summary>
    IEntityState<TAttributes>? EntityState { get; }
    
    /// <summary>The current state of this Entity</summary>
    string? State { get; }

    /// <summary>
    /// The current Attributes of this Entity
    /// </summary>
    TAttributes? Attributes { get; }
    
    /// <summary>
    /// Observable, All state changes including attributes
    /// </summary>
    IObservable<IStateChange<TAttributes>> StateAllChanges();

    /// <summary>
    /// Observable, All state changes. New.State!=Old.State
    /// </summary>
    IObservable<IStateChange<TAttributes>> StateChanges();
}