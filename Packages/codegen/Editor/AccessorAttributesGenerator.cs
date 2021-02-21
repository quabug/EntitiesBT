using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace EntitiesBT.CodeGen.Editor
{
    internal static class AccessorAttributesGenerator
    {
        public static IEnumerable<CustomAttribute> GenerateAccessorAttributes(this MethodDefinition methodDefinition)
        {
            var module = methodDefinition.Module;
            return methodDefinition.HasBody
                ? methodDefinition.Body.Instructions.SelectMany(GenerateAccessorAttributes)
                : Enumerable.Empty<CustomAttribute>()
            ;

            IEnumerable<CustomAttribute> GenerateAccessorAttributes(Instruction instruction)
            {
                switch (instruction.OpCode.Code)
                {
                    case Code.Calli:
                        throw new NotSupportedException("Cannot generate attributes from `calli`");
                    case Code.Call:
                    case Code.Callvirt:
                        return GenerateFromOperand(instruction);
                    default:
                        return Enumerable.Empty<CustomAttribute>();
                }
            }

            IEnumerable<CustomAttribute> GenerateFromOperand(Instruction instruction)
            {
                switch (instruction.Operand)
                {
                    case GenericInstanceMethod method:
                    {
                        var methodDefinition = method.GetElementMethod().Resolve();
                        var genericAttributes = CreateAccessorAttributes(methodDefinition.GenericParameters, method.GenericArguments);
                        var methodAttributes = FindAccessorAttributes(methodDefinition.CustomAttributes);
                        var parameterAttributes = CreateAccessorAttributesOnParameters(methodDefinition, instruction);
                        return genericAttributes.Concat(methodAttributes).Concat(parameterAttributes);
                    }
                    case MethodReference method:
                    {
                        var methodDefinition = method.Resolve();
                        var methodAttributes = FindAccessorAttributes(methodDefinition.CustomAttributes);
                        var parameterAttributes = CreateAccessorAttributesOnParameters(methodDefinition, instruction);
                        return methodAttributes.Concat(parameterAttributes);
                    }
                    default:
                    {
                        return Enumerable.Empty<CustomAttribute>();
                    }
                }
            }

            IEnumerable<CustomAttribute> CreateAccessorAttributes(IEnumerable<ICustomAttributeProvider> attributeProviders, IEnumerable<TypeReference> types)
            {
                return attributeProviders.Zip(types, (param, type) => (attributes: FindAccessorAttributes(param.CustomAttributes), type))
                    .SelectMany(t => t.attributes.Select(attribute => (attribute, t.type)))
                    .Select(t => CreateAccessorAttribute(t.attribute.Constructor, t.type.Yield()))
                ;
            }

            IEnumerable<CustomAttribute> CreateAccessorAttributesOnParameters(MethodDefinition method, Instruction instruction)
            {
                var attributes = FindAccessorAttributes(method.Parameters.SelectMany(param => param.CustomAttributes));
                if (!attributes.Any()) yield break;

                if (method.Parameters.Count > 1)
                    throw new NotSupportedException($"Cannot generate accessor attribute on method({method.FullName}) with multiple parameters. must manually set accessor attributes instead.");
                var parameter = method.Parameters[0];
                foreach (var attr in attributes)
                {
                    var arguments = (CustomAttributeArgument[]) attr.ConstructorArguments[0].Value;
                    if (arguments == null || !arguments.Any())
                    {
                        var type = parameter.ParameterType.FullName == "System.Type"
                            ? GuessParameterType()
                            : parameter.ParameterType
                        ;
                        yield return CreateAccessorAttribute(attr.Constructor, type.Yield());
                    }
                    else
                    {
                        yield return CreateAccessorAttribute(attr.Constructor, arguments.Select(arg => (TypeReference)arg.Value));
                    }
                }

                TypeReference GuessParameterType()
                {
                    var errorMessage = $"Cannot generate accessor attribute on method[{method.FullName}] with unsupported instruction ";
                    instruction = instruction.Previous; // call typeof
                    while (instruction.OpCode.Code == Code.Constrained) instruction = instruction.Previous; // skip constrained
                    if (instruction.OpCode.Code != Code.Call) throw new NotSupportedException(errorMessage + instruction.OpCode.Code + " [-1]");
                    var typeofMethod = instruction.Operand as MethodReference;
                    if (typeofMethod == null) throw new NotSupportedException(errorMessage + instruction.Operand.GetType() + " [-1]");
                    if (typeofMethod.Name != nameof(Type.GetTypeFromHandle)) throw new NotSupportedException(errorMessage + typeofMethod.Name + " [-1]");

                    instruction = instruction.Previous; // ldtoken type
                    if (instruction.OpCode.Code != Code.Ldtoken) throw new NotSupportedException(errorMessage + instruction.OpCode.Code + " [-2]");
                    var type = instruction.Operand as TypeReference;
                    if (type == null) throw new NotSupportedException(errorMessage + instruction.Operand.GetType() + " [-2]");
                    return type;
                }
            }

            CustomAttribute CreateAccessorAttribute(MethodReference constructor, IEnumerable<TypeReference> arguments)
            {
                var attribute = new CustomAttribute(module.ImportReference(constructor));
                var systemTypeReference = module.ImportReference(typeof(Type));
                var argumentType = module.ImportReference(typeof(Type[]));
                var argumentValue = arguments.Select(argType => new CustomAttributeArgument(systemTypeReference, module.ImportReference(argType))).ToArray();
                var argument = new CustomAttributeArgument(argumentType, argumentValue);
                attribute.ConstructorArguments.Add(argument);
                return attribute;
            }
        }

        public static IEnumerable<CustomAttribute> FindAccessorAttributes(this IEnumerable<CustomAttribute> attributes)
        {
            var attributeType = typeof(ComponentAccessorAttribute);
            return attributes.Where(attribute => attributeType.IsAssignableFrom(attribute.AttributeType));
        }
    }
}