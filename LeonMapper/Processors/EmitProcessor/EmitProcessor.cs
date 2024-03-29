﻿using System.Reflection.Emit;
using LeonMapper.Exceptions;

namespace LeonMapper.Processors.EmitProcessor;

public class EmitProcessor<TInput, TOutput> : AbstractProcessor<TInput, TOutput> where TOutput : class
{
    private static readonly Func<TInput, TOutput> MapFunc;

    static EmitProcessor()
    {
        var methodName = $"Map_{typeof(TInput).FullName?.Replace(".","_")}__to__{typeof(TOutput).FullName?.Replace(".","_")}_Method_{Guid.NewGuid():N}";
        var method = new DynamicMethod(methodName, typeof(TOutput), new[] { typeof(TInput) });
        var generator = method.GetILGenerator();
        var outCtor = typeof(TOutput).GetConstructor(Type.EmptyTypes);
        if (outCtor == null)
        {
            throw new NoEmptyConstructorException();
        }
        generator.DeclareLocal(typeof(TOutput));
        generator.Emit(OpCodes.Newobj, outCtor);
        generator.Emit(OpCodes.Stloc_0);
        foreach (var propertyMap in PropertyDictionary)
        {
            // var tinGetMethod = propertyMap.Key.GetGetMethod();
            // var toutSetMethod = propertyMap.Value.GetSetMethod();
            // if (tinGetMethod != null && toutSetMethod != null)
            // {
            //     generator.Emit(OpCodes.Ldloc_0);
            //     generator.Emit(OpCodes.Ldarg_0);
            //     generator.Emit(OpCodes.Callvirt,tinGetMethod);
            //     generator.Emit(OpCodes.Callvirt, toutSetMethod);
            // }
        }

        foreach (var fieldInfo in FieldDictionary)
        {
            // generator.Emit(OpCodes.Ldloc_0);
            // generator.Emit(OpCodes.Ldarg_0);
            // generator.Emit(OpCodes.Ldfld, fieldInfo.Key);
            // generator.Emit(OpCodes.Stfld, fieldInfo.Value);
        }
        generator.Emit(OpCodes.Ldloc_0);
        generator.Emit(OpCodes.Ret);
        MapFunc = (Func<TInput, TOutput>)method.CreateDelegate(typeof(Func<TInput, TOutput>));
    }


    public override TOutput? MapTo(TInput input)
    {
        return Equals(input, default(TInput)) ? default(TOutput) : MapFunc(input);
    }
}