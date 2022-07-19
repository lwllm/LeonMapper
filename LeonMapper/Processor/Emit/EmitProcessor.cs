using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using LeonMapper.Processor;
using LeonMapper.Processor.Emit;

namespace LeonMapper.Implement.Emit;

public class EmitProcessor<TIn, TOut> : AbstractProcessor<TIn, TOut> where TOut : class
{
    static EmitProcessor()
    {
        var typeName = $"Map_{typeof(TIn).FullName}_to_{typeof(TOut).FullName}_{Guid.NewGuid().ToString("N")}";
        var mapMethodName = "MapTo";
        var typeBuilder =
            EmitProcessorHelper.ModuleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Sealed);
        var method = typeBuilder.DefineMethod(mapMethodName,
            MethodAttributes.Public | MethodAttributes.Static, null, new Type[] { typeof(TIn), typeof(TOut) });
        var il = method.GetILGenerator();
        // foreach (var property in tout.GetType().GetProperties())
        foreach (var propertyMap in PropertyDictionary)
        {
            // property.SetValue(...);
        }

        il.Emit(OpCodes.Ret);
        var mapClassType = typeBuilder.CreateType();
        var t = Activator.CreateInstance(mapClassType);
    }


    public override TOut? MapTo(TIn source)
    {
        var target = Activator.CreateInstance<TOut>();
        return target;
    }
}