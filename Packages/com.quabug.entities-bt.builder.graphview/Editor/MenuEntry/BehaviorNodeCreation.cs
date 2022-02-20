using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using Nuwa.Blob;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    [UsedImplicitly]
    public class BehaviorNodeCreation : IMenuEntry
    {
        private readonly IReadOnlyDictionary<NodeId, GraphNodeComponent> _nodes;
        private readonly GraphRuntime<GraphNode> _graphRuntime;

        public BehaviorNodeCreation(GraphRuntime<GraphNode> graphRuntime, IReadOnlyDictionary<NodeId, GraphNodeComponent> nodes)
        {
            _graphRuntime = graphRuntime;
            _nodes = nodes;
        }

        public void MakeEntry(UnityEditor.Experimental.GraphView.GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (PrefabStageUtility.GetCurrentPrefabStage() == null) return;

            var menuPosition = graph.viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            var types = TypeCache.GetTypesWithAttribute<BehaviorNodeAttribute>();
            foreach (var (type, attribute) in (
                         from type in types
                         from attribute in type.GetCustomAttributes<BehaviorNodeAttribute>()
                         where !attribute.Ignore
                         select (type, attribute)
                     ).OrderBy(t => t.type.Name))
            {
                var path = $"{attribute.Type}/{type.Name}";
                menu.AddItem(new GUIContent(path), false, () =>
                {
                    if (stage != PrefabStageUtility.GetCurrentPrefabStage()) return;

                    var id = Guid.NewGuid();
                    var node = new BehaviorNode();
                    node.Blob = new DynamicBlobDataBuilder { BlobDataType = type.AssemblyQualifiedName };
                    BuilderUtility.SetBlobDataType(type, ref node.Blob.Builders, ref node.Blob.FieldNames);
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
