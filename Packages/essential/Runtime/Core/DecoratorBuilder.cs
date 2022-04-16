using Blob;
using UnityEngine.Assertions;

namespace EntitiesBT.Core
{
    public class DecoratorBuilder<T> : NodeDataBuilder<T> where T : unmanaged, INodeData
    {
        public DecoratorBuilder(INodeDataBuilder child) : this(child, new ValueBuilder<T>()) {}
        public DecoratorBuilder(INodeDataBuilder child, IBuilder<T> builder) : base(builder, child.Yield())
        {
            Assert.AreEqual(BehaviorNodeType.Decorate, BehaviorNodeType);
        }
    }
}