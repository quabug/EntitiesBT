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
        [AsGameObjectName(Default = "Unknown", NamePatter = @"(\w+\.|\w+\+)*(\w+), .*||$2")]
        public string NodeType;

        public DynamicBlobDataBuilder Builder;

        public void Build(BlobBuilder builder, IntPtr dataPtr) => throw new NotImplementedException();//Builder.Build(builder, dataPtr);

        public IBuilder FindBuilderByPath(string path)
        {
            var pathList = path.Split('.');
            return pathList.Aggregate((IBuilder)Builder, (builder, name) => builder.GetBuilder(name));
        }

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