using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Core;
using Unity.Entities;
using UnityEngine;

namespace EntitiesBT.Variable
{
    public static class VariableRegisters<T> where T : struct
    {
        public delegate T GetDataFunc(ref BlobVariable<T> variable, int nodeIndex, INodeBlob blob, IBlackboard bb);
        public delegate ref T GetDataRefFunc(ref BlobVariable<T> variable, int nodeIndex, INodeBlob blob, IBlackboard bb);
        public delegate IEnumerable<ComponentType> GetComponentAccessFunc(ref BlobVariable<T> variable);

        public static GetDataFunc GetData(int entryId) => _ENTRIES[entryId].Data;
        public static GetDataRefFunc GetDataRef(int entryId) => _ENTRIES[entryId].DataRef;
        public static GetComponentAccessFunc GetComponentAccess(int entryId) => _ENTRIES[entryId].ComponentAccess;

        readonly struct Entry
        {
            public readonly GetDataFunc Data;
            public readonly GetDataRefFunc DataRef;
            public readonly GetComponentAccessFunc ComponentAccess;

            public Entry(GetDataFunc data, GetDataRefFunc dataRef, GetComponentAccessFunc componentAccess)
            {
                Data = data;
                DataRef = dataRef ?? GetDataRefThrow;
                ComponentAccess = componentAccess ?? GetComponentAccessDefault;
            }
        }

        private static readonly Dictionary<int, Entry> _ENTRIES = new Dictionary<int, Entry>();

        public static void Register(int id, GetDataFunc data, GetDataRefFunc dataRef = null, GetComponentAccessFunc componentAccess = null)
        {
            if (_ENTRIES.ContainsKey(id)) throw new DuplicateIdException();
            _ENTRIES[id] = new Entry(data, dataRef, componentAccess);
        }

        private static IEnumerable<ComponentType> GetComponentAccessDefault(ref BlobVariable<T> _) => Enumerable.Empty<ComponentType>();
        private static ref T GetDataRefThrow(ref BlobVariable<T> _, int __, INodeBlob ___, IBlackboard ____) => throw new NotImplementedException();
    }
    
    public class DuplicateIdException : Exception {}
}
