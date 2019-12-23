using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Editor;
using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Test
{
    public struct NodeDataA : INodeData
    {
        public int A;
    }
    
    public struct NodeDataB : INodeData
    {
        public int B;
        public int BB;
    }
    
    public struct CompositeData : INodeData {}
    
    public class TestBT
    {
        private Dictionary<string, Func<BTNode>> _nodeCreators = new Dictionary<string, Func<BTNode>>
        {
            { "seq", Create<BTSequence> }
          , { "sel", Create<BTSelector> }
          , { "par", Create<BTParallel> }
          , { "yes", () => CreateTerminal(NodeState.Success) }
          , { "no", () => CreateTerminal(NodeState.Failure) }
          , { "run", () => CreateTerminal(NodeState.Running) }
        };

        static BTTerminal CreateTerminal(NodeState state)
        {
            var terminal = Create<BTTerminal>();
            terminal.State = state;
            return terminal;
        }

        static T Create<T>() where T : BTNode
        {
            return new GameObject(typeof(T).Name).AddComponent<T>();
        }

        GameObject CreateBTNode(string branch)
        {
            branch = "seq:yes|no|run";
            var splits = branch.Split(':');
            Assert.AreEqual(splits.Length, 2);
            var parent = Create(splits[0]);
            foreach (var childName in splits[1].Split('|'))
            {
                var child = Create(childName);
                child.transform.SetParent(parent.transform, false);
            }
            return parent;

            GameObject Create(string name) => _nodeCreators[name]().gameObject;
        }
        
        [Test]
        public unsafe void TestNodeBlob()
        {
            Debug.Log($"sizeof Composite: {sizeof(CompositeData)}");
            Debug.Log($"sizeof NodeA: {sizeof(NodeDataA)}");
            Debug.Log($"sizeof NodeB: {sizeof(NodeDataB)}");
            
            var size = sizeof(CompositeData) + sizeof(NodeDataA) + sizeof(NodeDataB);
            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var blob = ref blobBuilder.ConstructRoot<NodeBlob>();
                var types = blobBuilder.Allocate(ref blob.Types,  3);
                types[0] = 123;
                types[1] = 234;
                types[2] = 345;
                
                var endIndices = blobBuilder.Allocate(ref blob.EndIndices, 3);
                endIndices[0] = 3;
                endIndices[1] = 2;
                endIndices[2] = 3;
                
                var offsets = blobBuilder.Allocate(ref blob.Offsets,  3);
                var unsafePtr = (byte*) blobBuilder.Allocate(ref blob.DataBlob, size).GetUnsafePtr();
                var offset = 0;
                offsets[0] = offset;
                UnsafeUtilityEx.AsRef<CompositeData>(unsafePtr + offset);
                offset += sizeof(CompositeData);
                offsets[1] = offset;
                UnsafeUtilityEx.AsRef<NodeDataA>(unsafePtr + offset).A = 111;
                offset += sizeof(NodeDataA);
                offsets[2] = offset;
                ref var local2 = ref UnsafeUtilityEx.AsRef<NodeDataB>(unsafePtr + offset);
                local2.B = 222;
                local2.BB = 2222;
                var blobRef = blobBuilder.CreateBlobAssetReference<NodeBlob>(Allocator.Persistent);
                try
                {
                    Assert.IsTrue(blobRef.IsCreated);
                    Assert.AreEqual(blobRef.Value.DataBlob.Length, size);
                    Assert.AreEqual(blobRef.Value.Count, 3);
                    
                    Assert.AreEqual(blobRef.Value.Types[0], 123);
                    Assert.AreEqual(blobRef.Value.Types[1], 234);
                    Assert.AreEqual(blobRef.Value.Types[2], 345);
                    
                    Assert.AreEqual(blobRef.Value.EndIndices[0], 3);
                    Assert.AreEqual(blobRef.Value.EndIndices[1], 2);
                    Assert.AreEqual(blobRef.Value.EndIndices[2], 3);
                    
                    Assert.AreEqual(blobRef.Value.GetNodeData<NodeDataA>(1).A, 111);
                    ref var b = ref blobRef.Value.GetNodeData<NodeDataB>(2);
                    Assert.AreEqual(b.B, 222);
                    Assert.AreEqual(b.BB, 2222);
                } finally
                {
                    if (blobRef.IsCreated) blobRef.Dispose();
                }
            }
        }

        [Test]
        public void TestSequence() { }
    }
}
