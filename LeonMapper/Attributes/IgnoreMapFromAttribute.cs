namespace LeonMapper.Attributes;

/// <summary>
/// 标记目标成员不接受来自源类型的映射
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class IgnoreMapFromAttribute : Attribute
{
}
