using LeonMapper.Common;
using LeonMapper.Implement;
using LeonMapper.Implement.Emit;

namespace LeonMapper
{
    public class Mapper<TIn, TOut> where TOut : class
    {
        // private readonly IProcessor<TIn, TOut?> _processor = new EmitProcessor<TIn, TOut?>();  
        private readonly IProcessor<TIn, TOut?> _processor = new Processor.Expression.ExpressionProcessor<TIn, TOut>();

        public TOut? MapTo(TIn source)
        {
            return _processor.MapTo(source);
        }
    }
}