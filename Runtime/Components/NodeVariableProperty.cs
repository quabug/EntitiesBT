using System;
using System.Collections.Generic;
using EntitiesBT.Core;
using EntitiesBT.Variable;

namespace EntitiesBT.Components
{
    public struct DynamicNodeData
    {
        public int Index;
        public int Offset;
    }
    
    public class NodeVariableProperty<T> : VariableProperty where T : struct
    {
        public T FallbackValue;
        public BTNode NodeObject;
        public string NodeTypeName;
        public string NodeValueName;
        
        [NonSerialized] public IList<ITreeNode<INodeDataBuilder>> TreeNodes;
    }
}
