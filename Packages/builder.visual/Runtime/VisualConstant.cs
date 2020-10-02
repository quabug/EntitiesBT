using System;
using EntitiesBT.Variable;
using Runtime;
using UnityEngine;

namespace EntitiesBT.Builder.Visual
{
    public interface IVisualConstant<T> : IConstantNode<T>, IVisualVariablePropertyNode<T> where T : unmanaged
    {
        T Value { get; }
    }

    public static class VisualConstantExtension
    {
        public static IVariableProperty<T> VariableProperty<T>(this IVisualConstant<T> constant) where T : unmanaged
        {
            return new CustomVariableProperty<T> {CustomValue = constant.Value};
        }
    }

    [NodeSearcherItem("EntitiesBT/Build/ConstFloat")]
    [Serializable]
    public struct VisualConstantFloat : IVisualConstant<float>
    {
        public IVariableProperty<float> GetVariableProperty() => this.VariableProperty();

        [field: SerializeField]
        public float Value { get; private set; }

        [PortDescription(Runtime.ValueType.Float, Description = "Returns the float value.")]
        public OutputDataPort ValuePort;

        public void Execute<TCtx>(TCtx ctx) where TCtx : IGraphInstance {}
    }
}