namespace LeonMapper.Attributes;

/// <summary>
/// MapFrom优先于MapTo
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