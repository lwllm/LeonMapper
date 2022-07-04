using LeonMapper.Implement;

namespace LeonMapper
{
    public class Mapper
    {
        public static void Map<TIn, TOut>(TIn source, TOut target)
        {
            ExpressionProcessor<TIn, TOut>.Map(source, target);
        }

        public static TOut Map<TIn, TOut>(TIn source)
        {
            return ExpressionProcessor<TIn, TOut>.CreateAndMap(source);
        }
    }
}