using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using LeonMapper.Exception;
using LeonMapper.Processor;
using LeonMapper.Processor.Emit;

namespace LeonMapper.Implement.Emit;

public class EmitProcessor<TIn, TOut> : AbstractProcessor<TIn, TOut> where TOut : class
{
    private static readonly Func<TIn, TOut> mapFunc;

    static EmitProcessor()
    {
        var methodName = $"Map_{typeof(TIn).FullName}_to_{typeof(TOut).FullName}_Method_{Guid.NewGuid():N}";
        var method = new DynamicMethod(methodName, typeof(TOut), new Type[] { typeof(TIn) });
        var generator = method.GetILGenerator();
        var outCtor = typeof(TOut).GetConstructor(Type.EmptyTypes);
        if (outCtor == null)
        {
            throw new NoEmptyConstructorException();
        }
        generator.DeclareLocal(typeof(TOut));
        generator.Emit(OpCodes.Newobj, outCtor);
        generator.Emit(OpCodes.Stloc_0);
        foreach (var propertyMap in PropertyDictionary)
        {
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Callvirt, propertyMap.Key.GetGetMethod());
            generator.Emit(OpCodes.Callvirt, propertyMap.Value.GetSetMethod());
        }

        foreach (var fieldInfo in FieldDictionary)
        {
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, fieldInfo.Key);
            generator.Emit(OpCodes.Stfld, fieldInfo.Value);
        }
        generator.Emit(OpCodes.Ldloc_0);
        generator.Emit(OpCodes.Ret);
        mapFunc = (Func<TIn, TOut>)method.CreateDelegate(typeof(Func<TIn, TOut>));
    }


    public override TOut MapTo(TIn source)
    {
        return mapFunc(source);
    }
}