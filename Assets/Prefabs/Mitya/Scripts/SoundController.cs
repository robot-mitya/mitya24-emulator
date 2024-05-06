using UnityEngine;
using UnityEngine.Assertions;

namespace Prefabs.Mitya.Scripts
{
    public class SoundController : MonoBehaviour
    {
        [Header("Motors")]
        public AudioSource motorSound;

        public MoveController moveController;

        [Range(-3f, 3f)]
        public float motorSoundMinPitch = 1f;
        [Range(-3f, 3f)]
        public float motorSoundMaxPitch = 3f;

        [Header("Servos")]
        public AudioSource servoSound;

        public HeadController headController;

        [Range(-3f, 3f)]
        public float servoSoundMinPitch = 1f;
        [Range(-3f, 3f)]
        public float servoSoundMaxPitch = 2f;

        private bool _isMotorSoundPlaying;
        private bool _isServoSoundPlaying;
        
        private void Awake()
        {
            Assert.IsNotNull(motorSound);
            Assert.IsNotNull(moveController);
            Assert.IsNotNull(servoSound);
            Assert.IsNotNull(headController);
        }

        private void Update()
        {
            UpdateMotorSound();
            UpdateServoSound();
        }

        private void UpdateMotorSound()
        {
            if (motorSoundMaxPitch < motorSoundMinPitch) motorSoundMaxPitch = motorSoundMinPitch;
            float[] speeds =
            {
                Mathf.Abs(moveController.speedFL),
                Mathf.Abs(moveController.speedFR),
                Mathf.Abs(moveController.speedRL),
                Mathf.Abs(moveController.speedRR)
            };
            float speedFactor = Mathf.Clamp01(Mathf.Max(speeds) / moveController.maxSpeed);
            bool isMotorWorking = speedFactor > 0.01f;
            if (isMotorWorking != _isMotorSoundPlaying)
            {
                if (isMotorWorking) motorSound.Play();
                else motorSound.Stop();
                _isMotorSoundPlaying = isMotorWorking;
            }

            if (isMotorWorking)
            {
                motorSound.pitch = motorSoundMinPitch + (motorSoundMaxPitch - motorSoundMinPitch) * speedFactor;
            }
        }
        
        private void UpdateServoSound()
        {
            if (servoSoundMaxPitch < servoSoundMinPitch) servoSoundMaxPitch = servoSoundMinPitch;
            float[] speeds =
            {
                Mathf.Abs(headController.RealHorizontalAngularSpeed),
                Mathf.Abs(headController.RealVerticalAngularSpeed)
            };
            float maxSpeed = Mathf.Max(headController.maxHorizontalAngularSpeed, headController.maxVerticalAngularSpeed);
            float speedFactor = Mathf.Clamp01(Mathf.Max(speeds) / maxSpeed);
            bool isServoWorking = speedFactor > 0.001f;
            if (isServoWorking != _isServoSoundPlaying)
            {
                if (isServoWorking) servoSound.Play();
                else servoSound.Stop();
                _isServoSoundPlaying = isServoWorking;
            }

            if (isServoWorking)
            {
                servoSound.pitch = servoSoundMinPitch + (servoSoundMaxPitch - servoSoundMinPitch) * speedFactor;
            }
        }
    }
}
