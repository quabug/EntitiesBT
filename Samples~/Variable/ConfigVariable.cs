using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntitiesBT.Sample
{
    [CreateAssetMenu(fileName = "Variables", menuName = "Test/Variables")]
    public class ConfigVariable : ScriptableObject
    {
        public int Value;
        
        [SerializeReference, SerializeReferenceButton]
        public List<RootNode>  m_Trees;
    }
    
    public interface INode{  }
 
    [Serializable]
    public class RootNode : INode
    {
        [SerializeReference, SerializeReferenceButton] public INode left;
        [SerializeReference, SerializeReferenceButton] public INode right;
    }
 
    [Serializable]
    public class SubNode : RootNode
    {
        [SerializeReference, SerializeReferenceButton] INode parent;
    }
 
    [Serializable]
    public class LeafNode : INode
    {
        [SerializeReference, SerializeReferenceButton] public INode parent;
    }

}
