using System.IO;
using GraphExt.Editor;
using UnityEditor;

namespace EntitiesBT.Editor
{
    public static class GraphWindowMenuItem
    {
        [MenuItem("EntitiesBT/Graph Editor")]
        public static void OpenGraphEditorWindow()
        {
            const string fileName = "Graph Window Config.asset";
            var filePath = Path.Combine(Core.Utilities.GetCurrentDirectoryProjectRelativePath(), fileName);
            var config = AssetDatabase.LoadAssetAtPath<GraphConfig>(filePath);
            config.OpenWindow();
        }
    }
}