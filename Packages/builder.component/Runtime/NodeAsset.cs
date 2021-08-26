using System;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using Nuwa;
using Nuwa.Blob;
using Unity.Entities;

namespace EntitiesBT.Components
{
    [Serializable]
    public class NodeAsset
    {
        [SerializedType(typeof(INodeData), RenamePatter = @".*(\.|\+)(\w+)$||$2", Nullable = false, DisplayAssemblyName = false, CategoryName = nameof(CategoryName), Where = nameof(WhereFilter))]
        public string NodeType;

        public DynamicBlobDataBuilder Builder;

        public void Build(BlobBuilder builder, IntPtr dataPtr) => Builder.Build(builder, dataPtr);

        private string CategoryName(Type type)
        {
            return type?.GetCustomAttribute<BehaviorNodeAttribute>()?.Type.ToString() ?? "";
        }

        private bool WhereFilter(Type type)
        {
            return !type.IsAbstract && type.GetCustomAttributes<BehaviorNodeAttribute>().Any();
        }
    }
}