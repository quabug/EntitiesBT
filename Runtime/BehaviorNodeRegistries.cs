using EntitiesBT.Nodes;

namespace EntitiesBT
{
    public static class BehaviorNodeRegistries
    {
        public static void RegisterCommonNodes(this BehaviorNodeFactory factory)
        {
            factory.Register<SequenceNode>(() => new SequenceNode());
            factory.Register<SelectorNode>(() => new SelectorNode());
            factory.Register<ParallelNode>(() => new ParallelNode());
            
            factory.Register<ResetChildrenNode>(() => new ResetChildrenNode());
            factory.Register<ResetDescendantsNode>(() => new ResetDescendantsNode());
            
            factory.Register<SuccessNode>(() => new SuccessNode());
            factory.Register<FailureNode>(() => new FailureNode());
            factory.Register<RunningNode>(() => new RunningNode());
        }
    }
}
