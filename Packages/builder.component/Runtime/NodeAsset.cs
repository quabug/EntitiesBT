using System;
using System.Linq;
using System.Reflection;
using Blob;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Nuwa;
using Nuwa.Blob;
using Unity.Entities;
using IBuilder = Nuwa.Blob.IBuilder;

namespace EntitiesBT.Components
{
    [Serializable]
    public class NodeAsset
    {
        [SerializedType(typeof(INodeData), RenamePatter = @".*(\.|\+)(\w+)$||$2", Nullable = false, DisplayAssemblyName = false, CategoryName = nameof(CategoryName), Where = nameof(WhereFilter))]
        [AsGameObjectName(Default = "Unknown", NamePatter = @"(\w+\.|\w+\+)*(\w+), .*||$2")]
        public string NodeType;

        public DynamicBlobDataBuilder Builder;

        public void Build([NotNull] IBlobStream stream) => Builder.Build(stream);

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