using System.Collections.Generic;
using System.Linq;
using GraphExt;
using GraphExt.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class GraphViewSelectionEntry : IMenuEntry
    {
        private readonly GraphRuntime<EntitiesBT.BehaviorTreeNode> _behaviorGraph;
        private readonly GraphRuntime<VariantNode> _variantGraph;
        private readonly IReadOnlyDictionary<Node, NodeId> _nodes;
        private readonly IReadOnlyDictionary<Edge, EdgeId> _edges;

        public GraphViewSelectionEntry(
            GraphRuntime<EntitiesBT.BehaviorTreeNode> behaviorGraph,
            GraphRuntime<VariantNode> variantGraph,
            IReadOnlyDictionary<Node, NodeId> nodes,
            IReadOnlyDictionary<Edge, EdgeId> edges
        )
        {
            _behaviorGraph = behaviorGraph;
            _variantGraph = variantGraph;
            _nodes = nodes;
            _edges = edges;
        }

        public void MakeEntry(UnityEditor.Experimental.GraphView.GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            var edges = graph.selection?.OfType<Edge>();
            var nodes = graph.selection?.OfType<Node>();
            if (edges != null && edges.Any() || nodes != null && nodes.Any())
            {
                menu.AddItem(new GUIContent("Delete"), false, () =>
                {
                    foreach (var edge in edges.ToArray())
                    {
                        var edgeId = _edges[edge];
                        if (_behaviorGraph.Edges.Contains(edgeId))
                            _behaviorGraph.Disconnect(input: edgeId.Input, output: edgeId.Output);
                        if (_variantGraph.Edges.Contains(edgeId))
                            _variantGraph.Disconnect(input: edgeId.Input, output: edgeId.Output);
                    }

                    foreach (var node in nodes.ToArray())
                    {
                        var nodeId = _nodes[node];
                        _behaviorGraph.DeleteNode(nodeId);
                        _variantGraph.DeleteNode(nodeId);
                    }
                });
                menu.AddSeparator("");
            }
        }
    }
}