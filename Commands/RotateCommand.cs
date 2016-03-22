using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;

namespace SE_Mods.CommandRunner.Commands
{
    class RotateCommand : Command
    {
        /// <summary>
        /// Rotation angle.
        /// </summary>
        protected float Angle { get; set; }

        /// <summary>
        /// Rotation velocity.
        /// </summary>
        protected float Velocity { get; set; }
        protected const float DEFAULT_VELOCITY = 1.0f;

        /// <summary>
        /// Indicates whether angle should be interpreted as increment or exact value.
        /// </summary>
        protected bool IsCumulative { get; set; }

        public RotateCommand(MyGridProgram environment, params Argument[] args) : base(environment, CommandType.Rotate, args) {}

        protected override bool IsAcceptableArgument(ArgumentType arg)
        {
            return (arg == ArgumentType.ByAngle || arg == ArgumentType.ToAngle || arg == ArgumentType.Velocity || base.IsAcceptableArgument(arg));
        }

        protected override void Validate()
        {
            base.Validate();
            if (GetPrimaryArgument(ArgumentType.ToAngle, ArgumentType.ByAngle) == null)
            {
                Log(string.Format("Missed angle argument (either {0} or {1} must be set).", ArgumentType.ToAngle, ArgumentType.ByAngle));
                IsValid = false;
                return;
            }
        }

        protected override void Prepare()
        {
            base.Prepare();
            if (!IsValid) return;
            Targets = Utils.GetBlocksOfType<IMyMotorStator>(Targets);
            Argument arg = GetPrimaryArgument(ArgumentType.ToAngle, ArgumentType.ByAngle);
            float angle;
            IsValid &= float.TryParse(arg.Value, out angle);
            if (IsValid) Angle = angle;
            else { Log("Invalid angle format."); return; }

            IsCumulative = (arg.Type == ArgumentType.ByAngle);

            float velocity = 0;
            Argument velocityArg = GetPrimaryArgument(ArgumentType.Velocity);
            if (velocityArg != null) {
                if (float.TryParse(velocityArg.Value, out velocity) && velocity != 0) Velocity = velocity;
                else { Log(string.Format("Velocity value has invalid format. Will use default value {0}", DEFAULT_VELOCITY)); Velocity = DEFAULT_VELOCITY; }
            }

        }

        public override void Run(MyGridProgram environment = null)
        {
            if (environment != null) Environment = environment;
            if (!IsValid) { Log(string.Format("Command {0} is invalid", Type)); return; }
            foreach (IMyMotorStator rotor in Targets)
            {
                if (rotor != null)
                {
                    Log(string.Format("Rotating rotor {0}", rotor.CustomName));
                    RotateRotor(rotor, Angle, IsCumulative, Velocity);
                }
            }
        }

        /// <summary>
        /// Rotates specified rotor to/by specified angle with custom velocity.
        /// </summary>
        /// <param name="rotor">Target rotor.</param>
        /// <param name="angle">Desired angle.</param>
        /// <param name="cumulativeAngle">Flag which when set to true indicates that rotor should rotate by given angle, otherwise it will rotate to that angle.</param>
        /// <param name="velocity">Custom velocity of rotor.</param>
        protected void RotateRotor(IMyMotorStator rotor, float angle, bool cumulativeAngle = true, float velocity = DEFAULT_VELOCITY)
        {
            if (velocity == 0) velocity = DEFAULT_VELOCITY; // set default velocity.
            string limit = (angle < 0 ? "LowerLimit" : "UpperLimit");
            float curAngle = rotor.GetValue<float>(limit);
            float resAngle = (cumulativeAngle ? (curAngle + angle) : angle);
            resAngle -= (int)(resAngle / 360) * 360.0f; // subtracting full circles.
            Log(string.Format("Set angular limits to {0}. {1}", resAngle, (velocity != 1.0f ? string.Format("Set custom velocity: {0}.", velocity) : "")));
            rotor.SetValue("LowerLimit", resAngle);
            rotor.SetValue("UpperLimit", resAngle);
            rotor.SetValue("Velocity", Math.Abs(velocity) * Math.Sign(resAngle - curAngle));
        }
    }
}
