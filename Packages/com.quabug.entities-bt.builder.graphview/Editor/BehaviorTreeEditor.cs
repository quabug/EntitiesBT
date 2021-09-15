using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class BehaviorTreeEditor : EditorWindow
    {
        [MenuItem("EntitiesBT/BehaviorTreeEditor")]
        public static void ShowEditor()
        {
            BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviorTreeEditor");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.quabug.entities-bt.builder.graphview/Editor/BehaviorTreeEditor.uxml");
            visualTree.CloneTree(root);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.quabug.entities-bt.builder.graphview/Editor/BehaviorTreeEditor.uss");
            root.styleSheets.Add(styleSheet);
        }
    }
}