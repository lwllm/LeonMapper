namespace LeonMapper.Convert.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ConverterTypesAttribute : Attribute
{
    public ConverterTypesAttribute(Type inputType, Type outputType)
    {
        InputType = inputType;
        OutputType = outputType;
    }

    public Type InputType { get; }
    public Type OutputType { get; }
}