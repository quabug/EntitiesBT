using System;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EntitiesBT.Sample
{
    [GenerateAuthoringComponent]
    public struct EntitySpawnData : IComponentData
    {
        public Bounds PositionBounds;
        public Entity Prefab;
        public float2 LifeTimeRange;
        public int Count;
    }

    public struct EntityLifeTimeBuffer : IBufferElementData
    {
        public Entity Entity;
        public TimeSpan LifeTime;
    }
}