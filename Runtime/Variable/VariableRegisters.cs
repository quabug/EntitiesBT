using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using UnityEngine;

namespace EntitiesBT.Variable
{
    public static class VariableRegisters<T> where T : struct
    {
        public delegate T GetDataFunc(ref BlobVariable<T> variable, int nodeIndex, INodeBlob blob, IBlackboard bb);
        public delegate ref T GetDataRefFunc(ref BlobVariable<T> variable, int nodeIndex, INodeBlob blob, IBlackboard bb);

        public static readonly Dictionary<int, GetDataFunc> GET_DATA_FUNCS = new Dictionary<int, GetDataFunc>();
        public static readonly Dictionary<int, GetDataRefFunc> GET_DATA_REF_FUNCS = new Dictionary<int, GetDataRefFunc>();
        public static readonly Dictionary<int, Type> TYPES = new Dictionary<int, Type>();

        public static void Register(int id, GetDataFunc getter, GetDataRefFunc refGetter)
        {
            if (GET_DATA_FUNCS.ContainsKey(id) || GET_DATA_REF_FUNCS.ContainsKey(id))
            {
                Debug.LogError($"");
                throw new DuplicateIdException();
            }
            GET_DATA_FUNCS[id] = getter ?? throw new InvalidGetDataFuncException();
            GET_DATA_REF_FUNCS[id] = refGetter ?? throw new InvalidGetDataRefFuncException();
        }
    }
    
    public class InvalidGetDataFuncException : Exception {}
    public class InvalidGetDataRefFuncException : Exception {}
    public class DuplicateIdException : Exception {}
}
