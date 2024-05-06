using Prefabs.Display.Scripts;
using UnityEngine;
using UnityEngine.Assertions;

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

        private bool _orienting;
        private float _targetHorizontalAngle;
        private float _targetVerticalAngle;
        private float _orientSpeedFactor = 1f;
        private const float Epsilon = 0.01f;

        private float _horizontalAngularSpeed;
        private float _verticalAngularSpeed;

        public float RealHorizontalAngularSpeed => _horizontalAngularSpeed;
        public float RealVerticalAngularSpeed => _verticalAngularSpeed;

        private Pid _horizontalPid;
        private Pid _verticalPid;

        [Header("PID Controller Factors")]
        public float horizontalKp;
        public float horizontalKi;
        public float horizontalKd;
        public float verticalKp;
        public float verticalKi;
        public float verticalKd;
        
        private void Awake()
        {
            Assert.IsNotNull(rotateController);
            _horizontalPid = new Pid(Pid.Direction.Normal);
            _verticalPid = new Pid(Pid.Direction.Normal);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            _horizontalPid.Kp = horizontalKp;
            _horizontalPid.Ki = horizontalKi;
            _horizontalPid.Kd = horizontalKd;
            _horizontalPid.SetLimits(-maxHorizontalAngularSpeed * _orientSpeedFactor, maxHorizontalAngularSpeed * _orientSpeedFactor);
            _verticalPid.Kp = verticalKp;
            _verticalPid.Ki = verticalKi;
            _verticalPid.Kd = verticalKd;
            _verticalPid.SetLimits(-maxVerticalAngularSpeed * _orientSpeedFactor, maxVerticalAngularSpeed * _orientSpeedFactor);
            
            if (_orienting)
            {
                _horizontalPid.Input = horizontalAngle;
                _horizontalPid.Update(deltaTime);
                _horizontalAngularSpeed = _horizontalPid.Output;
                
                _verticalPid.Input = verticalAngle;
                _verticalPid.Update(deltaTime);
                _verticalAngularSpeed = _verticalPid.Output;
            
                if (Mathf.Abs(horizontalAngle - _targetHorizontalAngle) < Epsilon &&
                    Mathf.Abs(verticalAngle - _targetVerticalAngle) < Epsilon)
                {
                    _orienting = false;
                    _horizontalAngularSpeed = 0f;
                    _verticalAngularSpeed = 0f;
                }
            }
            else
            {
                _horizontalAngularSpeed = horizontalAngularSpeed;
                _verticalAngularSpeed = verticalAngularSpeed;
            }
            
            (horizontalAngle, horizontalAngularSpeed) = UpdateAxis(horizontalAngle, _horizontalAngularSpeed, deltaTime,
                rotateController.yawMin, rotateController.yawMax, maxHorizontalAngularSpeed);
            (verticalAngle, verticalAngularSpeed) = UpdateAxis(verticalAngle, _verticalAngularSpeed, deltaTime,
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

        public void Orient(float targetHorizontalAngle, float targetVerticalAngle, float speedFactor = 1f)
        {
            _targetHorizontalAngle = targetHorizontalAngle;
            _targetVerticalAngle = targetVerticalAngle;
            _orientSpeedFactor = Mathf.Clamp01(speedFactor);

            _horizontalPid.Setpoint = _targetHorizontalAngle;
            _verticalPid.Setpoint = _targetVerticalAngle;

            _orienting = true;
        }
    }
}