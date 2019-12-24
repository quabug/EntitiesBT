using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Editor;
using EntitiesBT.Nodes;
using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Test
{
    public struct CompositeData : INodeData {}
    
    public class TestBT
    {
        private Dictionary<string, Func<string, BTNode>> _nodeCreators = new Dictionary<string, Func<string, BTNode>>
        {
            { "seq", Create<BTSequence> }
          , { "sel", Create<BTSelector> }
          , { "par", Create<BTParallel> }
          , { "yes", CreateTerminal(NodeState.Success) }
          , { "no", CreateTerminal(NodeState.Failure) }
          , { "run", CreateTerminal(NodeState.Running) }
          , { "a", CreateA }
          , { "b", CreateB }
        };

        private BehaviorNodeFactory _factory;
        
        [SetUp]
        public void Setup()
        {
            _factory = new BehaviorNodeFactory();
            _factory.RegisterCommonNodes();
            _factory.Register<NodeA>(() => new NodeA());
            _factory.Register<NodeB>(() => new NodeB());
        }
        
        [Test]
        public unsafe void should_able_to_create_and_fetch_data_from_node_blob()
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
        public void should_create_behavior_tree_objects_from_single_line_of_string()
        {
            var root = CreateBTNode("!seq>yes|yes|b:1,1|a:111");
            Assert.AreEqual(root.name, "BTSequence");
            Assert.AreEqual(root.transform.childCount, 4);
            
            var children = root.Children<BTNode>().ToArray();
            
            Assert.AreEqual(children[0].name, "BTTerminal");
            Assert.AreEqual(children[0].transform.childCount, 0);
            
            Assert.AreEqual(children[1].name, "BTTerminal");
            Assert.AreEqual(children[1].transform.childCount, 0);
            
            Assert.AreEqual(children[2].name, "BTNodeB");
            Assert.AreEqual(children[2].transform.childCount, 0);
            
            Assert.AreEqual(children[3].name, "BTNodeA");
            Assert.AreEqual(children[3].transform.childCount, 0);
        }

        [Test]
        public void should_generate_blob_from_nodes()
        {
            var root = CreateBTNode("!seq>yes|no|b:1,1|a:111|run");
            var rootNode = root.GetComponent<BTNode>();
            var blobRef = rootNode.ToBlob(_factory);
            Assert.True(blobRef.IsCreated);
            Assert.AreEqual(blobRef.Value.Count, 6);
            
            var types = new[] { typeof(SequenceNode), typeof(SuccessNode), typeof(FailureNode), typeof(NodeB), typeof(NodeA), typeof(RunningNode) };
            Assert.AreEqual(blobRef.Value.Types.ToArray(), types.Select(_factory.GetTypeId).ToArray());
            Assert.AreEqual(blobRef.Value.Offsets.ToArray(), new [] { 0, 0, 0, 0, 8, 12 });
            Assert.AreEqual(blobRef.Value.EndIndices.ToArray(), new [] { 6, 2, 3, 4, 5, 6 });
            Assert.AreEqual(blobRef.Value.DataBlob.Length, 12);
            Assert.AreEqual(blobRef.Value.GetNodeData<NodeDataB>(3), new NodeDataB {B = 1, BB = 1});
            Assert.AreEqual(blobRef.Value.GetNodeData<NodeDataA>(4), new NodeDataA {A = 111});
        }

#region helpers
        static BTNode CreateA(string @params)
        {
            var nodeA = Create<BTNodeA>();
            nodeA.A = int.Parse(@params);
            return nodeA;
        }
        
        static BTNode CreateB(string @params)
        {
            var nodeB = Create<BTNodeB>();
            var paramArray = @params.Split(',');
            nodeB.B = int.Parse(paramArray[0].Trim());
            nodeB.BB = int.Parse(paramArray[1].Trim());
            return nodeB;
        }

        static Func<string, BTNode> CreateTerminal(NodeState state)
        {
            return @params =>
            {
                var terminal = Create<BTTerminal>();
                terminal.State = state;
                return terminal;
            };
        }

        static T Create<T>(string @params = "") where T : BTNode
        {
            var obj = new GameObject(typeof(T).Name);
            var comp = obj.AddComponent<T>();
            return comp;
        }

        // sample 1: "!seq>yes|no|run|a:10";
        // sample 2: @"
        // seq
        //   yes
        //   no
        //   run
        //   a:10
        //   b:1,2
        // ";
        GameObject CreateBTNode(string branch)
        {
            if (branch.First() == '!') return ParseSingleLine(branch.Substring(1));

            using (var reader = new StringReader(branch))
                return ParseMultiLines(reader);

            GameObject ParseMultiLines(StringReader reader)
            {
                throw new NotImplementedException();
                // var splits = branch.Split('>');
                // Assert.AreEqual(splits.Length, 2);
                // var parent = Create(splits[0].Trim());
                // foreach (var nodeString in splits[1].Split('|'))
                // {
                //     var child = Create(nodeString.Trim());
                //     child.transform.SetParent(parent.transform, false);
                // }
                // return parent;
            }

            GameObject ParseSingleLine(string branchString)
            {
                var splits = branchString.Split('>');
                Assert.AreEqual(splits.Length, 2);
                var parent = Create(splits[0].Trim());
                foreach (var nodeString in splits[1].Split('|'))
                {
                    var child = Create(nodeString.Trim());
                    child.transform.SetParent(parent.transform, false);
                }
                return parent;
            }

            GameObject Create(string nodeString)
            {
                var nameParamsArray = nodeString.Split(':');
                var name = nameParamsArray[0].Trim();
                var @params = nameParamsArray.Length >= 2 ? nameParamsArray[1].Trim() : "";
                return _nodeCreators[name](@params).gameObject;
            }
        }
#endregion
    }
}
