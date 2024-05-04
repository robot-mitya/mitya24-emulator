using UnityEngine;
using UnityEngine.Assertions;

// ReSharper disable once CheckNamespace
namespace Prefabs.Mitya.Scripts
{
    public class MoveController : MonoBehaviour
    {
        [Header("Links")]
        public WheelController wheelController;
        public Transform robotTransform;
        // public Transform tempPoint;
        
        [Header("Robot specs")]
        public float robotBaseWidth = 0.26f;
        // public float robotBaseLength = 0.24f;
        [Tooltip("Max speed in meters per second")]
        public float maxSpeed = 1.0f;
        
        [Header("Wheel Speeds")]
        [Tooltip("Wheel speed in meters per second")]
        // ReSharper disable once InconsistentNaming
        public float speedFL;
        [Tooltip("Wheel speed in meters per second")]
        // ReSharper disable once InconsistentNaming
        public float speedRL;
        [Tooltip("Wheel speed in meters per second")]
        // ReSharper disable once InconsistentNaming
        public float speedFR;
        [Tooltip("Wheel speed in meters per second")]
        // ReSharper disable once InconsistentNaming
        public float speedRR;

        private const float RadToDeg = 180f / Mathf.PI;
        
        private void Awake()
        {
            Assert.IsNotNull(wheelController);
            Assert.IsNotNull(robotTransform);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            wheelController.speedFL = speedFL;
            wheelController.speedFR = speedFR;
            wheelController.speedRL = speedRL;
            wheelController.speedRR = speedRR;

            float speedLeft = GetSideSpeed(speedFL, speedRL);
            float speedRight = GetSideSpeed(speedFR, speedRR);

            float moveLinearSpeed = GetMoveLinearSpeed(speedLeft, speedRight);
            float rotationAngularSpeed = GetRotateAngularSpeed(speedLeft, speedRight, robotBaseWidth);
            
            Vector4 localRotationOrigin = GetRotateLocalOrigin(speedLeft, speedRight, robotBaseWidth);
            Vector3 rotationOrigin = robotTransform.localToWorldMatrix * localRotationOrigin;
            Vector3 rotationAxis = robotTransform.localToWorldMatrix * Vector3.up;
            robotTransform.RotateAround(rotationOrigin,  rotationAxis, rotationAngularSpeed * deltaTime);
            
            Vector3 localMoveVector = moveLinearSpeed * deltaTime * Vector3.forward;
            Vector3 moveVector = robotTransform.localToWorldMatrix * localMoveVector;
            robotTransform.position += moveVector;

            // Debug.DrawRay(robotMoveModel.position, moveVector.normalized, Color.red);
            // Debug.Log($"linerSpeed: {moveLinearSpeed:F2}    angularSpeed: {rotationAngularSpeed:F2}    localRotationOrigin: {localRotationOrigin}    rotationOrigin: {rotationOrigin}    rotationAxis: {rotationAxis}");
            // tempPoint.position = rotationOrigin;
        }

        private static float GetSideSpeed(float speedFront, float speedRear)
        {
            if (speedFront >= 0f && speedRear >= 0f) return Mathf.Min(speedFront, speedRear);
            if (speedFront <= 0f && speedRear <= 0f) return Mathf.Max(speedFront, speedRear);
            return speedFront + speedRear;
        }

        private static float GetMoveLinearSpeed(float speedLeft, float speedRight)
        {
            if (speedLeft >= 0f && speedRight >= 0f) return Mathf.Min(speedLeft, speedRight);
            if (speedLeft <= 0f && speedRight <= 0f) return Mathf.Max(speedLeft, speedRight);
            return speedLeft + speedRight;
        }

        private static float GetRotateAngularSpeed(float speedLeft, float speedRight, float robotBaseWidth)
        {
            float rotateLinearSpeed = speedLeft - speedRight;
            if (Mathf.Approximately(robotBaseWidth, 0f)) return 0f;
            float rotateRadius = speedLeft * speedRight >= 0 ? robotBaseWidth : robotBaseWidth / 2f;
            return rotateLinearSpeed / rotateRadius * RadToDeg; // degrees per second
        }

        private static Vector4 GetRotateLocalOrigin(float speedLeft, float speedRight, float robotBaseWidth)
        {
            Vector4 result;
            float r = robotBaseWidth / 2f;
            Vector3 right = Vector3.right;
            if (speedLeft >= 0f && speedRight >= 0f) result = speedRight - speedLeft >= 0 ? -r * right : r * right;
            else if (speedLeft <= 0f && speedRight <= 0f) result = speedRight - speedLeft >= 0 ? r * right : -r * right;
            else result = Vector3.zero;
            result.w = 1;
            return result;
        }
    }
}