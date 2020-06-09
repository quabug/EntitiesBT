using System.Reflection;
using Unity.Assertions;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    public static class EntityCommandBufferExtension
    {
        private static MethodInfo _addBuffer;
        
        static EntityCommandBufferExtension()
        {
            _addBuffer = typeof(EntityCommandBuffer).GetMethod("AddBuffer", BindingFlags.Instance | BindingFlags.Public);
        }
        
        public static void AddBuffer(this EntityCommandBuffer ecb, Entity entity, ComponentType componentType)
        {
            var type = TypeManager.GetType(componentType.TypeIndex);
            Assert.IsNotNull(type);
            Assert.IsTrue(typeof(IBufferElementData).IsAssignableFrom(type));
            _addBuffer.MakeGenericMethod(type).Invoke(ecb, new object[] {entity});
        }
    }
}