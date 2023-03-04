namespace LeonMapper;

public class MapperConfig
{
    private static ProcessTypeEnum _defaultProcessType = ProcessTypeEnum.Expression;

    public static void SetDefaultProcessType(ProcessTypeEnum defaultProcessType)
    {
        _defaultProcessType = defaultProcessType;
    }

    internal static ProcessTypeEnum GetDefaultProcessType()
    {
        return _defaultProcessType;
    }
}