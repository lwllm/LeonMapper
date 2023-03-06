namespace LeonMapper.Processors;

public interface IProcessor<in TInput, out TOutput>
{
    public TOutput MapTo(TInput input);
}