namespace LeonMapper.Attributes;

/// <summary>
/// MapFrom优先于MapTo
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