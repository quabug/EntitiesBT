using UnityEditor.Experimental.GraphView;

namespace EntitiesBT.Editor
{
    public class VariantView : Node
    {
        private ISyntaxTreeNode _variant;
        private BehaviorTreeView _graph;

        public VariantView(BehaviorTreeView graph, ISyntaxTreeNode variant)
            // : base(Path.Combine(Utilities.GetCurrentDirectoryProjectRelativePath(), "NodeView.uxml"))
        {
            _variant = variant;
            _graph = graph;

            style.left = variant.Position.x;
            style.top = variant.Position.y;
        }
    }
}