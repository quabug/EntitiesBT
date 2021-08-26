using System;
using JetBrains.Annotations;

namespace Nuwa.Blob
{
    public ref struct BuilderFactory
    {
        public Type BuilderType { get; }
        private readonly Func<object> _creator;

        public BuilderFactory([NotNull] Type builderType)
        {
            BuilderType = builderType;
            _creator = () => Activator.CreateInstance(builderType);
        }

        public BuilderFactory([NotNull] Type builderType, [NotNull] Func<object> creator)
        {
            BuilderType = builderType;
            _creator = creator;
        }

        public object Create() => _creator();
    }

}