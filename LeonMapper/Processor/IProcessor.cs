namespace LeonMapper.Implement;

public interface IProcessor<in TInput, out TOutput>
{
    public TOutput MapTo(TInput input);
}