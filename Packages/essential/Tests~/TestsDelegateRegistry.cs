using System;
using System.Text.RegularExpressions;
using EntitiesBT.Variant;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static EntitiesBT.Core.Utilities;

namespace EntitiesBT.Tests
{
    public class TestsDelegateRegistry
    {
        [RegisterDelegateClass(GUID)]
        static class TestClass
        {
            public const string GUID = "BBE08DBD-ADD1-463A-B9C8-92CC8CCAF789";

            [RegisterDelegateMethod(typeof(Func<int, int, int>))]
            public static int A(int l, int r) => l + r;

            // should log error
            [RegisterDelegateMethod(typeof(Func<int, int, double>))]
            public static float B(int l, float r) => l + r;

            [RegisterDelegateMethod(typeof(Func<,,>))]
            public static R C<T, U, R>(T l, U r) => throw new NotImplementedException();

            [RegisterDelegateMethod(typeof(Func<int, int, float>))]
            public static float D(int l, int r) => 4;
        }

        [Test]
        public void should_get_delegate_method_A_by_id_from_registry()
        {
            var @delegate = DelegateRegistry<Func<int, int, int>>.TryGetValue(GuidHashCode(TestClass.GUID));
            Assert.IsNotNull(@delegate);
            Assert.AreEqual(3, @delegate(1, 2));
        }

        static bool _isFirstRun = true;
        static class FirstRun
        {
            public static void Init() { }
            static FirstRun() => _isFirstRun = false;
        }

        [Test]
        public void should_log_error_while_fetching_delegate_method_B_by_id_from_registry()
        {
            DelegateRegistry<Func<int, int, double>>.TryGetValue(GuidHashCode(TestClass.GUID));
            if (_isFirstRun) LogAssert.Expect(LogType.Error, new Regex("Cannot create delegate .*"));
            FirstRun.Init();
        }

        [Test]
        public void should_get_delegate_method_C_by_id_from_registry()
        {
            var @delegate = DelegateRegistry<Func<short, short, short>>.TryGetValue(GuidHashCode(TestClass.GUID));
            Assert.IsNotNull(@delegate);
            Assert.Throws<NotImplementedException>(() => @delegate(0, 0));
        }

        static class TestGeneric<T, U>
        {
            public static R Call<R>() where R : unmanaged
            {
                var id = GuidHashCode(TestClass.GUID);
                var @delegate = DelegateRegistry<Func<T, U, R>>.TryGetValue(id);
                return @delegate?.Invoke(default, default) ?? default;
            }
        }
        [Test]
        public void should_call_delegate_by_types_and_id()
        {
            Assert.AreEqual(4, TestGeneric<int, int>.Call<float>());
        }
    }
}