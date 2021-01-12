using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine.Assertions;
using static EntitiesBT.Core.Utilities;
using static EntitiesBT.Variant.Utilities;

namespace EntitiesBT.Variant
{
    public class MethodIdAttribute : Attribute
    {
        internal readonly int ID;
        public MethodIdAttribute(string guid) => ID = GuidHashCode(guid);
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class ReaderMethodAttribute : MethodIdAttribute
    {
        public ReaderMethodAttribute(string guid) : base(guid) {}
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class WriterMethodAttribute : MethodIdAttribute
    {
        public WriterMethodAttribute(string id) : base(id) {}
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AccessorMethodAttribute : MethodIdAttribute
    {
        public AccessorMethodAttribute(string id) : base(id) {}
    }

    internal static class VariantRegisters
    {
        internal delegate IEnumerable<ComponentType> GetComponentAccessFunc(ref BlobVariant variant);
        internal static readonly IReadOnlyDictionary<int, MethodInfo> READERS;
        internal static readonly IReadOnlyDictionary<int, MethodInfo> REF_READERS;
        internal static readonly IReadOnlyDictionary<int, MethodInfo> WRITERS;
        internal static readonly IReadOnlyDictionary<int, GetComponentAccessFunc> ACCESSORS;
        public static GetComponentAccessFunc GetComponentAccess(int entryId)
        {
            return ACCESSORS.TryGetValue(entryId, out var func) ? func : GetComponentAccessDefault;
        }

        private static IEnumerable<ComponentType> GetComponentAccessDefault(ref BlobVariant _) => Enumerable.Empty<ComponentType>();

        static VariantRegisters()
        {
            var readers = new Dictionary<int, MethodInfo>();
            var refReaders = new Dictionary<int, MethodInfo>();
            var writers = new Dictionary<int, MethodInfo>();
            var accessors = new Dictionary<int, GetComponentAccessFunc>();
            foreach (var (methodInfo, attribute) in
                from type in BEHAVIOR_TREE_ASSEMBLY_TYPES.Value
                from methodInfo in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                from attribute in methodInfo.GetCustomAttributes<MethodIdAttribute>()
                select (methodInfo, attribute)
            )
            {
                switch (attribute)
                {
                case ReaderMethodAttribute reader:
                    ValidateReaderMethod(methodInfo);
                    if (methodInfo.ReturnType.IsByRef) refReaders.Add(reader.ID, methodInfo);
                    else readers.Add(reader.ID, methodInfo);
                    break;
                case WriterMethodAttribute writer:
                    ValidateWriterMethod(methodInfo);
                    writers.Add(writer.ID, methodInfo);
                    break;
                case AccessorMethodAttribute accessor:
                    ValidateAccessorMethod(methodInfo);
                    accessors.Add(accessor.ID, (GetComponentAccessFunc)methodInfo.CreateDelegate(typeof(GetComponentAccessFunc)));
                    break;
                default:
                    throw new NotImplementedException();
                }
            }
            READERS = new ReadOnlyDictionary<int, MethodInfo>(readers);
            REF_READERS = new ReadOnlyDictionary<int, MethodInfo>(refReaders);
            WRITERS = new ReadOnlyDictionary<int, MethodInfo>(writers);
            ACCESSORS = new ReadOnlyDictionary<int, GetComponentAccessFunc>(accessors);
        }

        static void ValidateAccessorMethod(MethodInfo methodInfo)
        {
            // static IEnumerable<ComponentType> GetDynamicAccess(ref BlobVariant blobVariant)
            var methodMessage = $"{methodInfo.DeclaringType.FullName}.{methodInfo.Name}";
            Assert.IsFalse(methodInfo.IsGenericMethod, methodMessage);

            Assert.AreEqual(typeof(IEnumerable<ComponentType>), methodInfo.ReturnType, methodMessage);

            var parameters = methodInfo.GetParameters();
            Assert.AreEqual(1, parameters.Length, methodMessage);
            Assert.AreEqual(typeof(BlobVariant), parameters[0].ParameterType.GetElementType(), methodMessage);
            Assert.IsTrue(parameters[0].ParameterType.IsByRef, methodMessage);
        }

        static void ValidateWriterMethod(MethodInfo methodInfo)
        {
            // static void WriteFunc<T, TNodeBlob, TBlackboard>(ref BlobVariant variant, int nodeIndex, ref TNodeBlob blob, ref TBlackboard bb, T value)

            var methodMessage = $"{methodInfo.DeclaringType.FullName}.{methodInfo.Name}";
            Assert.IsTrue(methodInfo.IsGenericMethod, methodMessage);

            var genericArguments = methodInfo.GetGenericArguments();
            Assert.AreEqual(3, genericArguments.Length, methodMessage);

            Assert.AreEqual(typeof(void), methodInfo.ReturnType, methodMessage);

            var parameters = methodInfo.GetParameters();
            Assert.AreEqual(5, parameters.Length, methodMessage);
            Assert.AreEqual(typeof(BlobVariant), parameters[0].ParameterType.GetElementType(), methodMessage);
            Assert.IsTrue(parameters[0].ParameterType.IsByRef, methodMessage);
            Assert.AreEqual(typeof(int), parameters[1].ParameterType, methodMessage);
            Assert.AreEqual(genericArguments[1], parameters[2].ParameterType.GetElementType(), methodMessage);
            Assert.IsTrue(parameters[2].ParameterType.IsByRef, methodMessage);
            Assert.AreEqual(genericArguments[2], parameters[3].ParameterType.GetElementType(), methodMessage);
            Assert.IsTrue(parameters[3].ParameterType.IsByRef, methodMessage);
            Assert.AreEqual(genericArguments[0], parameters[4].ParameterType, methodMessage);
            Assert.IsFalse(parameters[4].ParameterType.IsByRef, methodMessage);
        }

        static void ValidateReaderMethod(MethodInfo methodInfo)
        {
            // static ref T Read<T, TNodeBlob, TBlackboard>(ref BlobVariant blobVariant, int index, ref TNodeBlob blob, ref TBlackboard bb)

            var methodMessage = $"{methodInfo.DeclaringType.FullName}.{methodInfo.Name}";
            Assert.IsTrue(methodInfo.IsGenericMethod, methodMessage);

            var genericArguments = methodInfo.GetGenericArguments();
            Assert.AreEqual(3, genericArguments.Length, methodMessage);

            var returnType = methodInfo.ReturnType;
            if (returnType.IsByRef) returnType = returnType.GetElementType();
            Assert.AreEqual(genericArguments[0], returnType, methodMessage);

            var parameters = methodInfo.GetParameters();
            Assert.AreEqual(4, parameters.Length, methodMessage);
            Assert.AreEqual(typeof(BlobVariant), parameters[0].ParameterType.GetElementType(), methodMessage);
            Assert.IsTrue(parameters[0].ParameterType.IsByRef, methodMessage);
            Assert.AreEqual(typeof(int), parameters[1].ParameterType, methodMessage);
            Assert.AreEqual(genericArguments[1], parameters[2].ParameterType.GetElementType(), methodMessage);
            Assert.IsTrue(parameters[2].ParameterType.IsByRef, methodMessage);
            Assert.AreEqual(genericArguments[2], parameters[3].ParameterType.GetElementType(), methodMessage);
            Assert.IsTrue(parameters[3].ParameterType.IsByRef, methodMessage);
        }
    }

    public static class VariantRegisters<T> where T : struct
    {
        private static IReadOnlyDictionary<int, TDelegate> MakeRegisterDictionary<TDelegate, TNodeBlob, TBlackboard>(
            IReadOnlyDictionary<int, MethodInfo> methodInfoMap
        )
            where TDelegate : System.Delegate
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var map = new Dictionary<int, TDelegate>(methodInfoMap.Count * 3);
            var types = new [] {typeof(T), typeof(TNodeBlob), typeof(TBlackboard)};
            foreach (var keyValue in methodInfoMap)
            {
                var func = (TDelegate)keyValue.Value
                    .MakeGenericMethod(types)
                    .CreateDelegate(typeof(TDelegate))
                ;
                map[keyValue.Key] = func;

            }
            return new ReadOnlyDictionary<int, TDelegate>(map);
        }

        public delegate T ReadFunc<TNodeBlob, TBlackboard>(ref BlobVariant variant, int nodeIndex, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        ;

        public delegate ref T ReadRefFunc<TNodeBlob, TBlackboard>(ref BlobVariant variant, int nodeIndex, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        ;

        public static ReadFunc<TNodeBlob, TBlackboard> GetReader<TNodeBlob, TBlackboard>(int entryId)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            => ReaderRegisters<TNodeBlob, TBlackboard>.READERS[entryId];

        public static bool TryGetValue<TNodeBlob, TBlackboard>(int entryId, out ReadFunc<TNodeBlob, TBlackboard> value)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            => ReaderRegisters<TNodeBlob, TBlackboard>.READERS.TryGetValue(entryId, out value);

        public static ReadRefFunc<TNodeBlob, TBlackboard> GetRefReader<TNodeBlob, TBlackboard>(int entryId)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            => RefReaderRegisters<TNodeBlob, TBlackboard>.READERS[entryId];

        // optimize: convert method info into delegate to increase performance on calling.
        private static class ReaderRegisters<TNodeBlob, TBlackboard>
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            public static readonly IReadOnlyDictionary<int, ReadFunc<TNodeBlob, TBlackboard>> READERS =
                MakeRegisterDictionary<ReadFunc<TNodeBlob, TBlackboard>, TNodeBlob, TBlackboard>(VariantRegisters.READERS);
        }

        // optimize: convert method info into delegate to increase performance on calling.
        private static class RefReaderRegisters<TNodeBlob, TBlackboard>
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            public static readonly IReadOnlyDictionary<int, ReadRefFunc<TNodeBlob, TBlackboard>> READERS =
                MakeRegisterDictionary<ReadRefFunc<TNodeBlob, TBlackboard>, TNodeBlob, TBlackboard>(VariantRegisters.REF_READERS);
        }

        public delegate void WriteFunc<TNodeBlob, TBlackboard>(ref BlobVariant variant, int nodeIndex, ref TNodeBlob blob, ref TBlackboard bb, T value)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        ;

        public static WriteFunc<TNodeBlob, TBlackboard> GetWriter<TNodeBlob, TBlackboard>(int entryId)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            => WriterRegisters<TNodeBlob, TBlackboard>.WRITERS[entryId];

        // optimize: convert writer method info into delegate to increase performance on calling.
        private static class WriterRegisters<TNodeBlob, TBlackboard>
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            public static readonly IReadOnlyDictionary<int, WriteFunc<TNodeBlob, TBlackboard>> WRITERS =
                MakeRegisterDictionary<WriteFunc<TNodeBlob, TBlackboard>, TNodeBlob, TBlackboard>(VariantRegisters.WRITERS);
        }
    }
}
