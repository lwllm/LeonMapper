using System.Reflection;

namespace LeonMapper.Utils;

/// <summary>
/// 成员信息工具类：提供属性和字段的类型获取
/// </summary>
internal static class MemberUtils
{
    /// <summary>
    /// 获取成员（属性或字段）的类型
    /// </summary>
    /// <param name="member">成员信息</param>
    /// <returns>成员的类型</returns>
    /// <exception cref="ArgumentException">不支持的成员类型</exception>
    public static Type GetMemberType(MemberInfo member)
    {
        if (member is PropertyInfo p)
        {
            return p.PropertyType;
        }

        if (member is FieldInfo f)
        {
            return f.FieldType;
        }

        // 支持伪成员（用于集合元素映射）
        if (member.MemberType == MemberTypes.Custom && member.DeclaringType != null)
        {
            return member.DeclaringType;
        }

        throw new ArgumentException($"不支持的成员类型: {member.MemberType}");
    }
}
