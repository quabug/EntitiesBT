using EntitiesBT.Nodes;

namespace EntitiesBT
{
    public class BehaviorNodeRegistries
    {
        public BehaviorNodeRegistries(BehaviorNodeFactory factory)
        {
            factory.Register<SequenceNode>(() => new SequenceNode());
            factory.Register<SelectorNode>(() => new SelectorNode());
            factory.Register<ParallelNode>(() => new ParallelNode());
            
            factory.Register<ResetChildrenNode>(() => new ResetChildrenNode());
            factory.Register<ResetDescendantsNode>(() => new ResetDescendantsNode());
            
            factory.Register<SuccessionNode>(() => new SuccessNode());
            factory.Register<FailureNode>(() => new FailureNode());
            factory.Register<RunningNode>(() => new RunningNode());
        }
    }
}
