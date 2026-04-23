namespace LeonMapper.Attributes;

/// <summary>
/// 标记成员在映射过程中被完全忽略（不参与任何方向的映射）
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class IgnoreMapAttribute : Attribute
{
}
