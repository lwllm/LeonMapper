using System.Collections;
using System.Reflection;

namespace LeonMapper.Utils;

/// <summary>
/// 集合物化工具类：提供集合类型映射的公共物化逻辑
/// </summary>
internal static class CollectionMaterializer
{
    // 缓存常用反射方法
    private static readonly MethodInfo EnumerableToArrayMethod = typeof(Enumerable)
        .GetMethod("ToArray")!;

    private static readonly MethodInfo EnumerableToListMethod = typeof(Enumerable)
        .GetMethod("ToList")!;

    /// <summary>
    /// 将已映射的元素序列物化为目标集合类型
    /// </summary>
    /// <typeparam name="TTargetElement">目标元素类型</typeparam>
    /// <typeparam name="TTargetCollection">目标集合类型</typeparam>
    /// <param name="mapped">已映射的元素序列</param>
    /// <returns>物化后的集合</returns>
    public static TTargetCollection Materialize<TTargetElement, TTargetCollection>(IEnumerable<TTargetElement> mapped)
        where TTargetCollection : class
    {
        var targetType = typeof(TTargetCollection);

        if (targetType.IsArray)
        {
            var array = mapped.ToArray();
            return array as TTargetCollection
                   ?? throw new InvalidOperationException($"无法将数组转换为 {targetType.Name}");
        }

        var isTargetList = targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>);
        var isTargetIEnumerable = targetType == typeof(IEnumerable<TTargetElement>);

        if (isTargetList || isTargetIEnumerable)
        {
            var list = mapped.ToList();
            return list as TTargetCollection
                   ?? throw new InvalidOperationException($"无法将 List 转换为 {targetType.Name}");
        }

        // 对于其他 IEnumerable<T> 派生类型，尝试使用构造函数
        var ctor = targetType.GetConstructor(new[] { typeof(IEnumerable<TTargetElement>) });
        if (ctor != null)
        {
            var list = mapped.ToList();
            return ctor.Invoke(new object[] { list }) as TTargetCollection
                   ?? throw new InvalidOperationException($"无法通过构造函数创建 {targetType.Name}");
        }

        throw new InvalidOperationException($"不支持的集合目标类型: {targetType.Name}");
    }

    /// <summary>
    /// 根据目标集合类型构建物化表达式
    /// </summary>
    /// <param name="sourceExpression">源集合表达式（IEnumerable&lt;T&gt; 类型）</param>
    /// <param name="targetType">目标集合类型</param>
    /// <param name="targetElementType">目标元素类型</param>
    /// <returns>物化表达式</returns>
    public static System.Linq.Expressions.Expression BuildMaterializeExpression(
        System.Linq.Expressions.Expression sourceExpression, Type targetType, Type targetElementType)
    {
        if (targetType.IsArray)
        {
            var toArrayMethod = EnumerableToArrayMethod.MakeGenericMethod(targetElementType);
            return System.Linq.Expressions.Expression.Call(toArrayMethod, sourceExpression);
        }

        var isTargetList = targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>);
        var isTargetIEnumerable = targetType == typeof(IEnumerable<>).MakeGenericType(targetElementType);

        if (isTargetList)
        {
            var toListMethod = EnumerableToListMethod.MakeGenericMethod(targetElementType);
            return System.Linq.Expressions.Expression.Call(toListMethod, sourceExpression);
        }

        if (isTargetIEnumerable)
        {
            return sourceExpression;
        }

        var ctor = targetType.GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(targetElementType) });
        if (ctor != null)
        {
            return System.Linq.Expressions.Expression.New(ctor, sourceExpression);
        }

        throw new InvalidOperationException($"不支持的集合目标类型: {targetType.Name}");
    }
}
