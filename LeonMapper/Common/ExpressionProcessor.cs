using System.Linq.Expressions;

namespace LeonMapper.Common
{
    internal static class ExpressionProcessor<TIn, TOut>
    {
        private static readonly List<Action<TIn, TOut>> _mapActions;
        private static readonly Func<TOut> _createTargetObjectFunc;

        static ExpressionProcessor()
        {
            var targetType = typeof(TOut);
            var sourceType = typeof(TIn);

            var targetParamExp = Expression.Parameter(targetType, "target");
            var sourceParamExp = Expression.Parameter(sourceType, "source");

            _createTargetObjectFunc = Expression.Lambda<Func<TOut>>(Expression.New(targetType), null).Compile();

            var targetTypeFields = targetType.GetFields().ToDictionary(t => t.Name);
            var sourceTypeFields = sourceType.GetFields().ToDictionary(t => t.Name);

            var targetTypeProps = targetType.GetProperties().ToDictionary(t => t.Name);
            var sourceTypeProps = sourceType.GetProperties().ToDictionary(t => t.Name);

            _mapActions = new List<Action<TIn, TOut>>(targetTypeFields.Count + targetTypeProps.Count);

            foreach (var targetTypeField in targetTypeFields)
            {
                if (sourceTypeFields.ContainsKey(targetTypeField.Key)
                    && object.Equals(targetTypeField.Value.FieldType, sourceTypeFields[targetTypeField.Key].FieldType))
                {
                    var exp = Expression.Assign(Expression.Field(targetParamExp, targetTypeField.Key), Expression.Field(sourceParamExp, targetTypeField.Key));
                    var action = Expression.Lambda<Action<TIn, TOut>>(exp, sourceParamExp, targetParamExp).Compile();
                    _mapActions.Add(action);
                }
            }

            foreach (var targetTypeProp in targetTypeProps)
            {
                if (!targetTypeProp.Value.CanWrite)
                {
                    continue;
                }
                if (sourceTypeProps.ContainsKey(targetTypeProp.Key)
                    && object.Equals(targetTypeProp.Value.PropertyType, sourceTypeProps[targetTypeProp.Key].PropertyType))
                {
                    var exp = Expression.Assign(Expression.Property(targetParamExp, targetTypeProp.Key), Expression.Property(sourceParamExp, targetTypeProp.Key));
                    var action = Expression.Lambda<Action<TIn, TOut>>(exp, sourceParamExp, targetParamExp).Compile();
                    _mapActions.Add(action);
                }
            }
        }

        public static TOut CreateAndMap(TIn source)
        {
            Assert.IsNotNull(source);
            var target = _createTargetObjectFunc.Invoke();
            Map(source, target);
            return target;
        }

        public static void Map(TIn source, TOut target)
        {
            Assert.IsNotNull(source);
            Assert.IsNotNull(target);
            foreach (var action in _mapActions)
            {
                action.Invoke(source, target);
            }
        }
    }
}
