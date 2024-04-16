using UnityEngine;
using UnityEngine.Assertions;

namespace Prefabs.Mitya.Scripts
{
    public class RobotController : MonoBehaviour
    {
        public Camera robotCamera;
        public Transform robotCameraParent;

        private void Awake()
        {
            Assert.IsNotNull(robotCamera);
            Assert.IsNotNull(robotCameraParent);
            robotCamera.transform.parent = robotCameraParent;
            robotCamera.transform.localPosition = Vector3.zero;
            robotCamera.transform.localRotation = Quaternion.identity;
        }
    }
}
