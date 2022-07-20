using System;
using System.IO;
using Blob;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.Variant;
using EntitiesBT.Variant.Expression;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EntitiesBT.Test
{
    [BehaviorNode("9CE63B04-86E6-48CE-A4A0-49A03D9A11B8")]
    public struct MultipleVariantsNode : INodeData
    {
        public int IntValue;
        public BlobVariantRO<int> ReadOnlyVariant;
        public BlobVariantRW<int> ReadWriteVariant;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            throw new NotImplementedException();
        }
    }
    
    public class TestMultipleVariant
    {
        private Blackboard _bb;
        
        [SetUp]
        public void SetUp()
        {
            _bb = new Blackboard();
        }
        
        [Test]
        public void should_create_correct_blob_from_local_variant_with_linked_rw()
        {
            CheckVariant("local-linked", LocalVariant.GUID, 123, 234);
        }
        
        [Test]
        public void should_create_correct_blob_from_component_variant_with_linked_rw()
        {
            _bb.SetData(new TestComponentVariableData { IntValue = 234 });
            CheckVariant("component-linked", ComponentVariant.GUID, 234);
        }
        
        [Test]
        public void should_create_correct_blob_from_node_variant_with_linked_rw()
        {
            CheckVariant("node-linked", NodeVariant.ID_RUNTIME_NODE, 222);
        }
        
        [Test]
        public void should_create_correct_blob_from_local_variant()
        {
            CheckVariant("local", LocalVariant.GUID, 123, 234);
        }
        
        [Test]
        public void should_create_correct_blob_from_component_variant()
        {
            _bb.SetData(new TestComponentVariableData { IntValue = 234 });
            CheckVariant("component", ComponentVariant.GUID, 234);
        }
        
        [Test]
        public void should_create_correct_blob_from_node_variant()
        {
            CheckVariant("node", NodeVariant.ID_RUNTIME_NODE, 222);
        }

        void CheckVariant(string prefabName, string variantGUID, int readOnlyValue)
        {
            CheckVariant(prefabName, variantGUID, readOnlyValue, readOnlyValue);
        }
        
        void CheckVariant(string prefabName, string variantGUID, int readOnlyValue, int readWriteValue)
        {
            var blob = LoadBlob(prefabName);
            try
            {
                Assert.That(blob.Count, Is.EqualTo(1));
                Assert.That(blob.GetTypeId(0), Is.EqualTo(typeof(MultipleVariantsNode).GetBehaviorNodeAttribute().Id));
                
                ref var defaultNode = ref blob.GetNodeDefaultData<MultipleVariantsNode, ManagedNodeBlobRef>(0);
                ref var node = ref blob.GetNodeData<MultipleVariantsNode, ManagedNodeBlobRef>(0);
                
                Assert.That(defaultNode.ReadOnlyVariant.Value.VariantId, Is.EqualTo(Guid.Parse(variantGUID).GetHashCode()));
                // Assert.That(defaultNode.ReadOnlyVariant.Value.MetaDataOffsetPtr, Is.EqualTo(20));
                Assert.That(defaultNode.ReadOnlyVariant.Read(0, ref blob, ref _bb), Is.EqualTo(readOnlyValue));
                
                Assert.That(node.ReadOnlyVariant.Value.VariantId, Is.EqualTo(Guid.Parse(variantGUID).GetHashCode()));
                // Assert.That(node.ReadOnlyVariant.Value.MetaDataOffsetPtr, Is.EqualTo(20));
                Assert.That(node.ReadOnlyVariant.Read(0, ref blob, ref _bb), Is.EqualTo(readOnlyValue));
                
                Assert.That(defaultNode.ReadWriteVariant.Reader.Value.VariantId, Is.EqualTo(Guid.Parse(variantGUID).GetHashCode()));
                // Assert.That(defaultNode.ReadWriteVariant.Reader.Value.MetaDataOffsetPtr, Is.EqualTo(16));
                Assert.That(defaultNode.ReadWriteVariant.Read(0, ref blob, ref _bb), Is.EqualTo(readWriteValue));
                
                Assert.That(node.ReadWriteVariant.Reader.Value.VariantId, Is.EqualTo(Guid.Parse(variantGUID).GetHashCode()));
                // Assert.That(node.ReadWriteVariant.Reader.Value.MetaDataOffsetPtr, Is.EqualTo(16));
                Assert.That(node.ReadWriteVariant.Read(0, ref blob, ref _bb), Is.EqualTo(readWriteValue));
            }
            finally
            {
                blob.Dispose();
            }
        }

        private ManagedNodeBlobRef LoadBlob([NotNull] string prefabName)
        {
            var directory = Core.Utilities.GetCurrentDirectoryProjectRelativePath();
            var prefabPath = Path.Combine(directory, $"{prefabName}.prefab");
            var blob = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath).GetComponent<BTDynamicNode>();
            var blobRef = blob.Node.ToBuilder(blob.FindGlobalValuesList()).CreateManagedBlobAssetReference();
            return new ManagedNodeBlobRef(blobRef);
        }
    }
}
