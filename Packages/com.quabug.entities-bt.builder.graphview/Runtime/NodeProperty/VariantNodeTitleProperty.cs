using System.Linq;
using GraphExt;
using GraphExt.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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

        public class Factory : SingleNodePropertyViewFactory<VariantNodeTitleProperty>
        {
            protected override VisualElement CreateView(Node node, VariantNodeTitleProperty property, INodePropertyViewFactory factory)
            {
                var container = new VisualElement();

                var label = factory.Create(node, property._titleProperty, factory).Single();
                var leftPort = new PortContainer(property._inputPortName);
                var rightPort = new PortContainer(property._outputPortName);

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
    }
}