namespace LeonMapper.Attributes;

/// <summary>
/// MapTo优先于MapFrom
/// </summary>
public class MapToAttribute : Attribute
{
    public string MapToName { get; }

    public MapToAttribute(string mapToName)
    {
        this.MapToName = mapToName;
    }
}