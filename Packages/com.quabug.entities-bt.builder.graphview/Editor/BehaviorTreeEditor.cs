using System.IO;
using JetBrains.Annotations;
using UnityEditor;
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

            void ClearEditorView(PrefabStage _) => ResetEditorView(null);
        }

        [MenuItem("EntitiesBT/BehaviorTreeEditor")]
        public static void ShowEditor()
        {
            GetWindow();
            ResetEditorView();
        }

        public void CreateGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.quabug.entities-bt.builder.graphview/Editor/BehaviorTreeEditor.uxml");
            visualTree.CloneTree(rootVisualElement);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.quabug.entities-bt.builder.graphview/Editor/BehaviorTreeEditor.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

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
            var graph = prefabStage == null ? null : FindGraph();
            window.rootVisualElement.Q<BehaviorTreeView>().Reset(graph);

            BehaviorTreeGraph FindGraph()
            {
                var graphPath = Path.ChangeExtension(prefabStage.assetPath, "asset");
                return AssetDatabase.LoadAssetAtPath<BehaviorTreeGraph>(graphPath);
            }
        }

        private void OnSelectionChange()
        {
            var graph = Selection.activeObject as BehaviorTreeGraph;
            if (graph != null)
            {
                var prefab = GetOrCreateCorrespondingPrefab();
                AssetDatabase.OpenAsset(prefab);
            }

            GameObject GetOrCreateCorrespondingPrefab()
            {
                if (graph.Prefab == null)
                {
                    var assetPath = AssetDatabase.GetAssetPath(graph);
                    var prefabPath = Path.ChangeExtension(assetPath, "prefab");
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    if (prefab == null)
                    {
                        Debug.Log($"create new root game object for {assetPath}");
                        var tempGameObject = new GameObject();
                        prefab = PrefabUtility.SaveAsPrefabAsset(tempGameObject, prefabPath);
                        DestroyImmediate(tempGameObject);
                    }
                    else
                    {
                        Debug.Log($"use existing root game object for {assetPath}");
                    }

                    graph.Prefab = prefab;
                }

                return graph.Prefab;
            }
        }
    }
}