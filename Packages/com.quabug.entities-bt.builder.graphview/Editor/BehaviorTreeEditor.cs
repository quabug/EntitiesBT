using System.IO;
using EntitiesBT.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class BehaviorTreeEditor : EditorWindow
    {
        static BehaviorTreeEditor()
        {
            PrefabStage.prefabStageOpened -= ResetEditorView;
            PrefabStage.prefabStageOpened += ResetEditorView;

            PrefabStage.prefabStageClosing -= ClearEditorView;
            PrefabStage.prefabStageClosing += ClearEditorView;

            void ClearEditorView(PrefabStage closingStage)
            {
                var currentStage = PrefabStageUtility.GetCurrentPrefabStage();
                // do NOT clear editor if current stage is not closing
                ResetEditorView(currentStage == closingStage ? null : currentStage);
            }
        }

        private BehaviorTreeView _view => rootVisualElement.Q<BehaviorTreeView>();

        [MenuItem("EntitiesBT/BehaviorTreeEditor")]
        public static void ShowEditor()
        {
            GetWindow();
            ResetEditorView();
        }

        public void CreateGUI()
        {
            var relativeDirectory = Utilities.GetCurrentDirectoryProjectRelativePath();
            var uxmlPath = Path.Combine(relativeDirectory, "BehaviorTreeEditor.uxml");
            var ussPath = Path.Combine(relativeDirectory, "BehaviorTreeEditor.uss");

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(rootVisualElement);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
            rootVisualElement.styleSheets.Add(styleSheet);

            var miniMap = rootVisualElement.Q<MiniMap>();
            var graph = _view;
            if (miniMap != null && graph != null) miniMap.graphView = graph;

            ResetEditorView();
        }

        private static BehaviorTreeEditor GetWindow()
        {
            return GetWindow<BehaviorTreeEditor>("BehaviorTree");
        }

        private static void ResetEditorView()
        {
            ResetEditorView(PrefabStageUtility.GetCurrentPrefabStage());
        }

        private static void ResetEditorView([CanBeNull] PrefabStage prefabStage)
        {
            if (!HasOpenInstances<BehaviorTreeEditor>()) return;

            var window = GetWindow();
            BehaviorTreeGraph graph = null;
            if (prefabStage != null)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabStage.assetPath);
                graph = new BehaviorTreeGraph(prefab, prefabStage);
            }
            window.rootVisualElement.Q<BehaviorTreeView>().Reset(graph);
        }
    }
}