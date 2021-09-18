using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public class MiniMapView : MiniMap
    {
        public new class UxmlFactory : UxmlFactory<MiniMapView, UxmlTraits> {}
    }
}