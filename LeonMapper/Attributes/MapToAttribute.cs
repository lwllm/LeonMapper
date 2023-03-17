namespace LeonMapper.Attributes;

/// <summary>
/// MapTo优先于MapFrom
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class MapToAttribute : Attribute
{
    public string MapToName { get; }

    public MapToAttribute(string mapToName)
    {
        this.MapToName = mapToName;
    }
}