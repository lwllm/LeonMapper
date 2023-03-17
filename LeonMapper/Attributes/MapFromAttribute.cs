namespace LeonMapper.Attributes;

/// <summary>
/// MapTo优先于MapFrom
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class MapFromAttribute : Attribute
{
    public string MapFromName { get; }

    public MapFromAttribute(string mapFromName)
    {
        MapFromName = mapFromName;
    }
}