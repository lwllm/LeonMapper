namespace LeonMapper;

/// <summary>
/// 非泛型映射器接口，适用于 DI 容器注入和运行时类型映射。
/// 内部通过 CachedMapperFactory 动态解析映射器，性能略低于直接使用 <see cref="Mapper{TSource, TDestination}"/>。
/// </summary>
public interface IMapper
{
    /// <summary>
    /// 执行映射（运行时类型解析）
    /// </summary>
    /// <param name="source">源对象</param>
    /// <param name="sourceType">源类型</param>
    /// <param name="targetType">目标类型</param>
    /// <returns>映射后的目标对象，映射失败时返回 null</returns>
    object? Map(object? source, Type sourceType, Type targetType);

    /// <summary>
    /// 执行映射（目标类型由泛型参数指定）
    /// </summary>
    TTarget? Map<TTarget>(object? source) where TTarget : class;
}
