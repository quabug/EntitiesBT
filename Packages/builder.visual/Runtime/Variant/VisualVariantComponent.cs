using System;
using EntitiesBT.Variant;
using Runtime;
using Unity.Assertions;

namespace EntitiesBT.Builder.Visual
{
    [Serializable]
    public struct VisualVariantComponent : IVisualVariantNode
    {
        public TypeReference Type;
        public OutputDataMultiPort ComponentData;

        public IVariantReader<T> GetVariantReader<T>(int dataIndex, GraphInstance instance, GraphDefinition definition) where T : unmanaged
        {
            for (uint i = 0; i < ComponentData.GetDataCount(); i++)
            {
                var port = ComponentData.SelectPort(i);
                var portIndex = (int)port.Port.Index;
                Assert.IsTrue(portIndex < definition.PortInfoTable.Count);
                if (definition.PortInfoTable[portIndex].DataIndex == dataIndex)
                {
                    var field = definition.GetComponentFieldDescription(Type, (int)i);
                    // TODO: check data type
                    if (field.HasValue)
                        return new DynamicComponentVariant.Reader<T>(field.Value.DeclaringTypeHash, field.Value.Offset);
                    throw new IndexOutOfRangeException();
                }
            }
            throw new IndexOutOfRangeException();
        }
    }
}
