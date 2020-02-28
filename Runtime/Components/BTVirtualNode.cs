using System.Collections.Generic;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public abstract class BTVirtualNode<T> : INodeDataBuilder where T : struct, INodeData
    {
        public virtual BehaviorNodeType BehaviorNodeType => typeof(T).GetBehaviorNodeAttribute().Type;
        public virtual int NodeId => typeof(T).GetBehaviorNodeAttribute().Id;
        public virtual INodeDataBuilder Self => this;
        public abstract IEnumerable<INodeDataBuilder> Children { get; }

        public BlobAssetReference Build(ITreeNode<INodeDataBuilder>[] builders)
        {
            var minSize = UnsafeUtility.SizeOf<T>();
            if (minSize == 0) return BlobAssetReference.Null;
            using (var blobBuilder = new BlobBuilder(Allocator.Temp, minSize))
            {
                ref var data = ref blobBuilder.ConstructRoot<T>();
                Build(blobBuilder, ref data, builders);
                return blobBuilder.CreateReference<T>();
            }
        }
        
        protected virtual void Build(BlobBuilder blobBuilder, ref T data, ITreeNode<INodeDataBuilder>[] builders) {}
    }

    public class BTVirtualRealSelf : INodeDataBuilder
    {
        public BTVirtualRealSelf(INodeDataBuilder self) => _self = self;
        private readonly INodeDataBuilder _self;
        public BehaviorNodeType BehaviorNodeType => _self.BehaviorNodeType;
        public int NodeId => _self.NodeId;
        public BlobAssetReference Build(ITreeNode<INodeDataBuilder>[] builders) => BlobAssetReference.Null;
        public INodeDataBuilder Self => _self;
        public IEnumerable<INodeDataBuilder> Children => _self.Children;
    }
    
    public class BTVirtualDecorator<T> : BTVirtualNode<T> where T : struct, INodeData
    {
        private readonly INodeDataBuilder _child;
        public BTVirtualDecorator(INodeDataBuilder child) => _child = new BTVirtualRealSelf(child);
        public override IEnumerable<INodeDataBuilder> Children => _child.Yield();
    }
}
