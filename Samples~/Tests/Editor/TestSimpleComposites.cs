using System.Linq;
using EntitiesBT.Core;
using NUnit.Framework;

namespace EntitiesBT.Test
{
    public class TestSimpleComposites : BehaviorTreeTestBase
    {
        [Test]
        public void should_run_sequence_until_reach_failure_node()
        {
            var blobRef = CreateBlob("!seq>yes|yes|yes|no|yes");
            var nodes = Enumerable.Range(1, 5).Select(i => blobRef.GetNodeData<TestNode>(i));
            
            VirtualMachine.Reset(blobRef, _blackboard);
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Failure);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 1, 1, 1, 0 });
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), (NodeState)0);
            
            VirtualMachine.Reset(blobRef, _blackboard);
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Failure);
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), (NodeState)0);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 2, 2, 2, 2, 2 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 2, 2, 2, 2, 0 });
        }
        
        [Test]
        public void should_run_select_until_reach_success_node()
        {
            var blobRef = CreateBlob("!sel>no|no|no|yes|no");
            var nodes = Enumerable.Range(1, 5).Select(i => blobRef.GetNodeData<TestNode>(i));
            
            VirtualMachine.Reset(blobRef, _blackboard);
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Success);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 1, 1, 1, 0 });
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), (NodeState)0);
            
            VirtualMachine.Reset(blobRef, _blackboard);
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Success);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 2, 2, 2, 2, 2 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 2, 2, 2, 2, 0 });
        }
        
        [Test]
        public void should_run_all_children_of_parallel()
        {
            var blobRef = CreateBlob("!par>yes|no|yes|yes|no");
            var nodes = Enumerable.Range(1, 5).Select(i => blobRef.GetNodeData<TestNode>(i));
            
            VirtualMachine.Reset(blobRef, _blackboard);
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Failure);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), (NodeState)0);
            
            VirtualMachine.Reset(blobRef, _blackboard);
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Failure);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 2, 2, 2, 2, 2 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 2, 2, 2, 2, 2 });
        }

        [Test]
        public void should_stay_on_running_node_for_select()
        {
            var blobRef = CreateBlob("!sel>no|run|no|no|no");
            var nodes = Enumerable.Range(1, 5).Select(i => blobRef.GetNodeData<TestNode>(i));
            
            VirtualMachine.Reset(blobRef, _blackboard);
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 1, 0, 0, 0 });
            
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 2, 0, 0, 0 });

            blobRef.GetNodeData<TestNode>(2).State = NodeState.Failure;
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Failure);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 3, 1, 1, 1 });
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), (NodeState)0);
            
            VirtualMachine.Reset(blobRef, _blackboard);
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 2, 2, 2, 2, 2 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 2, 4, 1, 1, 1 });
        }

        [Test]
        public void should_stay_on_running_node_for_sequence()
        {
            var blobRef = CreateBlob("!seq>yes|run|yes|yes|yes");
            var nodes = Enumerable.Range(1, 5).Select(i => blobRef.GetNodeData<TestNode>(i));
            
            VirtualMachine.Reset(blobRef, _blackboard);
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 1, 0, 0, 0 });
            
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 2, 0, 0, 0 });

            blobRef.GetNodeData<TestNode>(2).State = NodeState.Success;
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Success);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 3, 1, 1, 1 });
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), (NodeState)0);
            
            VirtualMachine.Reset(blobRef, _blackboard);
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 2, 2, 2, 2, 2 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 2, 4, 1, 1, 1 });
        }
        
        [Test]
        public void should_stay_on_running_node_for_parallel()
        {
            var blobRef = CreateBlob("!par>no|run|yes|run|no");
            var nodes = Enumerable.Range(1, 5).Select(i => blobRef.GetNodeData<TestNode>(i));
            
            VirtualMachine.Reset(blobRef, _blackboard);
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 1, 1, 1, 1 });

            blobRef.GetNodeData<TestNode>(2).State = NodeState.Success;
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 2, 1, 2, 1 });

            blobRef.GetNodeData<TestNode>(4).State = NodeState.Failure;
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Failure);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 2, 1, 3, 1 });
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), (NodeState)0);
            
            VirtualMachine.Reset(blobRef, _blackboard);
            Assert.AreEqual(VirtualMachine.Tick(blobRef, _blackboard), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 2, 2, 2, 2, 2 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 2, 3, 2, 4, 2 });
        }
    }
}
