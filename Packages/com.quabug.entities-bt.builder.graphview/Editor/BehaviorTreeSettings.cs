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

    public class BehaviorTreeWindowExtension : PrefabGraphWindowExtension<IGraphNode, GraphNodeComponent> {}
    public class BehaviorTreeBasicGraphInstaller : BasicGraphInstaller<IGraphNode> {}
    public class BehaviorTreeSerializableInstaller : SerializableGraphBackendInstaller<IGraphNode, GraphNodeComponent> {}

    public class BehaviorTreeSelectionMenuEntry : SelectionEntry<IGraphNode>
    {
        public BehaviorTreeSelectionMenuEntry([NotNull] GraphRuntime<IGraphNode> graph, [NotNull] IReadOnlyDictionary<Node, NodeId> nodes, [NotNull] IReadOnlyDictionary<Edge, EdgeId> edges) : base(graph, nodes, edges)
        {
        }
    }

    public class BehaviorTreeCreationMenuEntry : IMenuEntry
    {
        private readonly IReadOnlyDictionary<NodeId, GraphNodeComponent> _nodes;
        private readonly GraphRuntime<IGraphNode> _graphRuntime;

        public BehaviorTreeCreationMenuEntry(
            GraphRuntime<IGraphNode> graphRuntime,
            IReadOnlyDictionary<NodeId, GraphNodeComponent> nodes
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
                        _nodes[id].Position = menuPosition;
                        EditorSceneManager.MarkSceneDirty(stage.scene);
                    });
                }
            }
        }
    }
}
