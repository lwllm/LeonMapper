using System.Linq.Expressions;
using System.Reflection;
using LeonMapper.Plan;

namespace LeonMapper.Compilers;

/// <summary>
/// 表达式树编译器：从 TypeMappingPlan 编译为 Func 委托
/// </summary>
public class ExpressionCompiler<TSource, TTarget> : ICompiler<TSource, TTarget> where TTarget : class
{
    private readonly Func<TSource, TTarget?> _compiledFunc;

    public ExpressionCompiler(TypeMappingPlan plan)
    {
        _compiledFunc = BuildFunc(plan);
    }

    public TTarget? Map(TSource source)
    {
        return Equals(source, default(TSource)) ? default : _compiledFunc(source);
    }

    private static Func<TSource, TTarget?> BuildFunc(TypeMappingPlan plan)
    {
        var sourceParam = Expression.Parameter(typeof(TSource), "source");

        // new TTarget()
        var newTargetExpr = Expression.New(plan.TargetType);

        var bindings = new List<MemberBinding>();

        // 属性映射
        foreach (var mapping in plan.PropertyMappings)
        {
            var binding = BuildPropertyBinding(sourceParam, mapping);
            if (binding != null)
                bindings.Add(binding);
        }

        // 字段映射
        foreach (var mapping in plan.FieldMappings)
        {
            var binding = BuildFieldBinding(sourceParam, mapping);
            if (binding != null)
                bindings.Add(binding);
        }

        var memberInit = Expression.MemberInit(newTargetExpr, bindings);
        var lambda = Expression.Lambda<Func<TSource, TTarget?>>(memberInit, sourceParam);
        return lambda.Compile();
    }

    private static MemberBinding? BuildPropertyBinding(ParameterExpression sourceParam, MemberMapping mapping)
    {
        var sourceProp = (PropertyInfo)mapping.SourceMember;
        var targetProp = (PropertyInfo)mapping.TargetMember;

        var sourceAccess = Expression.Property(sourceParam, sourceProp);

        return mapping.Strategy switch
        {
            MappingStrategy.Direct => Expression.Bind(targetProp, sourceAccess),
            MappingStrategy.Convert => BuildConvertBinding(targetProp, sourceAccess, mapping),
            MappingStrategy.Complex => BuildComplexBinding(targetProp, sourceAccess, mapping),
            _ => null
        };
    }

    private static MemberBinding? BuildFieldBinding(ParameterExpression sourceParam, MemberMapping mapping)
    {
        var sourceField = (FieldInfo)mapping.SourceMember;
        var targetField = (FieldInfo)mapping.TargetMember;

        var sourceAccess = Expression.Field(sourceParam, sourceField);

        return mapping.Strategy switch
        {
            MappingStrategy.Direct => Expression.Bind(targetField, sourceAccess),
            MappingStrategy.Convert => BuildConvertBinding(targetField, sourceAccess, mapping),
            MappingStrategy.Complex => BuildComplexBinding(targetField, sourceAccess, mapping),
            _ => null
        };
    }

    private static MemberBinding BuildConvertBinding(MemberInfo targetMember, Expression sourceAccess,
        MemberMapping mapping)
    {
        var converterType = mapping.Converter!.GetType();
        var convertMethod = converterType.GetMethod("Convert")!;
        var targetMemberType = GetMemberType(targetMember);

        var converterInstance = Expression.Constant(mapping.Converter);
        var convertCall = Expression.Call(converterInstance, convertMethod, sourceAccess);

        var convertedExpr = Expression.Convert(convertCall, targetMemberType);
        return targetMember is PropertyInfo prop
            ? Expression.Bind(prop, convertedExpr)
            : Expression.Bind((FieldInfo)targetMember, convertedExpr);
    }

    private static MemberBinding BuildComplexBinding(MemberInfo targetMember, Expression sourceAccess,
        MemberMapping mapping)
    {
        var sourceMemberType = GetMemberType(mapping.SourceMember);
        var targetMemberType = GetMemberType(targetMember);

        // 使用缓存的映射委托
        var mapper = CachedMapperFactory.GetOrCreateMapper(sourceMemberType, targetMemberType);
        var mapperConstant = Expression.Constant(mapper);
        var mapMethod = mapper.GetType().GetMethod("MapTo")!;

        var mapCall = Expression.Call(mapperConstant, mapMethod, sourceAccess);
        var convertedExpr = Expression.Convert(mapCall, targetMemberType);

        return targetMember is PropertyInfo prop
            ? Expression.Bind(prop, convertedExpr)
            : Expression.Bind((FieldInfo)targetMember, convertedExpr);
    }

    private static Type GetMemberType(MemberInfo member)
    {
        return member switch
        {
            PropertyInfo p => p.PropertyType,
            FieldInfo f => f.FieldType,
            _ => throw new ArgumentException($"不支持的成员类型: {member.MemberType}")
        };
    }
}
