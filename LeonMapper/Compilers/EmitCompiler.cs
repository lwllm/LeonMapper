using System.Reflection;
using System.Reflection.Emit;
using LeonMapper.Plan;
using LeonMapper.Exceptions;

namespace LeonMapper.Compilers;

/// <summary>
/// IL 编译器：从 TypeMappingPlan 通过 IL Emit 生成映射委托
/// 复杂类型映射通过委托间接调用，确保 IL 生成正确
/// </summary>
public class EmitCompiler<TSource, TTarget> : ICompiler<TSource, TTarget> where TTarget : class
{
    private readonly Func<TSource, TTarget?> _compiledFunc;

    public EmitCompiler(TypeMappingPlan plan)
    {
        _compiledFunc = BuildFunc(plan);
    }

    public TTarget? Map(TSource source)
    {
        return Equals(source, default(TSource)) ? default : _compiledFunc(source);
    }

    private static Func<TSource, TTarget?> BuildFunc(TypeMappingPlan plan)
    {
        var methodName =
            $"Map_{plan.SourceType.FullName?.Replace(".", "_")}__to__{plan.TargetType.FullName?.Replace(".", "_")}_{Guid.NewGuid():N}";
        var method = new DynamicMethod(methodName, plan.TargetType, new[] { typeof(TSource) }, true);
        var il = method.GetILGenerator();

        var outCtor = plan.TargetType.GetConstructor(Type.EmptyTypes);
        if (outCtor == null)
            throw new NoEmptyConstructorException();

        var targetLocal = il.DeclareLocal(plan.TargetType);

        // new TTarget()
        il.Emit(OpCodes.Newobj, outCtor);
        il.Emit(OpCodes.Stloc, targetLocal);

        // 属性映射
        foreach (var mapping in plan.PropertyMappings)
        {
            EmitPropertyCopy(il, targetLocal, mapping);
        }

        // 字段映射
        foreach (var mapping in plan.FieldMappings)
        {
            EmitFieldCopy(il, targetLocal, mapping);
        }

        il.Emit(OpCodes.Ldloc, targetLocal);
        il.Emit(OpCodes.Ret);

        return (Func<TSource, TTarget?>)method.CreateDelegate(typeof(Func<TSource, TTarget?>));
    }

    private static void EmitPropertyCopy(ILGenerator il, LocalBuilder targetLocal, MemberMapping mapping)
    {
        var sourceProp = (PropertyInfo)mapping.SourceMember;
        var targetProp = (PropertyInfo)mapping.TargetMember;
        var getMethod = sourceProp.GetGetMethod(true);
        var setMethod = targetProp.GetSetMethod(true);

        if (getMethod == null || setMethod == null)
            return;

        switch (mapping.Strategy)
        {
            case MappingStrategy.Direct:
                il.Emit(OpCodes.Ldloc, targetLocal);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(getMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, getMethod);
                il.Emit(setMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, setMethod);
                break;

            case MappingStrategy.Convert:
            {
                var convertMethod = mapping.Converter!.GetType().GetMethod("Convert")!;
                il.Emit(OpCodes.Ldloc, targetLocal);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(getMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, getMethod);
                il.Emit(OpCodes.Call, convertMethod);
                il.Emit(setMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, setMethod);
                break;
            }

            case MappingStrategy.Complex:
            {
                // 通过缓存的 Func 委托进行复杂类型映射
                var mapFunc = CachedMapperFactory.GetOrCreateMapFunc(
                    sourceProp.PropertyType, targetProp.PropertyType);
                // 使用静态委托字段引用
                il.Emit(OpCodes.Ldloc, targetLocal);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(getMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, getMethod);
                il.Emit(OpCodes.Call, mapFunc.Method);
                il.Emit(setMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, setMethod);
                break;
            }
        }
    }

    private static void EmitFieldCopy(ILGenerator il, LocalBuilder targetLocal, MemberMapping mapping)
    {
        var sourceField = (FieldInfo)mapping.SourceMember;
        var targetField = (FieldInfo)mapping.TargetMember;

        switch (mapping.Strategy)
        {
            case MappingStrategy.Direct:
                il.Emit(OpCodes.Ldloc, targetLocal);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, sourceField);
                il.Emit(OpCodes.Stfld, targetField);
                break;

            case MappingStrategy.Convert:
            {
                var convertMethod = mapping.Converter!.GetType().GetMethod("Convert")!;
                il.Emit(OpCodes.Ldloc, targetLocal);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, sourceField);
                il.Emit(OpCodes.Call, convertMethod);
                il.Emit(OpCodes.Stfld, targetField);
                break;
            }

            case MappingStrategy.Complex:
            {
                var mapFunc = CachedMapperFactory.GetOrCreateMapFunc(
                    sourceField.FieldType, targetField.FieldType);
                il.Emit(OpCodes.Ldloc, targetLocal);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, sourceField);
                il.Emit(OpCodes.Call, mapFunc.Method);
                il.Emit(OpCodes.Stfld, targetField);
                break;
            }
        }
    }
}
