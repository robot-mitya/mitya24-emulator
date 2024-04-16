using UnityEngine;

namespace Prefabs.Display.Scripts
{
    public class RotateController : MonoBehaviour
    {
        public float xFactor = 200;
        public float yFactor = 200;

        [Header("Yaw")]
        public Transform yawObject;
        public Vector3 yawAxis = Vector3.up;
        public bool isYawLimited;
        public float yawMin;
        public float yawMax;

        [Header("Pitch")]
        public Transform pitchObject;
        public Vector3 pitchAxis = Vector3.right;
        public bool isPitchLimited;
        public float pitchMin;
        public float pitchMax;

        [Header("Roll")]
        public Transform rollObject;
        public Vector3 rollAxis = Vector3.forward;
        public bool isRollLimited;
        public float rollMin;
        public float rollMax;

        [Header("Modifiers")]
        public bool shiftPressRequire;
        public bool ctrlPressRequire;
    
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
            }
        }

        private void UpdateRotation(bool rollFlag)
        {
            float horizontal = Input.GetAxisRaw("Mouse X") * xFactor * Time.deltaTime;
            if (rollFlag)
            {
                if (isRollLimited) horizontal = Mathf.Clamp(horizontal, rollMin, rollMax);
                if (rollObject) rollObject.Rotate(rollAxis, horizontal);
            }
            else
            {
                if (isYawLimited) horizontal = Mathf.Clamp(horizontal, yawMin, yawMax);
                if (yawObject) yawObject.Rotate(yawAxis, horizontal);
            }

            float vertical = Input.GetAxisRaw("Mouse Y") * yFactor * Time.deltaTime;
            if (isPitchLimited) vertical = Mathf.Clamp(vertical, pitchMin, pitchMax);
            if (pitchObject) pitchObject.Rotate(pitchAxis, vertical);
        }
    }
}
