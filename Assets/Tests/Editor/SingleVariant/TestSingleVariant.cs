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
    [BehaviorNode("F5451E3F-B230-4207-A11B-B5E7D728F1E0")]
    public struct SingleVariantNode : INodeData
    {
        public int IntValue;
        public BlobVariantRO<int> Variant;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            throw new NotImplementedException();
        }
    }
    
    public class TestSingleVariant
    {
        private Blackboard _bb;
        
        [SetUp]
        public void SetUp()
        {
            _bb = new Blackboard();
        }
        
        [Test]
        public void should_create_correct_blob_from_local_variant()
        {
            CheckVariant("local", LocalVariant.GUID, 123);
        }
        
        [Test]
        public void should_create_correct_blob_from_component_variant()
        {
            _bb.SetData(new TestComponentVariableData { IntValue = 234 });
            CheckVariant("component", ComponentVariant.GUID, 234);
        }
        
        [Test]
        public void should_create_correct_blob_from_expression_variant()
        {
            CheckVariant("expression", ExpressionVariant.GUID, 123+456);
        }
        
        [Test]
        public void should_create_correct_blob_from_node_variant()
        {
            CheckVariant("node", NodeVariant.ID_RUNTIME_NODE, 111);
        }
        
        [Test]
        public void should_create_correct_blob_from_scriptable_object_variant()
        {
            CheckVariant("scriptable-object", ScriptableObjectVariant.GUID, 222);
        }

        void CheckVariant(string prefabName, string variantGUID, int value)
        {
            var blob = LoadBlob(prefabName);
            try
            {
                Assert.That(blob.Count, Is.EqualTo(1));
                Assert.That(blob.GetTypeId(0), Is.EqualTo(typeof(SingleVariantNode).GetBehaviorNodeAttribute().Id));
                
                ref var defaultNode = ref blob.GetNodeDefaultData<SingleVariantNode, ManagedNodeBlobRef>(0);
                Assert.That(defaultNode.Variant.Value.VariantId, Is.EqualTo(Guid.Parse(variantGUID).GetHashCode()));
                Assert.That(defaultNode.Variant.Value.MetaDataOffsetPtr, Is.EqualTo(4));
                Assert.That(defaultNode.Variant.Read(0, ref blob, ref _bb), Is.EqualTo(value));
                
                ref var node = ref blob.GetNodeData<SingleVariantNode, ManagedNodeBlobRef>(0);
                Assert.That(node.Variant.Value.VariantId, Is.EqualTo(Guid.Parse(variantGUID).GetHashCode()));
                Assert.That(node.Variant.Value.MetaDataOffsetPtr, Is.EqualTo(4));
                Assert.That(node.Variant.Read(0, ref blob, ref _bb), Is.EqualTo(value));
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
