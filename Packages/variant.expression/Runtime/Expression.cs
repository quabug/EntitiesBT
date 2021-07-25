using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DynamicExpresso;

namespace EntitiesBT.Variant.Expression
{
    public static class Expression
    {
        private static readonly Interpreter _interpreter;
        private static List<Lambda> _lambdas = new List<Lambda>(_capacityExpandingCount);
        private const int _capacityExpandingCount = 32;

        static Expression()
        {
            _interpreter = new Interpreter();
            _interpreter.EnableAssignment(AssignmentOperators.None);
            foreach (var type in VariantValueTypeRegistry.GetAllTypes()) _interpreter.Reference(type);
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetCustomAttributes<ExpressionReferenceAttribute>())
                .Select(attribute => attribute.Type)
            ) _interpreter.Reference(type);
        }

        public static Lambda Parse(this ref ExpressionVariant.Data data)
        {
            if (data.LambdaId >= 0) return _lambdas[data.LambdaId];
            var expressionType = VariantValueTypeRegistry.GetTypeById(data.ExpressionType);
            var parameters = new Parameter[data.VariantTypes.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var type = VariantValueTypeRegistry.GetTypeById(data.VariantTypes[i]);
                var name = data.VariantNames[i].ToString();
                parameters[i] = new Parameter(name, type);
            }
            var lambda = _interpreter.Parse(
                data.Expression.ToString(),
                expressionType,
                parameters
            );

            lock (_lambdas)
            {
                var lambdas = _lambdas;
                if (lambdas.Count == lambdas.Capacity)
                {
                    lambdas = new List<Lambda>(_lambdas.Capacity + _capacityExpandingCount);
                    lambdas.AddRange(_lambdas);
                }
                lambdas.Add(lambda);
                data.LambdaId = lambdas.Count - 1;
                _lambdas = lambdas;
            }

            return lambda;
        }
    }
}