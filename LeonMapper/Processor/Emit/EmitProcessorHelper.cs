using System.Reflection;
using System.Reflection.Emit;

namespace LeonMapper.Processor.Emit;

public class EmitProcessorHelper
{
    private static readonly ModuleBuilder _moduleBuilder;
    // private static readonly TypeBuilder _typeBuilder;
    
    public static ModuleBuilder ModuleBuilder => _moduleBuilder;
    // public static TypeBuilder TypeBuilder => _typeBuilder;
    

    static EmitProcessorHelper()
    {
        AssemblyName assemblyName = new AssemblyName("LeonMapper.Emit.Assembly");
        AssemblyBuilder defineDynamicAssembly =
            AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Run);
        _moduleBuilder = defineDynamicAssembly.DefineDynamicModule("LeonMapper.Emit.Module");
        // _typeBuilder = _moduleBuilder.DefineType("LeonMapper", TypeAttributes.Public);
    }
}