using LeonMapper.Compilers;
using LeonMapper.Utils;

namespace LeonMapper;

/// <summary>
/// 非泛型映射器，实现 <see cref="IMapper"/> 接口。
/// 用于运行时类型解析的场景（如 DI 容器注入），内部通过 CachedMapperFactory 动态获取映射器。
/// </summary>
public class MapperService : IMapper
{
    /// <summary>
    /// 执行映射（运行时类型解析）
    /// </summary>
    public object? Map(object? source, Type sourceType, Type targetType)
    {
        if (source == null)
        {
            return null;
        }

        if (!MappingDepthTracker.TryIncrement())
        {
            return null;
        }

        try
        {
            var mapFunc = Compilers.CachedMapperFactory.GetOrCreateMapFunc(sourceType, targetType);
            return mapFunc.DynamicInvoke(source);
        }
        finally
        {
            MappingDepthTracker.Decrement();
        }
    }

    /// <summary>
    /// 执行映射（目标类型由泛型参数指定）
    /// </summary>
    public TTarget? Map<TTarget>(object? source) where TTarget : class
    {
        if (source == null)
        {
            return null;
        }

        return Map(source, source.GetType(), typeof(TTarget)) as TTarget;
    }
}
