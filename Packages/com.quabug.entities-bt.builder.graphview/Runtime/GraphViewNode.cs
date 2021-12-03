using System;
using System.Collections.Generic;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT
{
    public class GraphViewNode : MonoBehaviour, INodeDataBuilder
    {
        public NodeAsset NodeData;

        public INodeDataBuilder Self => this;
        public IEnumerable<INodeDataBuilder> Children => this.Children();

        public object GetPreviewValue(string path)
        {
            var builder = NodeData.FindBuilderByPath(path);
            return builder.PreviewValue;
        }

        public void SetPreviewValue(string path, object value)
        {
            var builder = NodeData.FindBuilderByPath(path);
            builder.PreviewValue = value;
        }

        public int NodeIndex { get; set; }
        public int NodeId => NodeType.GetBehaviorNodeAttribute().Id;

        private Type NodeType => Type.GetType(NodeData.NodeType ?? "") ?? typeof(ZeroNode);

        public unsafe BlobAssetReference Build(ITreeNode<INodeDataBuilder>[] builders)
        {
            if (NodeType.IsZeroSizeStruct()) return BlobAssetReference.Null;

            var blobBuilder = new BlobBuilder(Allocator.Temp, UnsafeUtility.SizeOf(NodeType));
            try
            {
                var dataPtr = blobBuilder.ConstructRootPtrByType(NodeType);
                NodeData.Build(blobBuilder, new IntPtr(dataPtr));
                return blobBuilder.CreateReferenceByType(NodeType);
            }
            finally
            {
                blobBuilder.Dispose();
            }
        }
    }
}