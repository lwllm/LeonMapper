using System.Collections.Concurrent;
using System.Reflection;
using LeonMapper.Plan;
using LeonMapper.Plan.Builder;

namespace LeonMapper.Compilers;

/// <summary>
/// 缓存的映射器工厂：为复杂类型映射创建并缓存 Mapper 实例和映射委托
/// </summary>
internal static class CachedMapperFactory
{
    private static readonly ConcurrentDictionary<(Type, Type), object> _mapperCache = new();
    private static readonly ConcurrentDictionary<(Type, Type), Delegate> _mapFuncCache = new();

    /// <summary>
    /// 获取或创建映射器实例（线程安全）
    /// </summary>
    /// <param name="sourceType">源类型</param>
    /// <param name="targetType">目标类型</param>
    /// <returns>缓存或新建的 Mapper 实例</returns>
    public static object GetOrCreateMapper(Type sourceType, Type targetType)
    {
        var key = (sourceType, targetType);
        return _mapperCache.GetOrAdd(key, k => CreateMapper(k.Item1, k.Item2));
    }

    /// <summary>
    /// 获取或创建映射委托（用于 ExpressionCompiler 的表达式树，线程安全）
    /// </summary>
    /// <param name="sourceType">源类型</param>
    /// <param name="targetType">目标类型</param>
    /// <returns>缓存或新建的映射委托</returns>
    public static Delegate GetOrCreateMapFunc(Type sourceType, Type targetType)
    {
        var key = (sourceType, targetType);
        return _mapFuncCache.GetOrAdd(key, k =>
        {
            var mapper = GetOrCreateMapper(k.Item1, k.Item2);
            var mapToMethod = mapper.GetType().GetMethod("MapTo")
                ?? throw new InvalidOperationException($"Mapper<{k.Item1.Name}, {k.Item2.Name}> 缺少 MapTo 方法");
            var funcType = typeof(Func<,>).MakeGenericType(k.Item1, k.Item2);
            return Delegate.CreateDelegate(funcType, mapper, mapToMethod);
        });
    }

    /// <summary>
    /// 通过反射创建指定类型的 Mapper 实例
    /// </summary>
    private static object CreateMapper(Type sourceType, Type targetType)
    {
        var plan = MappingPlanBuilder.Build(sourceType, targetType);
        var mapperType = typeof(Mapper<,>).MakeGenericType(sourceType, targetType);
        var constructor = mapperType.GetConstructor(new[] { typeof(TypeMappingPlan) });
        if (constructor != null)
        {
            return constructor.Invoke(new object[] { plan });
        }

        throw new InvalidOperationException(
            $"无法创建 {sourceType.Name} -> {targetType.Name} 的映射器，缺少接受 TypeMappingPlan 的构造函数");
    }
}
