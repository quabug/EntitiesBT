using System;
using JetBrains.Annotations;

namespace Nuwa.Blob
{
    public ref struct TypeFactory
    {
        public Type Type { get; }
        private readonly Func<object> _creator;

        public TypeFactory([NotNull] Type type)
        {
            Type = type;
            _creator = () => Activator.CreateInstance(type);
        }

        public TypeFactory([NotNull] Type type, [NotNull] Func<object> creator)
        {
            Type = type;
            _creator = creator;
        }

        public object Create() => _creator();
    }

}