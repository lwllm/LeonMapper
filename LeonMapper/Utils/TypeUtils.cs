using System.Collections;

namespace LeonMapper.Utils;

/// <summary>
/// 类型工具类：提供集合类型分析和基础类型判断
/// </summary>
/// <remarks>
/// 本类提供一组静态工具方法，用于类型分析和转换相关的辅助操作，包括：
/// <list type="bullet">
/// <item>基础类型判断（IsBaseType）</item>
/// <item>集合类型元素提取（GetCollectionElementType）</item>
/// <item>集合类型检测（IsCollectionType）</item>
/// <item>可空类型处理（GetUnderlyingType、IsNullableType）</item>
/// </list>
/// </remarks>
internal static class TypeUtils
{
    /// <summary>
    /// 判断类型是否为基础类型（基元类型、string、decimal）
    /// </summary>
    /// <param name="type">要判断的类型，不能为 null</param>
    /// <returns>如果类型是基元类型、string 或 decimal，返回 true；否则返回 false</returns>
    /// <exception cref="ArgumentNullException">当 type 为 null 时抛出</exception>
    /// <example>
    /// <code>
    /// TypeUtils.IsBaseType(typeof(int));     // true
    /// TypeUtils.IsBaseType(typeof(string));  // true
    /// TypeUtils.IsBaseType(typeof(decimal)); // true
    /// TypeUtils.IsBaseType(typeof(DateTime)); // false
    /// </code>
    /// </example>
    public static bool IsBaseType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type.IsEnum;
    }

    /// <summary>
    /// 获取集合类型的元素类型，支持数组、List&lt;T&gt;、IEnumerable&lt;T&gt; 及其派生类型
    /// </summary>
    /// <param name="type">要分析的集合类型，不能为 null</param>
    /// <returns>
    /// 集合的元素类型。如果类型不是支持的集合类型（如普通类、非 IEnumerable 实现等），返回 null
    /// </returns>
    /// <exception cref="ArgumentNullException">当 type 为 null 时抛出</exception>
    /// <remarks>
    /// 支持的集合类型包括：
    /// <list type="bullet">
    /// <item>数组类型（T[]）</item>
    /// <item>List&lt;T&gt;</item>
    /// <item>IEnumerable&lt;T&gt;、ICollection&lt;T&gt;、IList&lt;T&gt;</item>
    /// <item>IReadOnlyList&lt;T&gt;、IReadOnlyCollection&lt;T&gt;</item>
    /// <item>任何实现 IEnumerable&lt;T&gt; 的自定义类型</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// TypeUtils.GetCollectionElementType(typeof(List&lt;int&gt;));           // typeof(int)
    /// TypeUtils.GetCollectionElementType(typeof(string[]));            // typeof(string)
    /// TypeUtils.GetCollectionElementType(typeof(IEnumerable&lt;User&gt;)); // typeof(User)
    /// TypeUtils.GetCollectionElementType(typeof(int));               // null
    /// </code>
    /// </example>
    public static Type? GetCollectionElementType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (type.IsArray)
        {
            return type.GetElementType();
        }

        if (type.IsGenericType)
        {
            var genericDef = type.GetGenericTypeDefinition();
            if (genericDef == typeof(IEnumerable<>) ||
                genericDef == typeof(List<>) ||
                genericDef == typeof(ICollection<>) ||
                genericDef == typeof(IList<>) ||
                genericDef == typeof(IReadOnlyList<>) ||
                genericDef == typeof(IReadOnlyCollection<>))
            {
                return type.GetGenericArguments()[0];
            }
        }

        var enumerableInterface = type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        if (enumerableInterface != null)
        {
            return enumerableInterface.GetGenericArguments()[0];
        }

        return null;
    }

    /// <summary>
    /// 判断类型是否为集合类型（数组或 IEnumerable&lt;T&gt; 派生类型）
    /// </summary>
    /// <param name="type">要判断的类型，不能为 null</param>
    /// <returns>如果类型是支持的集合类型，返回 true；否则返回 false</returns>
    /// <exception cref="ArgumentNullException">当 type 为 null 时抛出</exception>
    /// <seealso cref="GetCollectionElementType"/>
    /// <example>
    /// <code>
    /// TypeUtils.IsCollectionType(typeof(List&lt;int&gt;));     // true
    /// TypeUtils.IsCollectionType(typeof(int[]));           // true
    /// TypeUtils.IsCollectionType(typeof(string));        // false
    /// TypeUtils.IsCollectionType(typeof(Dictionary&lt;int, string&gt;)); // true
    /// </code>
    /// </example>
    public static bool IsCollectionType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return GetCollectionElementType(type) != null;
    }

    /// <summary>
    /// 获取可空类型的底层类型，如果不是可空类型则返回原类型
    /// </summary>
    /// <param name="type">要分析的类型，不能为 null</param>
    /// <returns>
    /// 如果是可空类型（如 int?、DateTime?），返回其底层类型（如 int、DateTime）；
    /// 如果不是可空类型，返回原类型
    /// </returns>
    /// <exception cref="ArgumentNullException">当 type 为 null 时抛出</exception>
    /// <remarks>
    /// 此方法用于处理可空类型映射场景，在需要比较或转换类型前获取实际的基础类型
    /// </remarks>
    /// <example>
    /// <code>
    /// TypeUtils.GetUnderlyingType(typeof(int?));      // typeof(int)
    /// TypeUtils.GetUnderlyingType(typeof(string));      // typeof(string)
    /// TypeUtils.GetUnderlyingType(typeof(List&lt;int&gt;)); // typeof(List&lt;int&gt;)
    /// </code>
    /// </example>
    public static Type GetUnderlyingType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return Nullable.GetUnderlyingType(type) ?? type;
    }

    /// <summary>
    /// 判断类型是否为可空类型（Nullable&lt;T&gt;）
    /// </summary>
    /// <param name="type">要判断的类型，不能为 null</param>
    /// <returns>如果类型是 Nullable&lt;T&gt; 类型，返回 true；否则返回 false</returns>
    /// <exception cref="ArgumentNullException">当 type 为 null 时抛出</exception>
    /// <remarks>
    /// 注意：引用类型（如 string）本身可以为 null，但不是 Nullable&lt;T&gt; 类型，
    /// 此方法仅对值类型的可空包装返回 true
    /// </remarks>
    /// <example>
    /// <code>
    /// TypeUtils.IsNullableType(typeof(int?));     // true
    /// TypeUtils.IsNullableType(typeof(DateTime?)); // true
    /// TypeUtils.IsNullableType(typeof(int));      // false
    /// TypeUtils.IsNullableType(typeof(string));   // false
    /// </code>
    /// </example>
    public static bool IsNullableType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return Nullable.GetUnderlyingType(type) != null;
    }
}
