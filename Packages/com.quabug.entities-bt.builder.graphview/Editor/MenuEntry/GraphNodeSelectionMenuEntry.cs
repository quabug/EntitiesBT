using System.Collections.Generic;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace EntitiesBT.Editor
{
    public class GraphNodeSelectionMenuEntry : SelectionEntry<GraphNode>
    {
        public GraphNodeSelectionMenuEntry([NotNull] GraphRuntime<GraphNode> graph, [NotNull] IReadOnlyDictionary<Node, NodeId> nodes, [NotNull] IReadOnlyDictionary<Edge, EdgeId> edges) : base(graph, nodes, edges)
        {
        }
    }
}
