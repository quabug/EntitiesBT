using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Entities;
using UnityEngine.Scripting;

namespace EntitiesBT.Variable
{
    public static class VariableReaderRegisters<T> where T : unmanaged
    {
        public delegate T ReadFunc<TNodeBlob, TBlackboard>(ref BlobVariableReader<T> variable, int nodeIndex, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        ;

        public delegate IEnumerable<ComponentType> GetComponentAccessFunc(ref BlobVariableReader<T> variable);
        public static GetComponentAccessFunc GetComponentAccess(int entryId) => _ENTRIES[entryId].ComponentAccess;

        private static readonly Dictionary<int, Entry> _ENTRIES = new Dictionary<int, Entry>();
        
        // guarantee register every properties into this.
        static VariableReaderRegisters()
        {
            foreach (var type in VariablePropertyExtensions.VARIABLE_PROPERTY_TYPES.Value)
            {
                var propertyType = type.MakeGenericType(typeof(T));
                // call static ctor https://stackoverflow.com/a/29511342
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(propertyType.TypeHandle);
            }
        }

        public static void Register(int id, MethodInfo getData, GetComponentAccessFunc componentAccess = null)
        {
            if (_ENTRIES.ContainsKey(id)) throw new DuplicateIdException();
            _ENTRIES[id] = new Entry(getData, componentAccess ?? GetComponentAccessDefault);
        }
        
        public static ReadFunc<TNodeBlob, TBlackboard> Read<TNodeBlob, TBlackboard>(int entryId)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            => GetterRegisters<TNodeBlob, TBlackboard>.GETTER_ENTRIES[entryId];

        private readonly struct Entry
        {
            public readonly MethodInfo GetData;
            public readonly GetComponentAccessFunc ComponentAccess;
        
            public Entry([NotNull] MethodInfo getData, [NotNull] GetComponentAccessFunc componentAccess)
            {
                GetData = getData;
                ComponentAccess = componentAccess;
            }
        }

        private static IEnumerable<ComponentType> GetComponentAccessDefault(ref BlobVariableReader<T> _) => Enumerable.Empty<ComponentType>();
    
        private static class GetterRegisters<TNodeBlob, TBlackboard>
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            [Preserve] public static ReadFunc<TNodeBlob, TBlackboard> GetData(int entryId) => GETTER_ENTRIES[entryId];

            public static readonly Dictionary<int, ReadFunc<TNodeBlob, TBlackboard>> GETTER_ENTRIES;

            static GetterRegisters()
            {
                GETTER_ENTRIES = new Dictionary<int, ReadFunc<TNodeBlob, TBlackboard>>(_ENTRIES.Count * 3);
                var types = new [] {typeof(TNodeBlob), typeof(TBlackboard)};
                foreach (var entry in _ENTRIES)
                {
                    var getData = (ReadFunc<TNodeBlob, TBlackboard>)entry.Value.GetData
                        .MakeGenericMethod(types)
                        .CreateDelegate(typeof(ReadFunc<TNodeBlob, TBlackboard>))
                    ;
                    GETTER_ENTRIES[entry.Key] = getData;
                }
            }
        }
    }
    
    public class DuplicateIdException : Exception {}
}
