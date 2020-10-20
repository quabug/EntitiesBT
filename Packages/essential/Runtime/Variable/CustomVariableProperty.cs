using System;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine.Scripting;

namespace EntitiesBT.Variable
{
    [Serializable]
    public class CustomVariablePropertyReader<T> : IVariablePropertyReader<T> where T : unmanaged
    {
        public T CustomValue;

        public void Allocate(ref BlobBuilder builder, ref BlobVariableReader<T> blobVariable, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] tree)
        {
            blobVariable.VariableId = ID;
            builder.Allocate(ref blobVariable, CustomValue);
        }

        static CustomVariablePropertyReader()
        {
            var type = typeof(CustomVariablePropertyReader<T>);
            VariableReaderRegisters<T>.Register(ID, type.Getter("GetData"));
        }

        public static readonly int ID = new Guid("4BFE23E6-F819-4C5E-92FF-E6E7FFC83314").GetHashCode();
        
        [Preserve]
        private static T GetData<TNodeBlob, TBlackboard>(ref BlobVariableReader<T> blobVariable, int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            return blobVariable.Value<T>();
        }
    }
}
