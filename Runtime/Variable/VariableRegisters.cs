using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Variable
{
    public static class VariableRegisters<T> where T : struct
    {
        public delegate T GetDataFunc<TNodeBlob, TBlackboard>(ref BlobVariable<T> variable, int nodeIndex, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        ;
        
        public delegate ref T GetDataRefFunc<TNodeBlob, TBlackboard>(ref BlobVariable<T> variable, int nodeIndex, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        ;

        public delegate IEnumerable<ComponentType> GetComponentAccessFunc(ref BlobVariable<T> variable);
        public static GetComponentAccessFunc GetComponentAccess(int entryId) => _ENTRIES[entryId].ComponentAccess;

        private static readonly Dictionary<int, Entry> _ENTRIES = new Dictionary<int, Entry>();

        public static void Register(int id, MethodInfo getData, MethodInfo getDataRef = null, GetComponentAccessFunc componentAccess = null)
        {
            if (_ENTRIES.ContainsKey(id)) throw new DuplicateIdException();
            _ENTRIES[id] = new Entry(getData, getDataRef ?? GetDataRefThrowMethod, componentAccess ?? GetComponentAccessDefault);
        }
        
        public static GetDataFunc<TNodeBlob, TBlackboard> GetData<TNodeBlob, TBlackboard>(int entryId)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            => GetterRegisters<TNodeBlob, TBlackboard>.GETTER_ENTRIES[entryId].Data;
        
        public static GetDataRefFunc<TNodeBlob, TBlackboard> GetDataRef<TNodeBlob, TBlackboard>(int entryId)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            => GetterRegisters<TNodeBlob, TBlackboard>.GETTER_ENTRIES[entryId].DataRef;

        private readonly struct Entry
        {
            public readonly MethodInfo GetData;
            public readonly MethodInfo GetDataRef;
            public readonly GetComponentAccessFunc ComponentAccess;
        
            public Entry([NotNull] MethodInfo getData, [NotNull] MethodInfo getDataRef, [NotNull] GetComponentAccessFunc componentAccess)
            {
                GetData = getData;
                GetDataRef = getDataRef;
                ComponentAccess = componentAccess;
            }
        }

        private static MethodInfo GetDataRefThrowMethod = typeof(VariableRegisters<T>)
            .GetMethod("GetDataRefThrow", BindingFlags.Static | BindingFlags.NonPublic)
        ;
        
        private static ref T GetDataRefThrow<TNodeBlob, TBlackboard>(ref BlobVariable<T> _, int __, ref TNodeBlob ___, ref TBlackboard ____)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
            => throw new NotImplementedException()
        ;

        private static IEnumerable<ComponentType> GetComponentAccessDefault(ref BlobVariable<T> _) => Enumerable.Empty<ComponentType>();
    
        private static class GetterRegisters<TNodeBlob, TBlackboard>
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            public static GetDataFunc<TNodeBlob, TBlackboard> GetData(int entryId) => GETTER_ENTRIES[entryId].Data;
            public static GetDataRefFunc<TNodeBlob, TBlackboard> GetDataRef(int entryId) => GETTER_ENTRIES[entryId].DataRef;

            public readonly struct GetterEntry
            {
                public readonly GetDataFunc<TNodeBlob, TBlackboard> Data;
                public readonly GetDataRefFunc<TNodeBlob, TBlackboard> DataRef;

                public GetterEntry(
                    GetDataFunc<TNodeBlob, TBlackboard> data
                  , GetDataRefFunc<TNodeBlob, TBlackboard> dataRef
                )
                {
                    Data = data;
                    DataRef = dataRef;
                }
            }

            public static readonly Dictionary<int, GetterEntry> GETTER_ENTRIES;

            static GetterRegisters()
            {
                GETTER_ENTRIES = new Dictionary<int, GetterEntry>(_ENTRIES.Count * 3);
                var types = new [] {typeof(TNodeBlob), typeof(TBlackboard)};
                foreach (var entry in _ENTRIES)
                {
                    var getData = (GetDataFunc<TNodeBlob, TBlackboard>)entry.Value.GetData
                        .MakeGenericMethod(types)
                        .CreateDelegate(typeof(GetDataFunc<TNodeBlob, TBlackboard>))
                    ;
                    var getDataRef = (GetDataRefFunc<TNodeBlob, TBlackboard>)entry.Value.GetDataRef
                        .MakeGenericMethod(types)
                        .CreateDelegate(typeof(GetDataRefFunc<TNodeBlob, TBlackboard>))
                    ;
                    GETTER_ENTRIES[entry.Key] = new GetterEntry(getData, getDataRef);
                }
            }
        }
    }
    
    public class DuplicateIdException : Exception {}
}
