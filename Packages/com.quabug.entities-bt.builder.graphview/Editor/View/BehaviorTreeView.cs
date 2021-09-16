using System;
using System.Collections.Generic;
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
                if (@event.elementsToRemove != null)
                {
                    foreach (var node in @event.elementsToRemove.OfType<NodeView>()) node.Dispose();
                }
                return @event;
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if (port.direction != startPort.direction && port.node != startPort.node && port.portType == startPort.portType)
                    compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (selection != null && selection.Any())
            {
                evt.menu.AppendAction("Delete", _ => DeleteSelection());
                evt.menu.AppendSeparator();
            }

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

        private void CreateNode(IBehaviorTreeNode node)
        {
            AddElement(new NodeView(node));
        }
    }
}