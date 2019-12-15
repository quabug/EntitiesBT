using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Editor
{
    public class BTRoot : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            
        }
    }
}
