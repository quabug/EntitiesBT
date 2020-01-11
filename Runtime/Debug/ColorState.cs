#if UNITY_EDITOR

using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.DebugView
{
    [InitializeOnLoad]
    public class HierarchyHighlighter {
 
        static HierarchyHighlighter() {
            EditorApplication.hierarchyWindowItemOnGUI -= ColorState;
            EditorApplication.hierarchyWindowItemOnGUI += ColorState;
            // EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItem_CB;
        }
   
        private static void ColorState(int instanceId, Rect selectionRect)
        {
            if (Event.current.type != EventType.Repaint) return;
            if (!EditorApplication.isPlaying) return;
            
            var o = EditorUtility.InstanceIDToObject(instanceId);
            var view = (o as GameObject)?.GetComponent<BTDebugView>();
            if (view == null || !view.IsValid) return;
            
            var state = view.Blob.GetState(view.Index);
            var color = Color.white;
            switch (state)
            {
            case NodeState.Success:
                color = Color.green;
                break;
            case NodeState.Failure:
                color = Color.red;
                break;
            case NodeState.Running:
                color = Color.yellow;
                break;
            }
            
            GUI.backgroundColor = color;
            //doing this three times because once is kind of transparent.
            GUI.Box(selectionRect, "");
            GUI.Box(selectionRect, "");
            GUI.Box(selectionRect, "");
            GUI.backgroundColor = Color.white;
            EditorApplication.RepaintHierarchyWindow();
        }
    }
}
#endif
