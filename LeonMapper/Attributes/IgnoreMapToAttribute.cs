namespace LeonMapper.Attributes;

/// <summary>
/// 标记源成员不参与映射到目标类型的过程
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class IgnoreMapToAttribute : Attribute
{
}
