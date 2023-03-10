namespace LeonMapper.Config;

public class MapperConfig
{
    private static ProcessTypeEnum _defaultProcessType = ProcessTypeEnum.Expression;
    private static ConverterScopeEnum _defaultConverterScope = ConverterScopeEnum.CommonConverters;
    private static bool _autoConverter = true;

    public static void SetDefaultProcessType(ProcessTypeEnum defaultProcessType)
    {
        _defaultProcessType = defaultProcessType;
    }

    public static void SetConverterScope(ConverterScopeEnum converterScope)
    {
        _defaultConverterScope = converterScope;
    }

    public static void SetAutoConvert(bool autoConvert)
    {
        _autoConverter = autoConvert;
    }

    internal static ProcessTypeEnum GetDefaultProcessType()
    {
        return _defaultProcessType;
    }

    internal static ConverterScopeEnum GetDefaultConverterScope()
    {
        return _defaultConverterScope;
    }

    internal static bool GetAutoConvert()
    {
        return _autoConverter;
    }
}