using UnityEngine;
using UnityEngine.Assertions;

// ReSharper disable once CheckNamespace
namespace Prefabs.Mitya.Scripts
{
    public class GamepadController : MonoBehaviour
    {
        public MoveController moveController;
        public HeadController headController;
        
        private InputControls _inputControls;

        private void Awake()
        {
            Assert.IsNotNull(moveController);
            Assert.IsNotNull(headController);
            _inputControls = new InputControls();
        }

        private void OnEnable()
        {
            _inputControls.Enable();
        }

        private void OnDisable()
        {
            _inputControls.Disable();
        }

        private void Update()
        {
            UpdateBodyMove();
            UpdateHeadRotate();
        }

        private void UpdateBodyMove()
        {
            bool fastRotation = _inputControls.TankActionMap.BodyRotationMode.IsPressed();
            float triggerSpeedFactor = fastRotation ? 1f : _inputControls.TankActionMap.BodySpeedFactor.ReadValue<float>();
            Vector2 moveVector = CircleToSquare(_inputControls.TankActionMap.BodyMove.ReadValue<Vector2>());
            float leftSpeedFactor = fastRotation
                ? moveVector.x / 2f
                : moveVector.x < 0f ? 1f + moveVector.x : 1f;
            float rightSpeedFactor = fastRotation
                ? -moveVector.x / 2f
                : moveVector.x > 0f ? 1f - moveVector.x : 1f;
            float directionSpeedFactor = fastRotation ? 1f : moveVector.y;

            float leftSpeed = triggerSpeedFactor * directionSpeedFactor * leftSpeedFactor * moveController.maxSpeed;
            float rightSpeed = triggerSpeedFactor * directionSpeedFactor * rightSpeedFactor * moveController.maxSpeed;
            moveController.speedFL = leftSpeed;
            moveController.speedRL = leftSpeed;
            moveController.speedFR = rightSpeed;
            moveController.speedRR = rightSpeed;
        }
        
        private void UpdateHeadRotate()
        {
            if (_inputControls.TankActionMap.HeadReset.WasPressedThisFrame())
            {
                headController.ResetOrientation();
            }
            
            Vector2 headAngularSpeedFactors = CircleToSquare(_inputControls.TankActionMap.HeadRotate.ReadValue<Vector2>());
            headAngularSpeedFactors.x = Mathf.Pow(headAngularSpeedFactors.x, 3);
            headAngularSpeedFactors.y = Mathf.Pow(headAngularSpeedFactors.y, 3);
            headController.horizontalAngularSpeed = headController.maxHorizontalAngularSpeed * headAngularSpeedFactors.x;
            headController.verticalAngularSpeed = headController.maxVerticalAngularSpeed * headAngularSpeedFactors.y;
        }
        
        private static Vector2 CircleToSquareInFirstQuadrant(Vector2 value)
        {
            float x = value.x;
            float y = value.y;
            if (x == 0) return value; // (to avoid dividing by 0)
            if (x >= 0 && y >= 0)
            {
                bool firstOctantInQuadrant = x >= y;
                if (!firstOctantInQuadrant) (x, y) = (y, x);
                float resultX = Mathf.Sqrt(x * x + y * y);
                float resultY = y * resultX / x;
                value = new Vector2(resultX, resultY);
                if (!firstOctantInQuadrant) (value.x, value.y) = (value.y, value.x);
            }
            return value;
        }

        private static Vector2 CircleToSquare(Vector2 value)
        {
            if (value is {x: >= 0, y: >= 0})
            {
                value = CircleToSquareInFirstQuadrant(value);
            }
            else if (value is {x: < 0, y: >= 0})
            {
                value.x = -value.x;
                value = CircleToSquareInFirstQuadrant(value);
                value.x = -value.x;
            }
            else if (value is {x: < 0, y: < 0})
            {
                value.x = -value.x;
                value.y = -value.y;
                value = CircleToSquareInFirstQuadrant(value);
                value.x = -value.x;
                value.y = -value.y;
            }
            else if (value is {x: >= 0, y: < 0})
            {
                value.y = -value.y;
                value = CircleToSquareInFirstQuadrant(value);
                value.y = -value.y;
            }
            return new Vector2(Mathf.Clamp(value.x, -1f, 1f), Mathf.Clamp(value.y, -1f, 1f));
        }

    }
}