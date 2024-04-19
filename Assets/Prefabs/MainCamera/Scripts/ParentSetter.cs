using UnityEngine;

namespace Prefabs.MainCamera.Scripts
{
    public class ParentSetter : MonoBehaviour
    {
        public Transform parentTransform;

        private void Awake()
        {
            transform.parent = parentTransform;
        }
    }
}
