using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DynamicExpresso;

namespace EntitiesBT.Variant.Expression
{
    public static class Expression
    {
        private static readonly Interpreter _interpreter;
        private static readonly ConcurrentDictionary<LambdaId, Lambda> _lambdas = new ConcurrentDictionary<LambdaId, Lambda>();

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

        public static Lambda Parse(this ref ExpressionVariant.Data data, in LambdaId lambdaId)
        {
            if (_lambdas.TryGetValue(lambdaId, out var lambda)) return lambda;

            var expressionType = VariantValueTypeRegistry.GetTypeById(data.ExpressionType);
            var parameters = new Parameter[data.VariantTypes.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var type = VariantValueTypeRegistry.GetTypeById(data.VariantTypes[i]);
                var name = data.VariantNames[i].ToString();
                parameters[i] = new Parameter(name, type);
            }

            lambda = _interpreter.Parse(
                data.Expression.ToString(),
                expressionType,
                parameters
            );
            _lambdas[lambdaId] = lambda;
            return lambda;
        }
    }
}