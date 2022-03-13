using System;
using System.Collections.Generic;
using System.Linq;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    [UsedImplicitly]
    public class VariantNodeCreation : IMenuEntry
    {
        private readonly IReadOnlyDictionary<NodeId, GraphNodeComponent> _nodes;
        private readonly GraphRuntime<GraphNode> _graphRuntime;

        public VariantNodeCreation(GraphRuntime<GraphNode> graphRuntime, IReadOnlyDictionary<NodeId, GraphNodeComponent> nodes)
        {
            _graphRuntime = graphRuntime;
            _nodes = nodes;
        }

        public void MakeEntry(UnityEditor.Experimental.GraphView.GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage == null) return;

            var menuPosition = graph.viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            foreach (var type in TypeCache.GetTypesDerivedFrom<VariantNode>().OrderBy(type => type.Name))
            {
                var path = $"{nameof(VariantNode)}/{type.Name}";
                menu.AddItem(new GUIContent(path), false, () =>
                {
                    if (stage != PrefabStageUtility.GetCurrentPrefabStage()) return;

                    var id = Guid.NewGuid();
                    var node = (VariantNode)Activator.CreateInstance(type);
                    _graphRuntime.AddNode(id, node);
                    var component = _nodes[id];
                    component.Position = menuPosition;
                    component.name = node.Name;
                    EditorSceneManager.MarkSceneDirty(stage.scene);
                });
            }
        }
    }
}
