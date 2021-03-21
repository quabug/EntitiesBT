using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using Sirenix.OdinInspector;
using Unity.Entities;

namespace EntitiesBT.Components.Odin
{
    public class OdinNodeVariant
    {
        // TODO: check loop ref?
        [Serializable]
        public class Any<T> : IVariant where T : unmanaged
        {
            public INodeDataBuilder NodeObject;

#if UNITY_EDITOR
            private IEnumerable<string> _validFieldName => OdinEditorUtilities.GetReadableFieldName<T>(NodeObject);
            [ValueDropdown(nameof(_validFieldName))]
#endif
            public string ValueFieldName;


            public IntPtr Allocate(ref BlobBuilder builder, ref BlobVariant blobVariant, INodeDataBuilder self,
                ITreeNode<INodeDataBuilder>[] tree)
            {
                return NodeVariant.Allocate<T>(ref builder, ref blobVariant, self, tree, NodeObject, ValueFieldName);
            }
        }

        [Serializable] public class Reader<T> : Any<T>, IVariantReader<T> where T : unmanaged {}
        [Serializable] public class Writer<T> : Any<T>, IVariantWriter<T> where T : unmanaged {}
        [Serializable] public class ReaderAndWriter<T> : Any<T>, IVariantReaderAndWriter<T> where T : unmanaged {}
    }
}