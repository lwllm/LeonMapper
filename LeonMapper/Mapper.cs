using LeonMapper.Common;
using LeonMapper.Implement;
using LeonMapper.Implement.Emit;

namespace LeonMapper
{
    public class Mapper<TIn, TOut> where TOut : class
    {
        private readonly IProcessor<TIn, TOut?> _emitProcessor = new EmitProcessor<TIn, TOut?>();

        private readonly IProcessor<TIn, TOut?> _expressionProcessor =
            new Processor.Expression.ExpressionProcessor<TIn, TOut>();

        public TOut? MapTo(TIn source)
        {
            return _expressionProcessor.MapTo(source);
        }

        public TOut? MapTo(TIn source, ProcessTypeEnum processType)
        {
            return Equals(processType, ProcessTypeEnum.Emit)
                ? _emitProcessor.MapTo(source)
                : _expressionProcessor.MapTo(source);
        }
    }
}