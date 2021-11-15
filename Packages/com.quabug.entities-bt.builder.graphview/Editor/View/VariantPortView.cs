using System;
using System.IO;
using EntitiesBT.Variant;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Utilities = EntitiesBT.Core.Utilities;

namespace EntitiesBT.Editor
{
    public class NodePropertyView : VisualElement
    {
        private Label _labelTitle;

        public string Title
        {
            get => _labelTitle.text;
            set => _labelTitle.text = value;
        }

        public Port LeftPort { get; }
        public Port RightPort { get; }

        public NodePropertyView(Type type, Direction direction = Direction.Input)
        {
            var relativeDirectory = Utilities.GetCurrentDirectoryProjectRelativePath();
            var uxmlPath = Path.Combine(relativeDirectory, "NodePropertyView.uxml");
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(this);

            _labelTitle = this.Q<Label>("title");

            LeftPort = Port.Create<Edge>(Orientation.Horizontal, direction, Port.Capacity.Multi, type);
            LeftPort.portName = "";
            LeftPort.AddToClassList("variant");
            this.Q<VisualElement>("left-port").Add(LeftPort);

            RightPort = Port.Create<Edge>(Orientation.Horizontal, direction, Port.Capacity.Multi, type);
            RightPort.portName = "";
            RightPort.AddToClassList("variant");
            this.Q<VisualElement>("right-port").Add(RightPort);
        }
    }
}