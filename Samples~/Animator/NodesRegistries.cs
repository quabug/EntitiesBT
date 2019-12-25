using System;
using EntitiesBT.Editor;
using UnityEngine;

namespace EntitiesBT
{
    [RequireComponent(typeof(UnityBehaviorNodeFactory)), DisallowMultipleComponent]
    public class NodesRegistries : MonoBehaviour
    {
        public Animator Animator;
        
        private void Awake()
        {
            var factory = GetComponent<UnityBehaviorNodeFactory>().Factory;
            factory.RegisterCommonNodes(() => TimeSpan.FromSeconds(Time.deltaTime));
            factory.Register<SetAnimatorTriggerNode>(() => new SetAnimatorTriggerNode(Animator));
        }
    }
}
