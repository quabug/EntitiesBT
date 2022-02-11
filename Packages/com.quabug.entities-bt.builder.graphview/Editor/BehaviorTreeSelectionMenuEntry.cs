using System.Collections.Generic;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace EntitiesBT.Editor
{
    public class BehaviorTreeSelectionMenuEntry : SelectionEntry<EntitiesBT.BehaviorTreeNode>
    {
        public BehaviorTreeSelectionMenuEntry([NotNull] GraphRuntime<EntitiesBT.BehaviorTreeNode> graph, [NotNull] IReadOnlyDictionary<Node, NodeId> nodes, [NotNull] IReadOnlyDictionary<Edge, EdgeId> edges) : base(graph, nodes, edges)
        {
        }
    }
}
