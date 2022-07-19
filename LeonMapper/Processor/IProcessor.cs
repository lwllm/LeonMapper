namespace LeonMapper.Implement;

public interface IProcessor<in TIn, out TOut>
{
    public TOut? MapTo(TIn source);
}