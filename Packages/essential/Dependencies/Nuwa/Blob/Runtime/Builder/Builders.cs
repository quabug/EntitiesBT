using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Blob;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Assertions;

namespace Nuwa.Blob
{
    using BlobString = BlobString<UTF8Encoding>;

    public interface IBuilder : global::Blob.IBuilder
    {
        IBuilder GetBuilder([NotNull] string name);
        object PreviewValue { get; set; }
    }

    public interface IObjectBuilder
    {
        IReadOnlyList<string> GetFieldNames();
        IReadOnlyList<IBuilder> GetBuilders();
    }

    public abstract class Builder<T> : IBuilder where T : unmanaged
    {
        public int Position { get; private set; }

        public void Build(IBlobStream stream)
        {
            Position = stream.DataPosition;
            stream.EnsureDataSize<T>();
            BuildImpl(stream);
        }

        protected abstract void BuildImpl([NotNull] IBlobStream stream);

        public virtual IBuilder GetBuilder(string name) => throw new NotImplementedException();

        public virtual object PreviewValue
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Builder of POD.
    /// Show the POD as whole in inspector.
    /// Not support data with its own builder, e.g. `BlobPtr`, `BlobArray` and `BlobString`.
    /// </summary>
    /// <typeparam name="T">type of POD</typeparam>
    [Serializable]
    public class PlainDataBuilder<T> : Builder<T> where T : unmanaged
    {
        public T Value;

        protected override void BuildImpl(IBlobStream stream)
        {
            stream.WriteValue(ref Value);
        }

        public override object PreviewValue
        {
            get => Value;
            set => Value = (T)value;
        }
    }

    /// <summary>
    /// Builder of structure with `Blob` data inside.
    /// Split each data inside structure into its own builder to show and edit.
    /// Support data with <seealso cref="DefaultBuilderAttribute"/> and <seealso cref="CustomBuilderAttribute"/>, e.g. `BlobPtr`, `BlobArray` and `BlobString`
    /// </summary>
    /// <typeparam name="T">type of blob structure</typeparam>
    [Serializable]
    public class BlobDataBuilder<T> : Builder<T>, IObjectBuilder where T : unmanaged
    {
        [HideInInspector] public string[] FieldNames;
        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] public IBuilder[] Builders;

        public BlobDataBuilder()
        {
            BuilderUtility.SetBlobDataType(typeof(T), ref Builders, ref FieldNames);
        }

        protected override void BuildImpl(IBlobStream stream)
        {
            var dataPosition = stream.DataPosition;
            var fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (var i = 0; i < Builders.Length; i++)
            {
                Assert.AreEqual(FieldNames[i], fields[i].Name);
                var offset = UnsafeUtility.GetFieldOffset(fields[i]);
                stream.ToPosition(dataPosition + offset).WriteValue(Builders[i]);
            }
        }

        public IReadOnlyList<string> GetFieldNames() => FieldNames;
        public IReadOnlyList<IBuilder> GetBuilders() => Builders;

        public override IBuilder GetBuilder(string name)
        {
            var index = Array.IndexOf(FieldNames, name);
            if (index < 0) throw new ArgumentException($"cannot find field by name {name}");
            return Builders[index];
        }

        public override object PreviewValue
        {
            get => FieldNames.Zip(Builders, (field, builder) => (field, builder.PreviewValue)).ToDictionary(t => t.field, t => t.PreviewValue);
            set
            {
                foreach (var t in (IDictionary<string, object>)value)
                {
                    var index = Array.IndexOf(FieldNames, t.Key);
                    Builders[index].PreviewValue = t.Value;
                }
            }
        }
    }

    [Serializable]
    public class ArrayBuilder<T> : Builder<BlobArray<T>> where T : unmanaged
    {
        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] public IBuilder[] Value;

        protected override unsafe void BuildImpl(IBlobStream stream)
        {
            stream.WritePatchOffset()
                .WriteValue(Value.Length)
                .ToPatchPosition()
                .WriteArray(Value, sizeof(T), Utilities.AlignOf<T>())
            ;
        }

        public override IBuilder GetBuilder(string name)
        {
            var index = int.Parse(name);
            return Value[index];
        }

        public override object PreviewValue
        {
            get => Value.Select(v => v.PreviewValue).ToArray();
            set
            {
                for (var i = 0; i < Value.Length; i++) Value[i].PreviewValue = ((Array)value).GetValue(i);
            }
        }
    }

    [Serializable, DefaultBuilder]
    public class StringBuilder : Builder<BlobString>
    {
        public string Value;

        public override object PreviewValue
        {
            get => Value;
            set => Value = (string)value;
        }

        protected override void BuildImpl(IBlobStream stream)
        {
            var stringBytes = Encoding.UTF8.GetBytes(Value);
            stream.WritePatchOffset()
                .WriteValue(stringBytes.Length)
                .ToPatchPosition()
                .WriteArray(stringBytes)
            ;
        }
    }

    [DefaultBuilder] public class StringArrayBuilder : ArrayBuilder<BlobString> {}
    [DefaultBuilder] public class StringPtrBuilder : PtrBuilder<BlobString> {}

    [Serializable]
    public class PtrBuilder<T> : Builder<BlobPtr<T>> where T : unmanaged
    {
        [SerializeReference, UnboxSingleProperty, UnityDrawProperty] public IBuilder Value;

        protected override void BuildImpl(IBlobStream stream)
        {
            stream.WritePatchOffset().ToPatchPosition().WriteValue(Value);
        }

        public override IBuilder GetBuilder(string name)
        {
            if (name != "*") throw new ArgumentException("it must be * to access builder of BlobPtr");
            return Value;
        }
    }
}