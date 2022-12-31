namespace NetDaemon.HassModel.Entities;

/// <summary>
/// Extension methods for IEntities
/// </summary>
public static class EntityGenericExtensions
{
    /// <summary>
    /// Get a strongly typed IEntity&lt;TAttributes&gt; from any IEntity
    /// This will work if and only if the underlying JSON is compatible with your types.
    /// </summary>
    /// <param name="entity">The source entity</param>
    /// <param name="mapper">The type mapper from raw values to the target types</param>
    /// <typeparam name="TStateOld"></typeparam>
    /// <typeparam name="TAttributesOld"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TAttributes"></typeparam>
    /// <returns></returns>
    public static IEntity<TAttributes> MappedBy<TAttributesOld, TAttributes>(this IEntity<TAttributesOld> entity, IEntityStateMapper<TAttributes> mapper)
        where TAttributesOld : class
        where TAttributes : class
        => mapper.Map(entity);

    /// <summary>
    /// Get a new IEntity with a new attributes class mapping
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="newAttributesParser"></param>
    /// <typeparam name="TAttributesNew"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TAttributes"></typeparam>
    /// <returns></returns>
    public static IEntity<TAttributesNew> WithAttributesAs<TAttributesNew, TAttributes>(this IEntity<TAttributes> entity, Func<JsonElement?, TAttributesNew> newAttributesParser)
        where TAttributesNew : class
        where TAttributes : class
        => entity.EntityStateMapper.WithAttributesAs(newAttributesParser).Map(entity);
}