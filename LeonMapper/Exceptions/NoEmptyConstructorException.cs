namespace LeonMapper.Exceptions;

/// <summary>
/// 映射异常基类：所有 LeonMapper 抛出的异常都继承此类
/// </summary>
public abstract class MappingException : System.Exception
{
    /// <summary>源类型</summary>
    public Type? SourceType { get; }

    /// <summary>目标类型</summary>
    public Type? TargetType { get; }

    protected MappingException(string message)
        : base(message)
    {
    }

    protected MappingException(string message, Type? sourceType, Type? targetType)
        : base(message)
    {
        SourceType = sourceType;
        TargetType = targetType;
    }

    protected MappingException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}

/// <summary>
/// 当目标类型缺少无参构造函数时抛出
/// </summary>
public sealed class NoEmptyConstructorException : MappingException
{
    public NoEmptyConstructorException()
        : base("目标类型缺少无参构造函数")
    {
    }

    public NoEmptyConstructorException(string message)
        : base(message)
    {
    }

    public NoEmptyConstructorException(Type targetType)
        : base($"目标类型 {targetType.Name} 缺少无参构造函数", null, targetType)
    {
    }

    public NoEmptyConstructorException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}

/// <summary>
/// 当类型转换失败或转换器不存在时抛出
/// </summary>
public sealed class ConverterNotFoundException : MappingException
{
    /// <summary>源转换类型</summary>
    public Type SourceConversionType { get; }

    /// <summary>目标转换类型</summary>
    public Type TargetConversionType { get; }

    public ConverterNotFoundException(Type sourceType, Type targetType)
        : base($"未找到 {sourceType.Name} -> {targetType.Name} 的转换器", sourceType, targetType)
    {
        SourceConversionType = sourceType;
        TargetConversionType = targetType;
    }

    public ConverterNotFoundException(string message)
        : base(message)
    {
        SourceConversionType = typeof(void);
        TargetConversionType = typeof(void);
    }
}

/// <summary>
/// 当成员映射配置无效时抛出（如 MapFrom 引用不存在的源属性）
/// </summary>
public sealed class InvalidMappingConfigurationException : MappingException
{
    /// <summary>目标成员名称</summary>
    public string? TargetMemberName { get; }

    public InvalidMappingConfigurationException(string message)
        : base(message)
    {
    }

    public InvalidMappingConfigurationException(string message, string targetMemberName)
        : base(message)
    {
        TargetMemberName = targetMemberName;
    }
}
