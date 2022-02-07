using System;

namespace EntitiesBT
{
    public static class GraphUtility
    {
        public static void RegisterNameChanged<T>(Action<T> setName)
        {
            UnityEditor.EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            UnityEditor.EditorApplication.hierarchyChanged += OnHierarchyChanged;

            void OnHierarchyChanged()
            {
                var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage == null) return;
                foreach (var node in prefabStage.prefabContentsRoot .GetComponentsInChildren<T>())
                    setName(node);
            }
        }
    }
}