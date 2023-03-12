using System.Reflection;

namespace LeonMapper.Processors;

public static class Constants
{
    public static readonly Dictionary<string, MethodInfo> COMPLEX_TYPE_MAP_TO_METHOD_DICTIONARY =
        new Dictionary<string, MethodInfo>();
}