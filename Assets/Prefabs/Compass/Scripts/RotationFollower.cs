using UnityEngine;
using UnityEngine.Assertions;

namespace Prefabs.Compass.Scripts
{
    public class RotationFollower : MonoBehaviour
    {
        public Transform sourceObject;
        public Transform destinationObject;
        public float lerpAlpha = 0.05f;

        private void Awake()
        {
            Assert.IsNotNull(sourceObject);
            Assert.IsNotNull(destinationObject);
            destinationObject.rotation = sourceObject.rotation;
        }

        private void LateUpdate()
        {
            destinationObject.rotation = Quaternion.Slerp(destinationObject.rotation, sourceObject.rotation, lerpAlpha);
        }
    }
}