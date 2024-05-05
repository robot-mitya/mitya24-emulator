using UnityEngine;

namespace Prefabs.Mitya.Scripts
{
    public class Pid
    {
        public enum Direction
        {
            Normal,
            Reverse
        }

        public struct Limits
        {
            public float Min;
            public float Max;
        }
        
        public float Kp;
        public float Ki;
        public float Kd;

        public float Setpoint = 0;
        public float Input = 0;
        
        public float Output { get; private set; }

        private readonly Direction _direction;
        private Limits? _outputLimits;
        private float _prevInput;
        private float _integral;

        public Pid(Direction direction)
        {
            _direction = direction;
        }

        public Pid(float kp, float ki, float kd, Direction direction)
        {
            Kp = kp;
            Ki = ki;
            Kd = kd;
            _direction = direction;
        }

        public void SetLimits(float minOutput, float maxOutput)
        {
            _outputLimits = new Limits {Min = minOutput, Max = maxOutput};
        }

        public void ClearLimits()
        {
            _outputLimits = null;
        }

        public void Update(float deltaTime)
        {
            float error = Setpoint - Input;
            float deltaInput = _prevInput - Input;
            _prevInput = Input;
            if (_direction == Direction.Reverse)
            {
                error = -error;
                deltaInput = -deltaInput;
            }

            Output = error * Kp;
            _integral += error * Ki * deltaTime;
            Output += _integral;
            Output += deltaInput * Kd / deltaTime;

            if (_outputLimits != null)
                Output = Mathf.Clamp(Output, ((Limits) _outputLimits).Min, ((Limits) _outputLimits).Max);
        }
    }
}
