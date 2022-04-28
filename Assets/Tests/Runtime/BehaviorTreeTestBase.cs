using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blob;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using NUnit.Framework;

namespace EntitiesBT.Test
{
    public class BehaviorTreeTestBase
    {
        public abstract class NodeDataBuilder : INodeDataBuilder
        {
            public abstract Type NodeType { get; }
            public abstract int NodeId { get; }
            public int NodeIndex { get; set; }
            public abstract IBuilder BlobStreamBuilder { get; }
            public IEnumerable<INodeDataBuilder> Children => ChildrenList;
            public readonly List<NodeDataBuilder> ChildrenList = new List<NodeDataBuilder>();
        }
        
        public class NodeDataBuilder<T> : NodeDataBuilder where T : unmanaged, INodeData
        {
            public override Type NodeType => typeof(T);
            public override int NodeId => typeof(T).GetBehaviorNodeAttribute().Id;
            public override IBuilder BlobStreamBuilder => new ValueBuilder<T>(Value);
            public T Value { get; }
            public NodeDataBuilder() : this(default) {}
            public NodeDataBuilder(T value) => Value = value;
        }

        private readonly Dictionary<string, Func<string, NodeDataBuilder>> _nodeCreators = new Dictionary<string, Func<string, NodeDataBuilder>>
        {
            { "seq", _ => new NodeDataBuilder<SequenceNode>() }
          , { "sel", _ => new NodeDataBuilder<SelectorNode>() }
          , { "par", _ => new NodeDataBuilder<ParallelNode>() }
          , { "yes", CreateTestNode(NodeState.Success) }
          , { "no", CreateTestNode(NodeState.Failure) }
          , { "run", CreateTestNode(NodeState.Running) }
          , { "a", CreateA }
          , { "b", CreateB }
        };

        protected Blackboard _blackboard;
        
        protected struct Blackboard : IBlackboard
        {
            public bool HasData<T>() where T : struct
            {
                throw new NotImplementedException();
            }

            public T GetData<T>() where T : struct
            {
                throw new NotImplementedException();
            }

            public ref T GetDataRef<T>() where T : struct
            {
                throw new NotImplementedException();
            }

            public bool HasData(Type type)
            {
                throw new NotImplementedException();
            }

            public IntPtr GetDataPtrRO(Type type)
            {
                throw new NotImplementedException();
            }

            public IntPtr GetDataPtrRW(Type type)
            {
                throw new NotImplementedException();
            }

            public T GetObject<T>() where T : class
            {
                throw new NotImplementedException();
            }
        }

        [SetUp]
        public void Setup()
        {
            _blackboard = new Blackboard();
        }

        protected ManagedNodeBlobRef CreateBlob(string tree)
        {
            var root = CreateRootBuilder(tree);
            var blob = root.ToBuilder(Enumerable.Empty<IGlobalValuesBuilder>()).CreateManagedBlobAssetReference();
            return new ManagedNodeBlobRef(blob);
        }
    
        private static NodeDataBuilder CreateA(string @params)
        {
            return new NodeDataBuilder<NodeA>(new NodeA { A = int.Parse(@params) });
        }
        
        private static NodeDataBuilder CreateB(string @params)
        {
            var paramArray = @params.Split(',');
            var node = new NodeB
            {
                B = int.Parse(paramArray[0].Trim()),
                BB = int.Parse(paramArray[1].Trim())
            };
            return new NodeDataBuilder<NodeB>(node);
        }

        private static Func<string, NodeDataBuilder> CreateTestNode(NodeState state)
        {
            return @params => new NodeDataBuilder<TestNode>(new TestNode { State = state } );
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
        protected NodeDataBuilder CreateRootBuilder(string branch)
        {
            if (branch.First() == '!') return ParseSingleLine(branch.Substring(1));

            using (var reader = new StringReader(branch))
                return ParseMultiLines(reader);

            NodeDataBuilder ParseMultiLines(StringReader reader)
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

            NodeDataBuilder ParseSingleLine(string branchString)
            {
                var splits = branchString.Split('>');
                Assert.AreEqual(splits.Length, 2);
                var parent = Create(splits[0].Trim());
                foreach (var nodeString in splits[1].Split('|'))
                {
                    var child = Create(nodeString.Trim());
                    parent.ChildrenList.Add(child);
                }
                return parent;
            }

            NodeDataBuilder Create(string nodeString)
            {
                var nameParamsArray = nodeString.Split(':');
                var name = nameParamsArray[0].Trim();
                var @params = nameParamsArray.Length >= 2 ? nameParamsArray[1].Trim() : "";
                return _nodeCreators[name](@params);
            }
        }
    }
}
