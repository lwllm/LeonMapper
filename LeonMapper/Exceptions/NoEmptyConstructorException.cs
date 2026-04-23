namespace LeonMapper.Exceptions;

/// <summary>
/// 当目标类型缺少无参构造函数时抛出
/// </summary>
public class NoEmptyConstructorException : System.Exception
{
    /// <summary>
    /// 使用默认错误消息初始化异常
    /// </summary>
    public NoEmptyConstructorException()
        : base("目标类型缺少无参构造函数")
    {
    }

    /// <summary>
    /// 使用指定错误消息初始化异常
    /// </summary>
    /// <param name="message">错误消息</param>
    public NoEmptyConstructorException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// 使用指定错误消息和内部异常初始化异常
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="inner">内部异常</param>
    public NoEmptyConstructorException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
