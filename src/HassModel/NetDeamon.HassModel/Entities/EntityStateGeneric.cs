namespace NetDaemon.HassModel.Entities;

/// <summary>
/// Detailed state information where a strongly typed value is parsed each time it's accessed.
/// This is the default behavior and ideal when the type is easily (or even trivially parsed if
/// it's a string or wrapper of a string).
/// </summary>
/// <typeparam name="TState"></typeparam>
/// <typeparam name="TAttributes"></typeparam>
public sealed record EntityStateGeneric<TAttributes> : IEntityState<TAttributes>
    where TAttributes : class
{
    /// <inheritdoc/>
    public string EntityId { get; }

    /// <inheritdoc/>
    public string? State { get; init; }

    /// <inheritdoc/>
    public JsonElement? AttributesJson { get; init; }

    /// <inheritdoc/>
    public DateTime? LastChanged { get; init; }

    /// <inheritdoc/>
    public DateTime? LastUpdated { get; init; }

    /// <inheritdoc/>
    public Context? Context { get; init; }
    private readonly Lazy<TAttributes?> _attributesLazy;

    internal EntityStateGeneric(string entityId, IEntityStateMapper<TAttributes> mapper)
    {
        EntityId = entityId;
        _attributesLazy = new (() => mapper.ParseAttributes(AttributesJson));
    }

    /// <inheritdoc/>
    public TAttributes? Attributes => _attributesLazy.Value;
}