using UnityEngine;
using UnityEngine.Assertions;

// ReSharper disable InconsistentNaming
namespace Prefabs.Mitya.Scripts
{
    public class WheelSpeedController
    {
        private static readonly float Epsilon = 0.00001f;
        private readonly Transform _wheelModel;
        private readonly float _wheelCircumference;

        private float _speed;
        private float _angleSpeed;
    
        public WheelSpeedController(float wheelDiameter, Transform wheelModel)
        {
            _wheelModel = wheelModel;
            _wheelCircumference = Mathf.PI * wheelDiameter;
        }

        public void Update(float speed, float deltaTime)
        {
            if (Mathf.Abs(speed - _speed) > Epsilon)
            {
                _speed = speed;
                _angleSpeed = Mathf.Abs(_wheelCircumference) < Epsilon ? 0 : speed * 360f / _wheelCircumference;
            }

            _wheelModel.Rotate(Vector3.left, _angleSpeed * deltaTime);
        }
    }

    public class WheelController : MonoBehaviour
    {
        [Tooltip("Wheel diameter in meters")]
        public float wheelDiameter = 0.08f;
    
        [Tooltip("Wheel models")]
        public Transform wheelFL; // front-left
        public Transform wheelFR; // left-right
        public Transform wheelRL; // rear-left
        public Transform wheelRR; // rear-right

        [Tooltip("Wheel speeds in meters per second")]
        public float speedFL;
        public float speedFR;
        public float speedRL;
        public float speedRR;
    
        private WheelSpeedController _speedControllerFL;
        private WheelSpeedController _speedControllerFR;
        private WheelSpeedController _speedControllerRL;
        private WheelSpeedController _speedControllerRR;

        private void Awake()
        {
            Assert.IsNotNull(wheelFL);
            Assert.IsNotNull(wheelFR);
            Assert.IsNotNull(wheelRL);
            Assert.IsNotNull(wheelRR);
            _speedControllerFL = new WheelSpeedController(wheelDiameter, wheelFL);
            _speedControllerFR = new WheelSpeedController(wheelDiameter, wheelFR);
            _speedControllerRL = new WheelSpeedController(wheelDiameter, wheelRL);
            _speedControllerRR = new WheelSpeedController(wheelDiameter, wheelRR);
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            _speedControllerFL.Update(speedFL, deltaTime);
            _speedControllerFR.Update(speedFR, deltaTime);
            _speedControllerRL.Update(speedRL, deltaTime);
            _speedControllerRR.Update(speedRR, deltaTime);
        }
    }
}