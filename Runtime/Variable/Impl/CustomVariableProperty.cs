using System;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine.Scripting;

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
            var type = typeof(CustomVariableProperty<T>);
            VariableRegisters<T>.Register(ID, type.Getter("GetData"), type.Getter("GetDataRef"));
        }

        public static readonly int ID = new Guid("4BFE23E6-F819-4C5E-92FF-E6E7FFC83314").GetHashCode();
        
        [Preserve]
        private static T GetData<TNodeBlob, TBlackboard>(ref BlobVariable<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return blobVariable.Value<T>();
        }
        
        [Preserve]
        private static ref T GetDataRef<TNodeBlob, TBlackboard>(ref BlobVariable<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return ref blobVariable.Value<T>();
        }
    }
}
