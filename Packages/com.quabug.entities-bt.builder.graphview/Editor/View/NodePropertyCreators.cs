using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Components;
using JetBrains.Annotations;
using Nuwa.Blob;
using Nuwa.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace EntitiesBT.Editor
{
    public interface INodePropertyCreator
    {
        int Order { get; }
        bool CanCreate([NotNull] SerializedProperty property);
        [NotNull] IEnumerable<NodePropertyView> Create([NotNull] SerializedProperty property);
    }

    public static class NodePropertyHandler
    {
        private static readonly IReadOnlyList<INodePropertyCreator> _propertyCreators;

        static NodePropertyHandler()
        {
            var propertyCreators = TypeCache.GetTypesDerivedFrom<INodePropertyCreator>();
            _propertyCreators = propertyCreators
                .Select(type => (INodePropertyCreator)Activator.CreateInstance(type))
                .OrderBy(creator => creator.Order)
                .ToArray()
            ;
        }

        [NotNull, Pure]
        public static IEnumerable<NodePropertyView> CreateNodeProperty([NotNull] this SerializedProperty property)
        {
            return _propertyCreators
                .Select(creator => creator.Create(property))
                .FirstOrDefault(nodeProperty => nodeProperty.Any())
                ?? Enumerable.Empty<NodePropertyView>()
            ;
        }

        [CanBeNull]
        internal static INodePropertyCreator FindCreator([NotNull] this SerializedProperty property)
        {
            return _propertyCreators.FirstOrDefault(creator => creator.CanCreate(property));
        }
    }

    public class NodeAssetPropertyCreator : INodePropertyCreator
    {
        public int Order => 0;

        public bool CanCreate(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.Generic) return false;
            if (property.type != nameof(NodeAsset)) return false;
            return true;
        }

        public IEnumerable<NodePropertyView> Create(SerializedProperty property)
        {
            if (!CanCreate(property)) yield break;

            var dynamicBuilder = property.FindPropertyRelative(nameof(NodeAsset.Builder));
            var builders = dynamicBuilder.FindPropertyRelative(nameof(DynamicBlobDataBuilder.Builders));
            var fields = dynamicBuilder.FindPropertyRelative(nameof(DynamicBlobDataBuilder.FieldNames));

            for (var i = 0; i < builders.arraySize; i++)
            {
                var builder = builders.GetArrayElementAtIndex(i);
                var creator = builder.FindCreator();
                if (creator == null) continue;

                foreach (var element in creator.Create(builder))
                {
                    yield return element;
                }
            }
        }
    }

    public class VariantRWPropertyCreator : INodePropertyCreator
    {
        public int Order => 0;

        public bool CanCreate(SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference) return false;
            if (property.managedReferenceFullTypename != "EntitiesBT.Runtime EntitiesBT.Variant.BlobVariantLinkedRWBuilder") return false;
            return true;
        }

        public IEnumerable<NodePropertyView> Create(SerializedProperty property)
        {
            if (!CanCreate(property)) return Enumerable.Empty<NodePropertyView>();

            var propertyView = new NodePropertyView(property);

            var port = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(int));
            port.portName = property.name;
            port.AddToClassList("variant");
            return port.Yield();
        }
    }
}