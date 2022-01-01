using System;
using System.Reflection;
using System.IO;
using System.Linq;
using EntitiesBT.Components;
using EntitiesBT.Core;
using GraphExt.Editor;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
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

    public class BehaviorTreeWindowExtension : PrefabStageWindowExtension<EntitiesBT.IBehaviorTreeNode, BehaviorTreeNodeComponent> {}

    public class BehaviorTreeCreationMenuEntry : IMenuEntry
    {
        public void MakeEntry(GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            var menuPosition = graph.viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            if (graph.Module is GameObjectHierarchyGraphViewModule<EntitiesBT.IBehaviorTreeNode, BehaviorTreeNodeComponent> module && PrefabStageUtility.GetCurrentPrefabStage() != null)
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
                        var id = Guid.NewGuid();
                        var node = new EntitiesBT.BehaviorTreeNode();
                        node.Data = new NodeAsset { NodeType = type.AssemblyQualifiedName };
                        module.AddGameObjectNode(id, node, menuPosition);
                    });
                }
            }
        }
    }
}
