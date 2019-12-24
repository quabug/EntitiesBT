using UnityEngine;

namespace EntitiesBT.Editor
{
    [RequireComponent(typeof(BTRoot)), DisallowMultipleComponent]
    public class CommonNodeRegistries : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<BTRoot>().Factory.RegisterCommonNodes();
        }
    }
}
