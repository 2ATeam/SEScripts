using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;

namespace SE_Mods.CommandRunner
{
    class RotateCommand : Command
    {
        /// <summary>
        /// Primary argument which is used to set rotation angle.
        /// </summary>
        protected Argument PrimaryArgumentAngle { get; set; }

        public RotateCommand(params Argument[] args) : base(CommandType.AA_Rotate, args) {}

        // Add angle arguments to acceptable list.
        protected override bool IsAcceptableArgument(ArgumentType arg)
        {
            switch (arg) { 
                case ArgumentType.AA_ByAngle:
                case ArgumentType.AA_ToAngle:
                    return true;
                default:
                    return base.IsAcceptableArgument(arg);
            }
        }

        // Validate angle arguments.
        protected override void Validate()
        {
            base.Validate();
            PrimaryArgumentAngle = GetPrimaryArgument(ArgumentType.AA_ToAngle, ArgumentType.AA_ByAngle);
            if (PrimaryArgumentAngle == null) throw new ArgumentException("Missed angle argument");

            float angleValue;
            if (!float.TryParse(PrimaryArgumentAngle.Value, out angleValue)) throw new ArgumentException("Invalid angle format");
        }

        public override void Run(MyGridProgram environment)
        {
            float angle = float.Parse(PrimaryArgumentAngle.Value);
            bool cumulative = (PrimaryArgumentAngle.Type == ArgumentType.AA_ByAngle);
            switch (PrimaryArgumentKey.Type)
            {
                case ArgumentType.AA_Group:
                    {
                        IMyBlockGroup group = environment.GridTerminalSystem.GetBlockGroupWithName(PrimaryArgumentKey.Value);
                        for (int i = 0; i < group.Blocks.Count; i++)
                        {
                            IMyMotorStator rotor = group.Blocks[i] as IMyMotorStator;
                            if (rotor != null) RotateRotor(rotor, angle, cumulative);
                        }
                        break;
                    }
                case ArgumentType.AA_Name:
                    {
                        IMyMotorStator rotor = environment.GridTerminalSystem.GetBlockWithName(PrimaryArgumentKey.Value) as IMyMotorStator;
                        if (rotor != null) RotateRotor(rotor, angle, cumulative);
                        break;
                    }
                case ArgumentType.AA_Tag:
                    {
                        List<IMyMotorStator> rotors = Utils.GetBlocksWithTag<IMyMotorStator>(PrimaryArgumentKey.Value, environment);
                        for (int i = 0; i < rotors.Count; i++)
                            RotateRotor(rotors[i], angle, cumulative);
                        break;
                    }
                default: break; // never appears
            }
        }
        /// <summary>
        /// Rotates specified rotor to/by specified angle with custom velocity.
        /// </summary>
        /// <param name="rotor">Target rotor.</param>
        /// <param name="angle">Desired angle.</param>
        /// <param name="cumulativeAngle">Flag which when set to true indicates that rotor should rotate by given angle, otherwise it will rotate to that angle.</param>
        /// <param name="velocity">Custom velocity of rotor.</param>
        protected void RotateRotor(IMyMotorStator rotor, float angle, bool cumulativeAngle = true, float velocity = 1.0f)
        {
            float curAngle = GetCurrentAngle(rotor);
            float resAngle = (cumulativeAngle ? (curAngle + angle) : angle) % 360;
            if (angle < 0) rotor.SetValueFloat("LowerLimit", resAngle);
            else if (angle > 0) rotor.SetValueFloat("UpperLimit", resAngle);
            rotor.SetValueFloat("Velocity", Math.Abs(velocity) * Math.Sign(angle));
        }

        protected float GetCurrentAngle(IMyMotorStator rotor)
        {
            string angle = System.Text.RegularExpressions.Regex.Replace(rotor.DetailedInfo, "[^.0-9]", "");
            return float.Parse(angle);
        }
    }
}
