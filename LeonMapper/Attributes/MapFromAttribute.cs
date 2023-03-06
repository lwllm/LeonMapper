namespace LeonMapper.Attributes;

/// <summary>
/// MapTo优先于MapFrom
/// </summary>
public class MapFromAttribute : Attribute
{
    public string MapFromName { get; }

    public MapFromAttribute(string mapFromName)
    {
        MapFromName = mapFromName;
    }
}