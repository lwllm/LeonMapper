using System.Reflection;

namespace LeonMapper.Processors.Model;

public class MethodInvoker
{
    public MethodInfo? MethodInfo { get; set; }

    public Object Invoker { get; set; }
}