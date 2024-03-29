using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Nodes;
using NUnit.Framework;
using UnityEngine;

namespace EntitiesBT.Test
{
    public class BehaviorTreeTestBase
    {
        class Sequence : BTNode<SequenceNode> {}
        class Selector : BTNode<SelectorNode> {}
        class Parallel : BTNode<ParallelNode> {}

        private readonly Dictionary<string, Func<string, BTNode>> _nodeCreators = new Dictionary<string, Func<string, BTNode>>
        {
            { "seq", Create<Sequence> }
          , { "sel", Create<Selector> }
          , { "par", Create<Parallel> }
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

        protected NodeBlobRef CreateBlob(string tree)
        {
            var root = CreateBTNode(tree).GetComponent<BTNode>();
            var blob = root.ToBlob(root.FindScopeValuesList());
            return new NodeBlobRef(blob);
        }
    
        private static BTNode CreateA(string @params)
        {
            var nodeA = Create<BTTestNodeA>();
            nodeA.A = int.Parse(@params);
            return nodeA;
        }
        
        private static BTNode CreateB(string @params)
        {
            var nodeB = Create<BTTestNodeB>();
            var paramArray = @params.Split(',');
            nodeB.B = int.Parse(paramArray[0].Trim());
            nodeB.BB = int.Parse(paramArray[1].Trim());
            return nodeB;
        }

        private static Func<string, BTNode> CreateTestNode(NodeState state)
        {
            return @params =>
            {
                var testNodeState = Create<BTTestNodeState>();
                testNodeState.State = state;
                return testNodeState;
            };
        }

        private static T Create<T>(string @params = "") where T : BTNode
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
        protected GameObject CreateBTNode(string branch)
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
    }
}
