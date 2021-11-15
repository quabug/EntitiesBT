using System.IO;
using EntitiesBT.Core;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace EntitiesBT.Editor
{
    public sealed class SyntaxNodeView : Node, INodeView
    {
        public ISyntaxTreeNode Node { get; }
        private BehaviorTreeView _graph;

        public SyntaxNodeView(BehaviorTreeView graph, ISyntaxTreeNode node)
            : base(Path.Combine(Utilities.GetCurrentDirectoryProjectRelativePath(), "SyntaxNodeView.uxml"))
        {
            Node = node;
            _graph = graph;

            style.left = node.Position.x;
            style.top = node.Position.y;

            SetName(node.Name);
            this.TrackPropertyValue(node.Name, SetName);

            void SetName(SerializedProperty nameProperty) => title = nameProperty.stringValue;
        }

        public void SyncPosition()
        {
            Node.Position = GetPosition().position;
        }

        public void Dispose()
        {
            Node.Dispose();
        }
    }
}