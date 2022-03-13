using System.Collections.Generic;
using EntitiesBT.Core;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public abstract class BTVirtualNode<T> : INodeDataBuilder where T : unmanaged, INodeData
    {
        public virtual int NodeId => typeof(T).GetBehaviorNodeAttribute().Id;
        public virtual INodeDataBuilder Self => this;
        public abstract IEnumerable<INodeDataBuilder> Children { get; }

        public virtual object GetPreviewValue(string path) => throw new System.NotImplementedException();
        public virtual void SetPreviewValue(string path, object value) => throw new System.NotImplementedException();

        public int NodeIndex { get; set; } = 0;

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
        public int NodeId => _self.NodeId;
        public BlobAssetReference Build(ITreeNode<INodeDataBuilder>[] builders) => BlobAssetReference.Null;
        public int NodeIndex { get; set; } = 0;
        public INodeDataBuilder Self => _self;
        public IEnumerable<INodeDataBuilder> Children => _self.Children;

        public virtual object GetPreviewValue(string path) => throw new System.NotImplementedException();
        public virtual void SetPreviewValue(string path, object value) => throw new System.NotImplementedException();
    }
    
    public class BTVirtualDecorator<T> : BTVirtualNode<T> where T : unmanaged, INodeData
    {
        private readonly IEnumerable<INodeDataBuilder> _children;
        public BTVirtualDecorator(INodeDataBuilder child) => _children = new BTVirtualRealSelf(child).Yield();
        public BTVirtualDecorator(IEnumerable<INodeDataBuilder> children) => _children = children;
        public override IEnumerable<INodeDataBuilder> Children => _children;
    }
}
