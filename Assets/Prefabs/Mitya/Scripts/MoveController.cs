using UnityEngine;
using UnityEngine.Assertions;

// ReSharper disable once CheckNamespace
namespace Prefabs.Mitya.Scripts
{
    public class MoveController : MonoBehaviour
    {
        public Transform tempPoint;
        [Header("Links")]
        public WheelController wheelController;
        public Transform robotMoveModel;
        public Transform robotRotateModel;

        [Header("Move Geometry")]
        [Tooltip("Local vector")]
        public Vector3 moveRightVector = Vector3.right;
        [Tooltip("Local vector")]
        public Vector3 moveUpVector = Vector3.up;
        [Tooltip("Local vector")]
        public Vector3 moveForwardVector = Vector3.forward;
        
        [Header("Rotate Geometry")]
        public float robotBaseWidth = 0.26f;
        // public float robotBaseLength = 0.24f;
        [Tooltip("Local vector")]
        public Vector3 rotateRightVector = Vector3.right;
        [Tooltip("Local vector")]
        public Vector3 rotateUpVector = Vector3.up;
        [Tooltip("Local vector")]
        public Vector3 rotateForwardVector = Vector3.forward;
        
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
            Assert.IsNotNull(robotMoveModel);
            Assert.IsNotNull(robotRotateModel);
        }

        private void Update()
        {
            wheelController.speedFL = speedFL;
            wheelController.speedFR = speedFR;
            wheelController.speedRL = speedRL;
            wheelController.speedRR = speedRR;

            float speedLeft = GetSideSpeed(speedFL, speedRL);
            float speedRight = GetSideSpeed(speedFR, speedRR);

            float moveLinearSpeed = GetMoveLinearSpeed(speedLeft, speedRight);
            float rotationAngularSpeed = GetRotateAngularSpeed(speedLeft, speedRight, robotBaseWidth);
            Vector3 localRotationOrigin = GetRotateLocalOrigin(speedLeft, speedRight, rotateRightVector, robotBaseWidth);

            float deltaTime = Time.deltaTime;
            Vector3 localMoveVector = moveLinearSpeed * deltaTime * moveForwardVector;
            Vector3 moveVector = robotRotateModel.localToWorldMatrix * localMoveVector;
            // Debug.DrawRay(robotMoveModel.position, moveVector.normalized, Color.red);
            robotMoveModel.position += moveVector;
            Vector3 rotationOrigin = robotRotateModel.localToWorldMatrix * localRotationOrigin;
            Vector3 localRotationAxis = rotateUpVector;
            Vector3 rotationAxis = robotRotateModel.localToWorldMatrix * localRotationAxis;
            robotRotateModel.RotateAround(rotationOrigin,  rotationAxis, rotationAngularSpeed * deltaTime);
            Debug.Log($"linerSpeed: {moveLinearSpeed:F2}    angularSpeed: {rotationAngularSpeed:F2}    localRotationOrigin: {localRotationOrigin}    rotationOrigin: {rotationOrigin}    localRotationAxis: {localRotationAxis}    rotationAxis: {rotationAxis}");
            tempPoint.position = rotationOrigin;
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

        // private static float GetRotateLinearSpeed(float speedLeft, float speedRight)
        // {
        //     // if (speedLeft >= 0f && speedRight >= 0f) return speedLeft - speedRight;
        //     // if (speedLeft <= 0f && speedRight <= 0f) return speedLeft - speedRight;
        //     return speedLeft - speedRight;
        // }

        private static float GetRotateAngularSpeed(float speedLeft, float speedRight, float robotBaseWidth)
        {
            float rotateLinearSpeed = speedLeft - speedRight;
            if (Mathf.Approximately(robotBaseWidth, 0f)) return 0f;
            float rotateRadius = speedLeft * speedRight >= 0 ? robotBaseWidth : robotBaseWidth / 2f;
            // Debug.Log($"{rotateLinearSpeed:F2}    {rotateRadius:F2}    {Mathf.PI * rotateRadius / rotateLinearSpeed / 180}");
            return rotateLinearSpeed / rotateRadius * RadToDeg; // degrees per second
        }

        private static Vector3 GetRotateLocalOrigin(float speedLeft, float speedRight, Vector3 rightVector, float robotBaseWidth)
        {
            float r = robotBaseWidth / 2f;
            if (speedLeft >= 0f && speedRight >= 0f) return speedRight - speedLeft >= 0 ? r * rightVector : -r * rightVector;
            if (speedLeft <= 0f && speedRight <= 0f) return speedRight - speedLeft >= 0 ? -r * rightVector : r * rightVector;
            return Vector3.zero;
        }
    }
}