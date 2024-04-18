using System.Collections.Generic;
using UnityEngine;

namespace Prefabs.Display.Scripts
{
    public enum Axis { Right, Up, Forward }
    
    public class RotateController : MonoBehaviour
    {
        public float speedFactor = 100;

        [Header("Yaw")]
        public Transform yawObject;
        public Axis yawAxis = Axis.Up;
        public bool inverseYaw;
        public bool isYawLimited;
        public float yawMin;
        public float yawMax;

        [Header("Pitch")]
        public Transform pitchObject;
        public Axis pitchAxis = Axis.Right;
        public bool inversePitch;
        public bool isPitchLimited;
        public float pitchMin;
        public float pitchMax;

        [Header("Roll")]
        public Transform rollObject;
        public Axis rollAxis = Axis.Forward;
        public bool inverseRoll;
        public bool isRollLimited;
        public float rollMin;
        public float rollMax;

        [Header("Modifiers")]
        public bool shiftPressRequire;
        public bool ctrlPressRequire;

        private float _yaw;
        public float Yaw
        {
            get => _yaw;
            set
            {
                float newYaw = PrepareYaw(value);
                if (!Mathf.Approximately(newYaw, _yaw))
                {
                    _yaw = newYaw;
                    SetAngle(yawObject, yawAxis, _yaw);
                }
            }
        }

        private float _pitch;
        public float Pitch
        {
            get => _pitch;
            set
            {
                float newPitch = PreparePitch(value);
                if (!Mathf.Approximately(newPitch, _pitch))
                {
                    _pitch = newPitch;
                    SetAngle(pitchObject, pitchAxis, _pitch);
                }
            }
        }

        private float _roll;
        public float Roll
        {
            get => _roll;
            set
            {
                float newRoll = PrepareRoll(value);
                if (!Mathf.Approximately(newRoll, _roll))
                {
                    _roll = newRoll;
                    SetAngle(rollObject, rollAxis, _roll);
                }
            }
        }

        private float PrepareYaw(float value)
        {
            return isYawLimited ? Mathf.Clamp(value, yawMin, yawMax) : value;
        }

        private float PreparePitch(float value)
        {
            return isPitchLimited ? Mathf.Clamp(value, pitchMin, pitchMax) : value;
        }

        private float PrepareRoll(float value)
        {
            return isRollLimited ? Mathf.Clamp(value, rollMin, rollMax) : value;
        }

        private static Dictionary<Axis, Vector3> _axisVector = new()
        {
            { Axis.Right, Vector3.right },
            { Axis.Up, Vector3.up },
            { Axis.Forward, Vector3.forward }
        };

        private void Start()
        {
            _yaw = PrepareYaw(GetCurrentAngle(yawObject, yawAxis));
            _pitch = PreparePitch(GetCurrentAngle(pitchObject, pitchAxis));
            _roll = PrepareRoll(GetCurrentAngle(rollObject, rollAxis));
            // Debug.Log($"Start {name}: ({Yaw}, {Pitch}, {Roll})");
        }

        private static float GetCurrentAngle(Transform rotationObject, Axis axis)
        {
            if (rotationObject)
            {
                Vector3 axisVector = _axisVector[axis];
                Vector3 eulerAngle = Vector3.Scale(rotationObject.localEulerAngles, axisVector);
                return eulerAngle.x + eulerAngle.y + eulerAngle.z;
            }
            return 0;
        }

        private static void SetAngle(Transform rotationObject, Axis axis, float angle)
        {
            if (rotationObject)
            {
                rotationObject.localEulerAngles = angle * _axisVector[axis];
            }
        }

        private void Update()
        {
            if (!Input.GetMouseButton(1)) return; // 1 - right button
            
            bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            bool altPressed = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

            bool rollFlag = altPressed;
            bool noModifiers = !shiftPressRequire && !ctrlPressRequire && !shiftPressed && !ctrlPressed;
            bool shiftModifier = shiftPressRequire && !ctrlPressRequire && shiftPressed && !ctrlPressed;
            bool ctrlModifier = !shiftPressRequire && ctrlPressRequire && !shiftPressed && ctrlPressed;
            bool ctrlShiftModifier = shiftPressRequire && ctrlPressRequire && shiftPressed && ctrlPressed;
            if (noModifiers || shiftModifier || ctrlModifier || ctrlShiftModifier)
            {
                UpdateRotation(rollFlag);
                // Debug.Log($"Update {name}: ({Yaw}, {Pitch}, {Roll})");
            }
        }

        private void UpdateRotation(bool rollFlag)
        {
            float horizontal = Input.GetAxisRaw("Mouse X") * speedFactor * Time.deltaTime;
            if (rollFlag)
            {
                if (inverseRoll) horizontal *= -1f;
                Roll += horizontal;
            }
            else
            {
                if (inversePitch) horizontal *= -1f;
                Yaw += horizontal;
            }

            float vertical = Input.GetAxisRaw("Mouse Y") * speedFactor * Time.deltaTime;
            if (inversePitch) vertical *= -1f;
            Pitch += vertical;
        }
    }
}
