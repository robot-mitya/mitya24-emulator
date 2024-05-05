using Prefabs.Display.Scripts;
using UnityEngine;
using UnityEngine.Assertions;

// ReSharper disable once CheckNamespace
namespace Prefabs.Mitya.Scripts
{
    public class HeadController : MonoBehaviour
    {
        [Header("Links")]
        public RotateController rotateController;

        [Header("Current orientation")]
        public float horizontalAngle;
        public float verticalAngle;
        [Tooltip("Angular speed in degrees per second")]
        public float horizontalAngularSpeed;
        [Tooltip("Angular speed in degrees per second")]
        public float verticalAngularSpeed;

        [Header("Robot specs")]
        public float horizontalDefaultAngle;
        public float verticalDefaultAngle;
        public float maxHorizontalAngularSpeed = 360f;
        public float maxVerticalAngularSpeed = 360f;

        private bool _isResettingOrientation;
        
        private void Awake()
        {
            Assert.IsNotNull(rotateController);
        }

        private void Update()
        {
            if (_isResettingOrientation) return;
            float deltaTime = Time.deltaTime;
            (horizontalAngle, horizontalAngularSpeed) = UpdateAxis(horizontalAngle, horizontalAngularSpeed, deltaTime,
                rotateController.yawMin, rotateController.yawMax, maxHorizontalAngularSpeed);
            (verticalAngle, verticalAngularSpeed) = UpdateAxis(verticalAngle, verticalAngularSpeed, deltaTime,
                rotateController.pitchMin, rotateController.pitchMax, maxVerticalAngularSpeed);

            rotateController.Yaw = horizontalAngle;
            rotateController.Pitch = verticalAngle;
        }

        private static (float angle, float angularSpeed) UpdateAxis(float currentAngle, float currentAngularSpeed,
            float deltaTime, float minAngle, float maxAngle, float maxAngularSpeed)
        {
            currentAngularSpeed = Mathf.Clamp(currentAngularSpeed, -maxAngularSpeed, maxAngularSpeed);
            float angle = currentAngle + currentAngularSpeed * deltaTime;
            if (angle < minAngle) return (angle: minAngle, angularSpeed: 0f);
            if (angle > maxAngle) return (angle: maxAngle, angularSpeed: 0f);
            return (angle, currentAngularSpeed);
        }

        public void ResetOrientation()
        {
            _isResettingOrientation = true;
            horizontalAngularSpeed = 0f;
            verticalAngularSpeed = 0f;
            horizontalAngle = horizontalDefaultAngle;
            verticalAngle = verticalDefaultAngle;
            _isResettingOrientation = false;
        }
    }
}