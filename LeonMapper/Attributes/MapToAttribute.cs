namespace LeonMapper.Attributes;

/// <summary>
/// 标记源成员映射到目标类型的指定成员（支持多个目标）
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class MapToAttribute : Attribute
{
    /// <summary>目标成员名称</summary>
    public string MapToName { get; }

    /// <summary>
    /// 初始化 MapToAttribute
    /// </summary>
    /// <param name="mapToName">目标成员名称</param>
    public MapToAttribute(string mapToName)
    {
        this.MapToName = mapToName;
    }
}
