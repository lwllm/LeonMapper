namespace LeonMapper.Convert;

public interface IConverter<in TInput, out TOutput>
{
    public TOutput Convert(TInput input);
}