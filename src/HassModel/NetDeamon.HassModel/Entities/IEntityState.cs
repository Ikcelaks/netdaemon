namespace NetDaemon.HassModel.Entities;

/// <summary>
/// Generic EntityState with specific types of State and Attributes
/// </summary>
/// <typeparam name="TState">Type of the State property</typeparam>
/// <typeparam name="TAttributes">Type of the Attributes property</typeparam>
public interface IEntityState<TAttributes>
    where TAttributes : class
{
    /// <summary>
    /// Unique id of the entity
    /// </summary>
    /// <value></value>
    string EntityId { get; }

    /// <summary>
    /// The raw state of the entity as the original nullable string
    /// </summary>
    /// <value></value>
    string? State { get; }

    /// <summary>
    /// The raw attributes as the original JSON
    /// </summary>
    /// <value></value>
    JsonElement? AttributesJson { get; }

    /// <summary>
    /// When the state or attributes last changed
    /// </summary>
    /// <value></value>
    DateTime? LastChanged { get; }

    /// <summary>
    /// When the state or attributes were last update (even if they didn't change)
    /// </summary>
    /// <value></value>
    DateTime? LastUpdated { get; }

    /// <summary>
    /// Home Assistant Context
    /// </summary>
    /// <value></value>
    Context? Context { get; }

    /// <summary>
    /// The attributes of the entity as the class TAttributes (possibly null)
    /// </summary>
    /// <value></value>
    TAttributes? Attributes { get; }
}