using System.Reflection;
using LeonMapper.Processors.Model;

namespace LeonMapper.Processors;

public static class Constants
{
    public static readonly Dictionary<string, MethodInvoker> COMPLEX_TYPE_MAP_TO_METHOD_DICTIONARY =
        new Dictionary<string, MethodInvoker>();
}