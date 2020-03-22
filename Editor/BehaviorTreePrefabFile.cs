using System.IO;
using EntitiesBT.Components;
using EntitiesBT.Entities;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace EntitiesBT.Editor
{
    // [InitializeOnLoad]
    // public class BehaviorTreePrefabFile : AssetPostprocessor
    // {
    //     static BehaviorTreePrefabFile()
    //     {
    //         PrefabStage.prefabSaved -= OnPrefabSaved;
    //         PrefabStage.prefabSaved += OnPrefabSaved;
    //         // PrefabUtility.prefabInstanceUpdated -= OnPrefabSaved;
    //         // PrefabUtility.prefabInstanceUpdated += OnPrefabSaved;
    //     }
    //
    //     static void OnPrefabSaved(GameObject prefab)
    //     {
    //         var node = prefab.GetComponent<BTNode>();
    //         if (node == null) return;
    //         
    //         var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab);
    //         var filePath = $"{Path.GetDirectoryName(path)}/{Path.GetFileNameWithoutExtension(path)}.bytes";
    //         using (var file = new FileStream(filePath, FileMode.OpenOrCreate))
    //             node.SaveToStream(file);
    //         AssetDatabase.ImportAsset(filePath);
    //         AssetDatabase.SaveAssets();
    //         AssetDatabase.Refresh();
    //     }
    // }
}
