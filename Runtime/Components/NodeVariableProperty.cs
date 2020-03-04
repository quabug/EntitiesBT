using System;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using EntitiesBT.Variable;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Components
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VariableNodeObjectAttribute : PropertyAttribute
    {
        public string NodeObjectFieldName;
        public VariableNodeObjectAttribute(string nodeObjectFieldName)
        {
            NodeObjectFieldName = nodeObjectFieldName;
        }
    }
    
    public struct DynamicNodeData
    {
        public int Index;
        public int Offset;
    }
    
    public class NodeVariableProperty<T> : VariableProperty<T> where T : struct
    {
        public override int VariablePropertyTypeId => AccessRuntimeData ? _ID_RUNTIME_NODE : _ID_DEFAULT_NODE;
        
        public T FallbackValue;
        public BTNode NodeObject;
        [VariableNodeObject("NodeObject")]
        public string ValueFieldName;
        public bool AccessRuntimeData = true;
        
        protected override void AllocateData(ref BlobBuilder builder, ref BlobVariable<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            var index = Array.FindIndex(tree, node => ReferenceEquals(node.Value, NodeObject));
            if (!NodeObject || index < 0)
            {
                Debug.LogError($"Invalid `NodeObject` {NodeObject}", (UnityEngine.Object)self);
                builder.Allocate(ref blobVariable, FallbackValue);
                return;
            }
            
            var nodeType = VirtualMachine.GetNodeType(NodeObject.NodeId);
            if (string.IsNullOrEmpty(ValueFieldName) && nodeType == typeof(T))
            {
                builder.Allocate(ref blobVariable, new DynamicNodeData{ Index = index, Offset = 0});
                return;
            }

            var fieldInfo = nodeType.GetField(ValueFieldName);
            if (fieldInfo == null)
            {
                UnityEngine.Debug.LogError($"Invalid `ValueFieldName` {ValueFieldName}", (UnityEngine.Object)self);
                builder.Allocate(ref blobVariable, FallbackValue);
                return;
            }

            var fieldOffset = Marshal.OffsetOf(nodeType, ValueFieldName).ToInt32();
            builder.Allocate(ref blobVariable, new DynamicNodeData{ Index = index, Offset = fieldOffset});
        }

        static NodeVariableProperty()
        {
            VariableRegisters<T>.Register(_ID_RUNTIME_NODE, GetRuntimeNodeData, GetRuntimeNodeDataRef);
            VariableRegisters<T>.Register(_ID_DEFAULT_NODE, GetDefaultNodeData, GetDefaultNodeDataRef);
        }

        private static readonly int _ID_RUNTIME_NODE = new Guid("220681AA-D884-4E87-90A8-5A8657A734BD").GetHashCode();
        private static readonly int _ID_DEFAULT_NODE = new Guid("743FABC9-77E6-4C2B-A475-ADED2A3B96AD").GetHashCode();
        
        private static T GetRuntimeNodeData(ref BlobVariable<T> blobVariable, int index, INodeBlob blob, IBlackboard bb)
        {
            return GetRuntimeNodeDataRef(ref blobVariable, index, blob, bb);
        }
        
        private static unsafe ref T GetRuntimeNodeDataRef(ref BlobVariable<T> blobVariable, int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blobVariable.Value<DynamicNodeData>();
            var ptr = (byte*)blob.GetNodeDataPtr(data.Index);
            return ref UnsafeUtilityEx.AsRef<T>(ptr + data.Offset);
        }
        
        private static T GetDefaultNodeData(ref BlobVariable<T> blobVariable, int index, INodeBlob blob, IBlackboard bb)
        {
            return GetDefaultNodeDataRef(ref blobVariable, index, blob, bb);
        }
        
        private static unsafe ref T GetDefaultNodeDataRef(ref BlobVariable<T> blobVariable, int index, INodeBlob blob, IBlackboard bb)
        {
            ref var data = ref blobVariable.Value<DynamicNodeData>();
            var ptr = (byte*)blob.GetDefaultDataPtr(data.Index);
            return ref UnsafeUtilityEx.AsRef<T>(ptr + data.Offset);
        }
    }
}
