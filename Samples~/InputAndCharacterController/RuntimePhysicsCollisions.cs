using System;
using UnityEngine;

namespace EntitiesBT.Samples
{
    public class RuntimePhysicsCollisions : MonoBehaviour
    {
        [Serializable]
        public struct CollisionLayer
        {
            [Layer] public int Layer1;
            [Layer] public int Layer2;
            public bool ShouldCollide;
        }

        public CollisionLayer[] Layers;
        
        private void Awake()
        {
            foreach (var layer in Layers)
            {
                Physics.IgnoreLayerCollision(layer.Layer1, layer.Layer2, !layer.ShouldCollide);
            }
        }
    }
}
