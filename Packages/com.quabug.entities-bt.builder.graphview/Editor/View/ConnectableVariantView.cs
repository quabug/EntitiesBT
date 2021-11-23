using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EntitiesBT.Variant;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class ConnectableVariantView : VisualElement, IVariantPortContainer
    {
        private readonly ConnectableVariant _variant;
        private Label _labelTitle;

        public string Title
        {
            get => _labelTitle.text;
            set => _labelTitle.text = value;
        }

        public IReadOnlyList<Port> Ports { get; }

        public ConnectableVariantView(ConnectableVariant variant)
        {
            _variant = variant;

            Type portType = null;
            if (typeof(IVariantReader).IsAssignableFrom(variant.VariantType)) portType = typeof(IVariantReader<>);
            else if (typeof(IVariantWriter).IsAssignableFrom(variant.VariantType)) portType = typeof(IVariantWriter<>);
            else if (typeof(IVariantReaderAndWriter).IsAssignableFrom(variant.VariantType)) portType = typeof(IVariantReaderAndWriter<>);
            var valueType = variant.Variant.FindValueType();
            if (portType == null || valueType == null) throw new NotImplementedException();
            portType = portType.MakeGenericType(valueType);

            var relativeDirectory = Core.Utilities.GetCurrentDirectoryProjectRelativePath();
            var uxmlPath = Path.Combine(relativeDirectory, "ConnectableVariantView.uxml");
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(this);

            _labelTitle = this.Q<Label>("title");

            Ports = new[]
            {
                Utilities.CreateVariantPort(Direction.Input, Port.Capacity.Single, portType),
                Utilities.CreateVariantPort(Direction.Output, Port.Capacity.Single, portType)
            };

            Ports[0].portName = "";
            Ports[0].AddToClassList(Utilities.VariantPortClass);
            Ports[0].AddToClassList(Utilities.VariantAccessMode(portType));
            this.Q<VisualElement>("left-port").Add(Ports[0]);

            Ports[1].portName = "";
            Ports[1].AddToClassList(Utilities.VariantPortClass);
            Ports[1].AddToClassList(Utilities.VariantAccessMode(portType));
            this.Q<VisualElement>("right-port").Add(Ports[1]);
        }

        public void Connect(Edge edge)
        {
            var (variantPort, syntaxNodePort) = DestructEdge(edge);
            var variantPortIndex = Ports.IndexOf(variantPort);
            GetSyntaxNodeView(edge).Connect(_variant, variantPortIndex, syntaxNodePort);
        }

        public void Disconnect(Edge edge)
        {
            GetSyntaxNodeView(edge).Disconnect(_variant);
        }

        public bool IsConnected(Edge edge)
        {
            return Ports.Any(port => edge.input == port || edge.output == port);
        }

        private SyntaxNodeView GetSyntaxNodeView(Edge edge)
        {
            return edge.input.node as SyntaxNodeView ?? edge.output.node as SyntaxNodeView;
        }

        private (Port variantPort, Port syntaxNodePort) DestructEdge(Edge edge)
        {
            if (Ports.Any(port => port == edge.input)) return (edge.input, edge.output);
            if (Ports.Any(port => port == edge.output)) return (edge.output, edge.input);
            throw new ArgumentException();
        }
    }
}