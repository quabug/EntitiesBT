using System;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class BehaviorTreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviorTreeView, UxmlTraits> {}

        private IBehaviorTreeGraph _graph;

        public BehaviorTreeView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.quabug.entities-bt.builder.graphview/Editor/BehaviorTreeEditor.uss");
            styleSheets.Add(styleSheet);
        }

        public void Reset([CanBeNull] IBehaviorTreeGraph graph)
        {
            graphViewChanged -= OnGraphChanged;

            _graph = graph;
            DeleteElements(graphElements.ToList());

            graphViewChanged += OnGraphChanged;

            if (_graph != null)
            {
                Debug.Log($"open behavior tree graph: {_graph.Name}");
                foreach (var node in _graph) CreateNode(node);
            }

            GraphViewChange OnGraphChanged(GraphViewChange @event)
            {
                foreach (var moved in @event.movedElements)
                {
                    if (moved is NodeView node)
                        _graph.MoveNode(node.Id, node.Position);
                }
                return @event;
            }
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var types = TypeCache.GetTypesWithAttribute<BehaviorNodeAttribute>();
            foreach (var (type, attribute) in
                from type in types
                from attribute in type.GetCustomAttributes<BehaviorNodeAttribute>()
                where !attribute.Ignore
                select (type, attribute))
            {
                evt.menu.AppendAction($"{attribute.Type}/{type.Name}", AddNode(type, attribute, evt.localMousePosition));
            }

            Action<DropdownMenuAction> AddNode(Type type, BehaviorNodeAttribute attribute, Vector2 position)
            {
                return action =>
                {
                    var node = _graph.AddNode(type, position);
                    CreateNode(node);
                };
            }
        }

        private void CreateNode(in BehaviorTreeGraphNode node)
        {
            AddElement(new NodeView(node));
        }
    }
}