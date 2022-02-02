using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using EntitiesBT.Core;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using Nuwa.Blob;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public static class GraphWindowMenuItem
    {
        [MenuItem("EntitiesBT/Graph Editor")]
        public static void OpenGraphEditorWindow()
        {
            const string fileName = "Graph Window Config.asset";
            var filePath = Path.Combine(Core.Utilities.GetCurrentDirectoryProjectRelativePath(), fileName);
            var config = AssetDatabase.LoadAssetAtPath<GraphConfig>(filePath);
            config.OpenWindow();
        }
    }

    public class BehaviorTreeWindowExtension : PrefabGraphWindowExtension<EntitiesBT.BehaviorTreeNode, BehaviorTreeNodeComponent> {}
    public class BehaviorTreeBasicGraphInstaller : BasicGraphInstaller<EntitiesBT.BehaviorTreeNode> {}
    public class BehaviorTreeSerializableInstaller : SerializableGraphBackendInstaller<EntitiesBT.BehaviorTreeNode, BehaviorTreeNodeComponent> {}

    public class BehaviorTreeSelectionMenuEntry : SelectionEntry<EntitiesBT.BehaviorTreeNode>
    {
        public BehaviorTreeSelectionMenuEntry([NotNull] GraphRuntime<EntitiesBT.BehaviorTreeNode> graph, [NotNull] IReadOnlyDictionary<Node, NodeId> nodes, [NotNull] IReadOnlyDictionary<Edge, EdgeId> edges) : base(graph, nodes, edges)
        {
        }
    }

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
                    from attribute in type.GetCustomAttributes<BehaviorNodeAttribute>()
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
