using System.IO;
using EntitiesBT.Core;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

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

        public Port LeftPort
        {
            get =>
        }

        public NodePropertyView(SerializedProperty property)
        {
            var relativeDirectory = Utilities.GetCurrentDirectoryProjectRelativePath();
            var uxmlPath = Path.Combine(relativeDirectory, "NodePropertyView.uxml");
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(this);

            _labelTitle = this.Q<Label>("title");
        }
    }
}