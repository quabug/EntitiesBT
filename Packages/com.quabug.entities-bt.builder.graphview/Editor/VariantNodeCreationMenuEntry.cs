using System;
using System.Collections.Generic;
using System.Linq;
using GraphExt;
using GraphExt.Editor;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class VariantNodeCreationMenuEntry : IMenuEntry
    {
        private readonly IReadOnlyDictionary<NodeId, VariantNodeComponent> _nodes;
        private readonly GraphRuntime<VariantNode> _graphRuntime;

        public VariantNodeCreationMenuEntry(
            GraphRuntime<VariantNode> graphRuntime,
            IReadOnlyDictionary<NodeId, VariantNodeComponent> nodes
        )
        {
            _graphRuntime = graphRuntime;
            _nodes = nodes;
        }

        public void MakeEntry(UnityEditor.Experimental.GraphView.GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            var menuPosition = graph.viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                foreach (var type in TypeCache.GetTypesDerivedFrom<VariantNode>().OrderBy(type => type.Name))
                {
                    var path = $"{nameof(VariantNode)}/{type.Name}";
                    menu.AddItem(new GUIContent(path), false, () =>
                    {
                        var stage = PrefabStageUtility.GetCurrentPrefabStage();
                        if (stage == null) return;

                        var id = Guid.NewGuid();
                        var node = (VariantNode)Activator.CreateInstance(type);
                        _graphRuntime.AddNode(id, node);
                        var component = _nodes[id];
                        component.Position = menuPosition;
                        component.name = type.Name;
                        EditorSceneManager.MarkSceneDirty(stage.scene);
                    });
                }
            }
        }
    }
}