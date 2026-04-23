namespace LeonMapper.Convert;

/// <summary>
/// 类型转换器接口：将 TInput 类型转换为 TOutput 类型
/// </summary>
/// <typeparam name="TInput">源类型</typeparam>
/// <typeparam name="TOutput">目标类型</typeparam>
public interface IConverter<in TInput, out TOutput>
{
    /// <summary>
    /// 执行类型转换
    /// </summary>
    /// <param name="input">输入值</param>
    /// <returns>转换后的值</returns>
    TOutput Convert(TInput input);
}
