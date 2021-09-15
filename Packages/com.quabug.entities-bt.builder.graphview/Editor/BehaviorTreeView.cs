using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class BehaviorTreeView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviorTreeView, UxmlTraits> {}

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

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var types = TypeCache.GetTypesWithAttribute<BehaviorNodeAttribute>();
            foreach (var (type, attribute) in
                from type in types
                from attribute in type.GetCustomAttributes<BehaviorNodeAttribute>()
                where !attribute.Ignore
                select (type, attribute))
            {
                evt.menu.AppendAction($"{attribute.Type}/{type.Name}", _ => AddElement(new NodeView()));
            }
        }
    }
}