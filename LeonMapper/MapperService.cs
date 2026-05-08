using System.Collections.Concurrent;
using System.Linq.Expressions;
using LeonMapper.Utils;

namespace LeonMapper;

/// <summary>
/// 非泛型映射器，实现 <see cref="IMapper"/> 接口。
/// 用于运行时类型解析的场景（如 DI 容器注入），内部通过 CachedMapperFactory 动态获取映射器。
/// </summary>
public class MapperService : IMapper
{
    // 缓存强类型的包装委托，避免 DynamicInvoke 的性能损失
    private static readonly ConcurrentDictionary<(Type, Type), Func<object?, object?>> _invokeCache = new();

    static MapperService()
    {
        Compilers.CachedMapperFactory.OnCacheCleared += () => _invokeCache.Clear();
    }

    /// <summary>
    /// 执行映射（运行时类型解析）
    /// </summary>
    public object? Map(object? source, Type sourceType, Type targetType)
    {
        ArgumentNullException.ThrowIfNull(sourceType);
        ArgumentNullException.ThrowIfNull(targetType);

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
            var invoker = _invokeCache.GetOrAdd((sourceType, targetType), CreateInvoker);
            return invoker(source);
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

        if (source is TTarget target)
        {
            return target;
        }

        return Map(source, source.GetType(), typeof(TTarget)) as TTarget;
    }

    /// <summary>
    /// 创建强类型的映射调用委托，绕过 DynamicInvoke 的性能开销
    /// </summary>
    private static Func<object?, object?> CreateInvoker((Type SourceType, Type TargetType) key)
    {
        var mapFunc = Compilers.CachedMapperFactory.GetOrCreateMapFunc(key.SourceType, key.TargetType);
        var invokeMethod = mapFunc.GetType().GetMethod("Invoke")
            ?? throw new InvalidOperationException("映射委托缺少 Invoke 方法");

        // 构建: (object source) => (object)((Func<TSource,TTarget>)delegate).Invoke((TSource)source)
        var param = Expression.Parameter(typeof(object), "source");
        var sourceExpr = Expression.Convert(param, key.SourceType);
        var delegateExpr = Expression.Constant(mapFunc);
        var callExpr = Expression.Call(delegateExpr, invokeMethod, sourceExpr);
        var resultExpr = Expression.Convert(callExpr, typeof(object));
        return Expression.Lambda<Func<object?, object?>>(resultExpr, param).Compile();
    }
}
