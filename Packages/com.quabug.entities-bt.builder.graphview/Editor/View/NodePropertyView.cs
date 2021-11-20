using System;
using System.IO;
using EntitiesBT.Variant;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class NodePropertyView : VisualElement
    {
        private readonly ConnectableVariant _variant;
        private Label _labelTitle;

        public string Title
        {
            get => _labelTitle.text;
            set => _labelTitle.text = value;
        }

        public Port LeftPort { get; }
        public Port RightPort { get; }

        public NodePropertyView(ConnectableVariant variant)
        {
            _variant = variant;

            Type portType = null;
            if (typeof(IVariantReader).IsAssignableFrom(variant.VariantType)) portType = typeof(IVariantReader<>);
            else if (typeof(IVariantWriter).IsAssignableFrom(variant.VariantType)) portType = typeof(IVariantWriter<>);
            else if (typeof(IVariantReaderAndWriter).IsAssignableFrom(variant.VariantType)) portType = typeof(IVariantReaderAndWriter<>);
            var valueType = variant.Variant.FindValueType();
            if (portType == null ||  valueType == null) throw new NotImplementedException();
            portType = portType.MakeGenericType(valueType);

            var relativeDirectory = Core.Utilities.GetCurrentDirectoryProjectRelativePath();
            var uxmlPath = Path.Combine(relativeDirectory, "NodePropertyView.uxml");
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(this);

            _labelTitle = this.Q<Label>("title");

            LeftPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, portType);
            LeftPort.portName = "";
            LeftPort.AddToClassList(Utilities.VariantPortClass);
            LeftPort.AddToClassList(Utilities.VariantAccessMode(portType));
            this.Q<VisualElement>("left-port").Add(LeftPort);

            RightPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, portType);
            RightPort.portName = "";
            RightPort.AddToClassList(Utilities.VariantPortClass);
            RightPort.AddToClassList(Utilities.VariantAccessMode(portType));

            this.Q<VisualElement>("right-port").Add(RightPort);
        }

        public void ConnectTo(SyntaxNodeView node)
        {
            node.Connect(_variant);
        }

        public void DisconnectFrom(SyntaxNodeView node)
        {
            node.Disconnect(_variant);
        }

        public bool IsConnected(Edge edge)
        {
            return edge.input == LeftPort || edge.input == RightPort || edge.output == LeftPort || edge.output == RightPort;
        }
    }
}