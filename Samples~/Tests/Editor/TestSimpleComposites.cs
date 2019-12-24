using System;
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
            var vm = CreateVM("!seq>yes|yes|yes|no|yes");
            var nodes = vm.BehaviorNodes.Skip(1).Cast<TestNode>().ToArray();
            
            Assert.AreEqual(vm.Tick(), NodeState.Failure);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 1, 1, 1, 0 });
            Assert.Catch<IndexOutOfRangeException>(() => vm.Tick());
            
            vm.ResetAll();
            Assert.AreEqual(vm.Tick(), NodeState.Failure);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 2, 2, 2, 2, 2 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 2, 2, 2, 2, 0 });
        }
        
        [Test]
        public void should_run_select_until_reach_success_node()
        {
            var vm = CreateVM("!sel>no|no|no|yes|no");
            var nodes = vm.BehaviorNodes.Skip(1).Cast<TestNode>().ToArray();
            
            Assert.AreEqual(vm.Tick(), NodeState.Success);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 1, 1, 1, 0 });
            Assert.Catch<IndexOutOfRangeException>(() => vm.Tick());
            
            vm.ResetAll();
            Assert.AreEqual(vm.Tick(), NodeState.Success);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 2, 2, 2, 2, 2 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 2, 2, 2, 2, 0 });
        }
        
        [Test]
        public void should_run_all_children_of_parallel()
        {
            var vm = CreateVM("!par>yes|no|yes|yes|no");
            var nodes = vm.BehaviorNodes.Skip(1).Cast<TestNode>().ToArray();
            
            Assert.AreEqual(vm.Tick(), NodeState.Failure);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.Catch<IndexOutOfRangeException>(() => vm.Tick());
            
            vm.ResetAll();
            Assert.AreEqual(vm.Tick(), NodeState.Failure);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 2, 2, 2, 2, 2 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 2, 2, 2, 2, 2 });
        }

        [Test]
        public void should_stay_on_running_node_for_select()
        {
            var vm = CreateVM("!sel>no|run|no|no|no");
            var nodes = vm.BehaviorNodes.Skip(1).Cast<TestNode>().ToArray();
            
            Assert.AreEqual(vm.Tick(), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 1, 0, 0, 0 });
            
            Assert.AreEqual(vm.Tick(), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 2, 0, 0, 0 });

            nodes[1].State = NodeState.Failure;
            Assert.AreEqual(vm.Tick(), NodeState.Failure);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 3, 1, 1, 1 });
            Assert.Catch<IndexOutOfRangeException>(() => vm.Tick());
            
            vm.ResetAll();
            Assert.AreEqual(vm.Tick(), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 2, 2, 2, 2, 2 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 2, 4, 1, 1, 1 });
        }

        [Test]
        public void should_stay_on_running_node_for_sequence()
        {
            var vm = CreateVM("!seq>yes|run|yes|yes|yes");
            var nodes = vm.BehaviorNodes.Skip(1).Cast<TestNode>().ToArray();
            
            Assert.AreEqual(vm.Tick(), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 1, 0, 0, 0 });
            
            Assert.AreEqual(vm.Tick(), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 2, 0, 0, 0 });

            nodes[1].State = NodeState.Success;
            Assert.AreEqual(vm.Tick(), NodeState.Success);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 3, 1, 1, 1 });
            Assert.Catch<IndexOutOfRangeException>(() => vm.Tick());
            
            vm.ResetAll();
            Assert.AreEqual(vm.Tick(), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 2, 2, 2, 2, 2 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 2, 4, 1, 1, 1 });
        }
        
        [Test]
        public void should_stay_on_running_node_for_parallel()
        {
            var vm = CreateVM("!par>no|run|yes|run|no");
            var nodes = vm.BehaviorNodes.Skip(1).Cast<TestNode>().ToArray();
            
            Assert.AreEqual(vm.Tick(), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 1, 1, 1, 1 });
            
            nodes[1].State = NodeState.Success;
            Assert.AreEqual(vm.Tick(), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 2, 1, 2, 1 });

            nodes[3].State = NodeState.Failure;
            Assert.AreEqual(vm.Tick(), NodeState.Failure);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 1, 2, 1, 3, 1 });
            Assert.Catch<IndexOutOfRangeException>(() => vm.Tick());
            
            vm.ResetAll();
            Assert.AreEqual(vm.Tick(), NodeState.Running);
            Assert.AreEqual(nodes.Select(n => n.InitializeTimes), new [] { 1, 1, 1, 1, 1 });
            Assert.AreEqual(nodes.Select(n => n.ResetTimes), new [] { 2, 2, 2, 2, 2 });
            Assert.AreEqual(nodes.Select(n => n.TickTimes), new [] { 2, 3, 2, 4, 2 });
        }
    }
}
