using System.Collections.Concurrent;
using System.Collections.Generic;
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
    private static readonly ConcurrentDictionary<(Type, Type), object> _emptyMapperCache = new();

    // 追踪当前正在创建的映射器类型对，防止自引用类型在 Mapper 构造阶段无限递归
    private static readonly ThreadLocal<HashSet<(Type, Type)>> _creatingMappers =
        new(() => new HashSet<(Type, Type)>());

    /// <summary>
    /// 获取或创建映射器实例（线程安全）。
    /// 包含自引用类型检测：当映射器正在被创建时递归调用，返回空计划映射器阻止无限递归。
    /// </summary>
    public static object GetOrCreateMapper(Type sourceType, Type targetType)
    {
        var key = (sourceType, targetType);

        // 检测自引用：如果该类型对正在当前线程的创建栈中，返回空计划映射器
        if (!_creatingMappers.Value!.Add(key))
        {
            return _emptyMapperCache.GetOrAdd(key, k => CreateEmptyMapper(k.Item1, k.Item2));
        }

        try
        {
            return _mapperCache.GetOrAdd(key, k => CreateMapper(k.Item1, k.Item2));
        }
        finally
        {
            _creatingMappers.Value!.Remove(key);
        }
    }

    /// <summary>
    /// 清空所有缓存（用于测试和动态类型场景下的内存回收）
    /// </summary>
    public static void ClearCache()
    {
        _mapperCache.Clear();
        _mapFuncCache.Clear();
        _emptyMapperCache.Clear();
    }

    /// <summary>
    /// 获取当前缓存的 Mapper 实例数量
    /// </summary>
    public static int GetCacheSize() => _mapperCache.Count;

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

    /// <summary>
    /// 创建空计划映射器：用于自引用类型，防止 Mapper 构造阶段无限递归。
    /// 该映射器的编译后委托仅返回 new TTarget()，不做任何属性赋值。
    /// </summary>
    private static object CreateEmptyMapper(Type sourceType, Type targetType)
    {
        var emptyPlan = new TypeMappingPlan(
            sourceType, targetType,
            Array.Empty<MemberMapping>(), Array.Empty<MemberMapping>(),
            Array.Empty<MemberInfo>(), Array.Empty<MemberInfo>());

        var mapperType = typeof(Mapper<,>).MakeGenericType(sourceType, targetType);
        var constructor = mapperType.GetConstructor(new[] { typeof(TypeMappingPlan) });
        return constructor!.Invoke(new object[] { emptyPlan });
    }
}
