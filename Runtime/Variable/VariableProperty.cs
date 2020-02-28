using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;
using Hash128 = UnityEngine.Hash128;

namespace EntitiesBT.Variable
{
    public enum ValueSource
    {
        Constant
      , ConstantComponent
      , ConstantScriptableObject
      , ConstantNode
        
      , DynamicComponent
      , DynamicScriptableObject
      , DynamicNode
    }

    public struct DynamicComponentData
    {
        public ulong StableHash;
        public int Offset;
    }

    public struct DynamicScriptableObjectData
    {
        public Hash128 Id;
        public int Offset;
    }

    public struct DynamicNodeData
    {
        public int Index;
        public int Offset;
    }
    
    [Serializable]
    public struct VariableProperty<T> where T : struct
    {
        public ValueSource ValueSource;
        public T ConstantValue;
        public string ComponentValue;
        public ScriptableObject ScriptableObject;
        public string ScriptableObjectValueName;
    }
}
