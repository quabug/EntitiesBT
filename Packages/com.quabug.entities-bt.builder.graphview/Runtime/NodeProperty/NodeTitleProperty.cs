using System.Linq;
using GraphExt;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace EntitiesBT
{
    public class NodeTitleProperty : INodeProperty
    {
        public int Order { get; set; } = 0;
        [NotNull] public INodeProperty TitleProperty { get; set; }
        [CanBeNull] public INodeProperty ToggleProperty { get; set; }

#if UNITY_EDITOR
        public class ViewFactory : GraphExt.Editor.SingleNodePropertyViewFactory<NodeTitleProperty>
        {
            protected override VisualElement CreateView(Node node, NodeTitleProperty property, GraphExt.Editor.INodePropertyViewFactory factory)
            {
                var view = new VisualElement();
                view.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                view.style.alignItems = new StyleEnum<Align>(Align.Center);
                view.style.alignSelf = new StyleEnum<Align>(Align.Center);
                view.name = "node-title-property";

                foreach (var titleView in factory.Create(node, property.TitleProperty, factory))
                {
                    titleView.style.marginTop = new StyleLength(0f);
                    titleView.style.marginBottom = new StyleLength(0f);
                    titleView.style.marginLeft = new StyleLength(0f);
                    titleView.style.marginRight = new StyleLength(0f);
                    view.Add(titleView);
                }
                var toggleView = factory.Create(node, property.ToggleProperty, factory).SingleOrDefault();
                if (toggleView != null) view.Add(toggleView);

                return view;
            }
        }
#endif
    }
}