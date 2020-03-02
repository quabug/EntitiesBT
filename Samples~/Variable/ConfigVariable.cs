using UnityEngine;

namespace EntitiesBT.Sample
{
    [CreateAssetMenu(fileName = "TestVariables", menuName = "EntitiesBT/TestVariables")]
    public class ConfigVariable : ScriptableObject
    {
        public int IntValue;
        public float FloatValue;
        public float AnotherFloatValue;
    }
}
