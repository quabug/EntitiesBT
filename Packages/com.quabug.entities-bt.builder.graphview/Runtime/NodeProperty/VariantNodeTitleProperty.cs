using System.Linq;
using GraphExt;
using UnityEngine.UIElements;

namespace EntitiesBT
{
    public class VariantNodeTitleProperty : INodeProperty
    {
        public int Order { get; set; } = 0;

        private readonly INodeProperty _titleProperty;
        private readonly string _inputPortName;
        private readonly string _outputPortName;

        public VariantNodeTitleProperty(INodeProperty titleProperty, string inputPortName, string outputPortName)
        {
            _titleProperty = titleProperty;
            _inputPortName = inputPortName;
            _outputPortName = outputPortName;
        }

#if UNITY_EDITOR
        public class ViewFactory : GraphExt.Editor.SingleNodePropertyViewFactory<VariantNodeTitleProperty>
        {
            protected override VisualElement CreateView(UnityEditor.Experimental.GraphView.Node node, VariantNodeTitleProperty property, GraphExt.Editor.INodePropertyViewFactory factory)
            {
                var container = new VisualElement();

                var label = factory.Create(node, property._titleProperty, factory).Single();
                var leftPort = new GraphExt.Editor.PortContainer(property._inputPortName);
                var rightPort = new GraphExt.Editor.PortContainer(property._outputPortName);

                container.name = "variant-node-title-property";
                container.style.flexDirection = FlexDirection.Row;

                label.name = "title-property";

                leftPort.name = "input-port";
                leftPort.style.alignSelf = Align.Center;

                rightPort.name = "output-port";
                rightPort.style.alignSelf = Align.Center;

                container.Add(leftPort);
                container.Add(label);
                container.Add(rightPort);

                return container;
            }
        }
#endif
    }
}