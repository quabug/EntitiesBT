using System;
using EntitiesBT.Variant;
using NUnit.Framework;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Tests
{
    public class TestsVariant
    {
        delegate R Delegate<T, U, R>(T lhs, U rhs);

        [RegisterDelegateClass(GUID)]
        static class TestClass
        {
            public const string GUID = "BBE08DBD-ADD1-463A-B9C8-92CC8CCAF789";

            [RegisterDelegateMethod(typeof(Delegate<int, int, int>))]
            public static int A(int l, int r) => throw new NotImplementedException();

            public static float B(int l, float r) => throw new NotImplementedException();

            [RegisterDelegateMethod(typeof(Delegate<,,>))]
            public static R C<T, U, R>(T l, U r) => throw new NotImplementedException();
        }

        static class TestGeneric<T, U>
        {
            public static R Call<R>() where R : unmanaged
            {
                var @delegate = DelegateRegistry<Delegate<T, U, R>>.TryGetValue(0);
                return @delegate?.Invoke(default, default) ?? default;
            }
        }

        [Test]
        public void read()
        {
            var @delegate = DelegateRegistry<Delegate<int, int, int>>.TryGetValue(GuidHashCode(TestClass.GUID));
            Assert.IsNotNull(@delegate);
        }
    }
}