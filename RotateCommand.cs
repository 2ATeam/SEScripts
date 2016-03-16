﻿using System;
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

        public RotateCommand(MyGridProgram environment, params Argument[] args) : base(environment, CommandType.AA_Rotate, args) {}

        // Add angle arguments to acceptable list.
        protected override bool IsAcceptableArgument(ArgumentType arg)
        {
            return (arg == ArgumentType.AA_ByAngle || arg == ArgumentType.AA_ToAngle || base.IsAcceptableArgument(arg));
        }

        // Validate angle arguments.
        protected override void Validate()
        {
            base.Validate();
            PrimaryArgumentAngle = GetPrimaryArgument(ArgumentType.AA_ToAngle, ArgumentType.AA_ByAngle);
            if (PrimaryArgumentAngle == null)
            {
                Log(string.Format("Missed angle argument (either  {0} or {1} must be set)", ArgumentType.AA_ToAngle, ArgumentType.AA_ByAngle));
                IsValid = false;
                return;
            }

            float angleValue;
            if (!float.TryParse(PrimaryArgumentAngle.Value, out angleValue)) Log("Invalid angle format");
        }

        public override void Run(MyGridProgram environment)
        {
            if (!IsValid) { Log(string.Format("Command {0} is invalid", Type)); return; }
            float angle = float.Parse(PrimaryArgumentAngle.Value);
            bool cumulative = (PrimaryArgumentAngle.Type == ArgumentType.AA_ByAngle);
            if (PrimaryArgumentKey.Type == ArgumentType.AA_Group)
            {
                IMyBlockGroup group = Environment.GridTerminalSystem.GetBlockGroupWithName(PrimaryArgumentKey.Value);
                if (group == null) { Log(string.Format("Specified rotor group not found: {0}", PrimaryArgumentKey.Value)); return; }
                for (int i = 0; i < group.Blocks.Count; i++)
                {
                    IMyMotorStator rotor = group.Blocks[i] as IMyMotorStator;
                    if (rotor != null) RotateRotor(rotor, angle, cumulative);
                    else Log(string.Format("Rotating group {0} : Group has non-rotor block.", PrimaryArgumentKey.Value));
                }
            }
            if (PrimaryArgumentKey.Type == ArgumentType.AA_Name)
            {
                IMyMotorStator rotor = Environment.GridTerminalSystem.GetBlockWithName(PrimaryArgumentKey.Value) as IMyMotorStator;
                if (rotor != null) RotateRotor(rotor, angle, cumulative);
                else { Log(string.Format("Specified rotor not found: {0}", PrimaryArgumentKey.Value)); return; }
            }
            if (PrimaryArgumentKey.Type == ArgumentType.AA_Tag)
            {
               // List<IMyMotorStator> rotors = Utils.GetBlocksWithTag<IMyMotorStator>(PrimaryArgumentKey.Value, environment);
                List<IMyMotorStator> rotors = Utils.GetRotorsWithTag(PrimaryArgumentKey.Value, environment);
                if (rotors.Count == 0) { Log(string.Format("No rotors with tag \"{0}\" were found", PrimaryArgumentKey.Value)); return; }
                for (int i = 0; i < rotors.Count; i++)
                    RotateRotor(rotors[i], angle, cumulative);
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
            Log(string.Format("Angle: {0}", curAngle));
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
