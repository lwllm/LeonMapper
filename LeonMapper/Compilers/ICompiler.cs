namespace LeonMapper.Compilers;

/// <summary>
/// 编译器接口
/// </summary>
/// <typeparam name="TSource">源类型</typeparam>
/// <typeparam name="TTarget">目标类型</typeparam>
public interface ICompiler<in TSource, out TTarget>
{
    /// <summary>
    /// 从映射计划编译为映射委托
    /// </summary>
    TTarget? Map(TSource source);
}
