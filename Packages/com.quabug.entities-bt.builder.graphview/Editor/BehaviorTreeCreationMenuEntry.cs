using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using GraphExt;
using GraphExt.Editor;
using Nuwa.Blob;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class BehaviorTreeCreationMenuEntry : IMenuEntry
    {
        private readonly IReadOnlyDictionary<NodeId, BehaviorTreeNodeComponent> _nodes;
        private readonly GraphRuntime<EntitiesBT.BehaviorTreeNode> _graphRuntime;

        public BehaviorTreeCreationMenuEntry(
            GraphRuntime<EntitiesBT.BehaviorTreeNode> graphRuntime,
            IReadOnlyDictionary<NodeId, BehaviorTreeNodeComponent> nodes
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
                var types = TypeCache.GetTypesWithAttribute<BehaviorNodeAttribute>();
                foreach (var (type, attribute) in (
                             from type in types
                             from attribute in CustomAttributeExtensions.GetCustomAttributes<BehaviorNodeAttribute>((MemberInfo)type)
                             where !attribute.Ignore
                             select (type, attribute)
                         ).OrderBy(t => t.type.Name))
                {
                    var path = $"{attribute.Type}/{type.Name}";
                    menu.AddItem(new GUIContent(path), false, () =>
                    {
                        var stage = PrefabStageUtility.GetCurrentPrefabStage();
                        if (stage == null) return;

                        var id = Guid.NewGuid();
                        var node = new EntitiesBT.BehaviorTreeNode();
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
}