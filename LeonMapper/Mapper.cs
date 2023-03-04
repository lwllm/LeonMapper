using LeonMapper.Implement;
using LeonMapper.Implement.EmitProcessor;

namespace LeonMapper
{
    public class Mapper<TIn, TOut> where TOut : class
    {
        private readonly IProcessor<TIn, TOut?> _emitProcessor = new EmitProcessor<TIn, TOut>();

        private readonly IProcessor<TIn, TOut?> _expressionProcessor =
            new Processor.ExpressionProcessor.ExpressionProcessor<TIn, TOut>();

        public TOut MapTo(TIn source)
        {
            return Equals(MapperConfig.GetDefaultProcessType(), ProcessTypeEnum.Expression)
                ? _expressionProcessor.MapTo(source)
                : _emitProcessor.MapTo(source);
        }

        public TOut MapTo(TIn source, ProcessTypeEnum processType)
        {
            return Equals(processType, ProcessTypeEnum.Expression)
                ? _expressionProcessor.MapTo(source)
                : _emitProcessor.MapTo(source);
        }
    }
}