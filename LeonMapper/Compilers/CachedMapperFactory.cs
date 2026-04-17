using System.Reflection;
using LeonMapper.Plan;
using LeonMapper.Plan.Builder;

namespace LeonMapper.Compilers;

/// <summary>
/// 缓存的映射器工厂：为复杂类型映射创建并缓存 Mapper 实例和映射委托
/// </summary>
internal static class CachedMapperFactory
{
    private static readonly Dictionary<(Type, Type), object> _mapperCache = new();
    private static readonly Dictionary<(Type, Type), Delegate> _mapFuncCache = new();
    private static readonly object _lock = new();

    /// <summary>
    /// 获取或创建映射器实例
    /// </summary>
    public static object GetOrCreateMapper(Type sourceType, Type targetType)
    {
        var key = (sourceType, targetType);
        lock (_lock)
        {
            if (_mapperCache.TryGetValue(key, out var cached))
                return cached;
        }

        var mapper = CreateMapper(sourceType, targetType);

        lock (_lock)
        {
            _mapperCache[key] = mapper;
        }

        return mapper;
    }

    /// <summary>
    /// 获取或创建映射委托（用于 ExpressionCompiler 的表达式树）
    /// </summary>
    public static Delegate GetOrCreateMapFunc(Type sourceType, Type targetType)
    {
        var key = (sourceType, targetType);
        lock (_lock)
        {
            if (_mapFuncCache.TryGetValue(key, out var cached))
                return cached;
        }

        var mapper = GetOrCreateMapper(sourceType, targetType);
        var mapToMethod = mapper.GetType().GetMethod("MapTo")!;

        // 创建委托：Func<TSource, TTarget>
        var funcType = typeof(Func<,>).MakeGenericType(sourceType, targetType);
        var mapFunc = Delegate.CreateDelegate(funcType, mapper, mapToMethod);

        lock (_lock)
        {
            _mapFuncCache[key] = mapFunc;
        }

        return mapFunc;
    }

    private static object CreateMapper(Type sourceType, Type targetType)
    {
        var plan = MappingPlanBuilder.Build(sourceType, targetType);
        var mapperType = typeof(Mapper<,>).MakeGenericType(sourceType, targetType);
        var constructor = mapperType.GetConstructor(new[] { typeof(TypeMappingPlan) });
        if (constructor != null)
            return constructor.Invoke(new object[] { plan });

        throw new InvalidOperationException(
            $"无法创建 {sourceType.Name} -> {targetType.Name} 的映射器，缺少接受 TypeMappingPlan 的构造函数");
    }
}
