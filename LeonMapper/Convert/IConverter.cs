namespace LeonMapper.Converter;

public interface IConverter<TInput, TOutput>
{
    public TOutput Convert(TInput input);
}