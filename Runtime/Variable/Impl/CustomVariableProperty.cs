using System;
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Variable
{
    [Serializable]
    public class CustomVariableProperty<T> : VariableProperty<T> where T : struct
    {
        public override int VariablePropertyTypeId => ID;
        public T CustomValue;

        protected override void AllocateData(ref BlobBuilder builder, ref BlobVariable<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            builder.Allocate(ref blobVariable, CustomValue);
        }

        static CustomVariableProperty()
        {
            VariableRegisters<T>.Register(ID, GetData, GetDataRef);
        }

        public static readonly int ID = new Guid("4BFE23E6-F819-4C5E-92FF-E6E7FFC83314").GetHashCode();
        
        private static T GetData(ref BlobVariable<T> blobVariable, int index, INodeBlob blob, IBlackboard bb)
        {
            return blobVariable.Value<T>();
        }
        
        private static ref T GetDataRef(ref BlobVariable<T> blobVariable, int index, INodeBlob blob, IBlackboard bb)
        {
            return ref blobVariable.Value<T>();
        }
    }
}
