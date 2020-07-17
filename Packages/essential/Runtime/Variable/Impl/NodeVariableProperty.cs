using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EntitiesBT.Components;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace EntitiesBT.Variable
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
    
    // TODO: check loop ref?
    public class NodeVariableProperty<T> : VariableProperty<T> where T : struct
    {
        private struct DynamicNodeRefData
        {
            public int Index;
            public int Offset;
        }
        
        public BTNode NodeObject;
#if ODIN_INSPECTOR
        IEnumerable<string> _validValueNames => NodeObject == null
            ? Enumerable.Empty<string>()
            : VirtualMachine.GetNodeType(NodeObject.NodeId)
                .GetFields(FIELD_BINDING_FLAGS)
                .Where(fi => fi.FieldType == typeof(T) || fi.FieldType == typeof(BlobVariable<>).MakeGenericType(typeof(T)))
                .Select(fi => fi.Name)
        ;

        [Sirenix.OdinInspector.ValueDropdown("_validValueNames")]
#else
        [VariableNodeObject("NodeObject")]
#endif
        public string ValueFieldName;
        public bool AccessRuntimeData = true;
        
        public override void Allocate(ref BlobBuilder builder, ref BlobVariable<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            var index = Array.FindIndex(tree, node => ReferenceEquals(node.Value, NodeObject));
            if (!NodeObject || index < 0)
            {
                Debug.LogError($"Invalid `NodeObject` {NodeObject}", (UnityEngine.Object)self);
                throw new ArgumentException();
            }
            
            var nodeType = VirtualMachine.GetNodeType(NodeObject.NodeId);
            if (string.IsNullOrEmpty(ValueFieldName) && nodeType == typeof(T))
            {
                builder.Allocate(ref blobVariable, new DynamicNodeRefData{ Index = index, Offset = 0});
                return;
            }

            var fieldInfo = nodeType.GetField(ValueFieldName, FIELD_BINDING_FLAGS);
            if (fieldInfo == null)
            {
                Debug.LogError($"Invalid `ValueFieldName` {ValueFieldName}", (UnityEngine.Object)self);
                throw new ArgumentException();
            }

            var fieldOffset = Marshal.OffsetOf(nodeType, ValueFieldName).ToInt32();
            builder.Allocate(ref blobVariable, new DynamicNodeRefData{ Index = index, Offset = fieldOffset});

            var fieldType = fieldInfo.FieldType;
            if (fieldType == typeof(T))
                blobVariable.VariableId = AccessRuntimeData ? _ID_RUNTIME_NODE : _ID_DEFAULT_NODE;
            else if (fieldType == typeof(BlobVariable<T>))
                blobVariable.VariableId = AccessRuntimeData ? _ID_RUNTIME_NODE_VARIABLE : _ID_DEFAULT_NODE_VARIABLE;
            else
            {
                Debug.LogError($"Invalid type of `ValueFieldName` {ValueFieldName} {fieldType}", (UnityEngine.Object)self);
                throw new ArgumentException();
            }
        }

        static NodeVariableProperty()
        {
            var type = typeof(NodeVariableProperty<T>);
            VariableRegisters<T>.Register(_ID_RUNTIME_NODE, type.Getter("GetRuntimeNodeData"), type.Getter("GetRuntimeNodeDataRef"));
            VariableRegisters<T>.Register(_ID_DEFAULT_NODE, type.Getter("GetDefaultNodeData"), type.Getter("GetDefaultNodeDataRef"));
            VariableRegisters<T>.Register(_ID_DEFAULT_NODE_VARIABLE, type.Getter("GetDefaultNodeVariable"), type.Getter("GetDefaultNodeVariableRef"));
            VariableRegisters<T>.Register(_ID_RUNTIME_NODE_VARIABLE, type.Getter("GetRuntimeNodeVariable"), type.Getter("GetRuntimeNodeVariableRef"));
        }

        private static readonly int _ID_RUNTIME_NODE = new Guid("220681AA-D884-4E87-90A8-5A8657A734BD").GetHashCode();
        private static readonly int _ID_DEFAULT_NODE = new Guid("743FABC9-77E6-4C2B-A475-ADED2A3B96AD").GetHashCode();
        
        private static readonly int _ID_RUNTIME_NODE_VARIABLE = new Guid("7DE6DCA5-71DF-4145-91A6-17EB813B9DEB").GetHashCode();
        private static readonly int _ID_DEFAULT_NODE_VARIABLE = new Guid("A98E0FBF-EF5D-4AFC-9997-0E08A569574D").GetHashCode();
        
        [Preserve]
        private static T GetRuntimeNodeData<TNodeBlob, TBlackboard>(ref BlobVariable<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return GetRuntimeNodeDataRef(ref blobVariable, index, ref blob, ref bb);
        }
        
        [Preserve]
        private static ref T GetRuntimeNodeDataRef<TNodeBlob, TBlackboard>(ref BlobVariable<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return ref GetValue(ref blobVariable, blob.GetRuntimeDataPtr);
        }
        
        [Preserve]
        private static T GetDefaultNodeData<TNodeBlob, TBlackboard>(ref BlobVariable<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return GetDefaultNodeDataRef(ref blobVariable, index, ref blob, ref bb);
        }
        
        [Preserve]
        private static ref T GetDefaultNodeDataRef<TNodeBlob, TBlackboard>(ref BlobVariable<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return ref GetValue(ref blobVariable, blob.GetDefaultDataPtr);
        }

        [Preserve]
        private static unsafe ref T GetValue(ref BlobVariable<T> blobVariable, Func<int, IntPtr> ptrFunc)
        {
            ref var data = ref blobVariable.Value<DynamicNodeRefData>();
            var ptr = ptrFunc(data.Index);
            return ref UnsafeUtilityEx.AsRef<T>(IntPtr.Add(ptr, data.Offset).ToPointer());
        }
        
        [Preserve]
        private static unsafe T GetRuntimeNodeVariable<TNodeBlob, TBlackboard>(ref BlobVariable<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var data = ref blobVariable.Value<DynamicNodeRefData>();
            var ptr = blob.GetRuntimeDataPtr(data.Index);
            ref var variable = ref UnsafeUtilityEx.AsRef<BlobVariable<T>>(IntPtr.Add(ptr, data.Offset).ToPointer());
            return variable.GetData(index, ref blob, ref bb);
        }
        
        [Preserve]
        private static unsafe ref T GetRuntimeNodeVariableRef<TNodeBlob, TBlackboard>(ref BlobVariable<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var data = ref blobVariable.Value<DynamicNodeRefData>();
            var ptr = blob.GetRuntimeDataPtr(data.Index);
            ref var variable = ref UnsafeUtilityEx.AsRef<BlobVariable<T>>(IntPtr.Add(ptr, data.Offset).ToPointer());
            return ref variable.GetDataRef(index, ref blob, ref bb);
        }
        
        [Preserve]
        private static unsafe T GetDefaultNodeVariable<TNodeBlob, TBlackboard>(ref BlobVariable<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var data = ref blobVariable.Value<DynamicNodeRefData>();
            var ptr = blob.GetDefaultDataPtr(data.Index);
            ref var variable = ref UnsafeUtilityEx.AsRef<BlobVariable<T>>(IntPtr.Add(ptr, data.Offset).ToPointer());
            return variable.GetData(index, ref blob, ref bb);
        }
        
        [Preserve]
        private static unsafe ref T GetDefaultNodeVariableRef<TNodeBlob, TBlackboard>(ref BlobVariable<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var data = ref blobVariable.Value<DynamicNodeRefData>();
            var ptr = blob.GetDefaultDataPtr(data.Index);
            ref var variable = ref UnsafeUtilityEx.AsRef<BlobVariable<T>>(IntPtr.Add(ptr, data.Offset).ToPointer());
            return ref variable.GetDataRef(index, ref blob, ref bb);
        }
    }
}
