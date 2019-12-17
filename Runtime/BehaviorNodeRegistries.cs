using EntitiesBT.Core;
using EntitiesBT.Nodes;

namespace EntitiesBT
{
    public class BehaviorNodeRegistries
    {
        public BehaviorNodeRegistries(BehaviorNodeFactory factory, VirtualMachine vm)
        {
            factory.Register<SequenceNode>(() => new SequenceNode());
            factory.Register<SelectorNode>(() => new SelectorNode());
            factory.Register<ParallelNode>(() => new ParallelNode());
            
            factory.Register<ResetChildrenNode>(() => new ResetChildrenNode());
            factory.Register<ResetDescendantsNode>(() => new ResetDescendantsNode());
        }
    }
}
