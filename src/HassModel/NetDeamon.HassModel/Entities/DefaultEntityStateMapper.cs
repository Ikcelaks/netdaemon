using NetDaemon.HassModel.Entities.Core;
namespace NetDaemon.HassModel.Entities;

/// <summary>
/// A Collection of useful `EntityStateMapper` instances
/// </summary>
public static class DefaultEntityStateMappers
{
    /// <summary>
    /// Parses the attributes JSON into the class TAttributes
    /// </summary>
    /// <typeparam name="TAttributes"></typeparam>
    public static TAttributes? AttributesAsClass<TAttributes>(JsonElement? a) where TAttributes : class =>
        a?.Deserialize<TAttributes>() ?? default;

    /// <summary>
    /// Matches the types of the original `Entity` class
    /// </summary>
    /// <returns></returns>
    public static IEntityStateMapper<BaseAttributes> Base =>
        new EntityStateMapper<BaseAttributes>(AttributesAsClass<BaseAttributes>);

    /// <summary>
    /// Matches the types of the Original `Entity&lt;TAttributes&gt;` class
    /// </summary>
    /// <typeparam name="TAttributes"></typeparam>
    public static IEntityStateMapper<TAttributes> TypedAttributes<TAttributes>() where TAttributes : class =>
        new EntityStateMapper<TAttributes>(AttributesAsClass<TAttributes>);

    /// <summary>
    /// Matches the types of the original NumericEntity class
    /// </summary>
    /// <returns></returns>
    public static IEntityStateMapper<NumericBaseAttributes> NumericBase =>
        new EntityStateMapper<NumericBaseAttributes>(AttributesAsClass<NumericBaseAttributes>);

    /// <summary>
    /// Parse the state as a DateTime
    /// </summary>
    /// <returns></returns>
    public static IEntityStateMapper<DateTimeBaseAttributes> DateTimeBase =>
        new EntityStateMapper<DateTimeBaseAttributes>(AttributesAsClass<DateTimeBaseAttributes>);
}