namespace LeonMapper.Convert.Attributes;

/// <summary>
/// 标记转换器为常用/安全转换器，仅在 Common 转换器范围下被使用
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CommonConverterAttribute : Attribute
{
}
