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

        public NodePropertyView(SerializedProperty property)
        {
            var relativeDirectory = Utilities.GetCurrentDirectoryProjectRelativePath();
            var uxmlPath = Path.Combine(relativeDirectory, "NodePropertyView.uxml");
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(this);

            _labelTitle = this.Q<Label>("title");
            Title = property.name;

            LeftPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(IVariant));
            LeftPort.portName = "";
            LeftPort.AddToClassList("variant");
            this.Q<VisualElement>("left-port").Add(LeftPort);

            RightPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(IVariant));
            RightPort.portName = "";
            RightPort.AddToClassList("variant");
            this.Q<VisualElement>("right-port").Add(RightPort);
        }
    }
}