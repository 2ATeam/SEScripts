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

        public RotateCommand(MyGridProgram environment, params Argument[] args) : base(environment, CommandType.AA_Rotate, args)
        {
            PrimaryArgumentAngle = GetPrimaryArgument(ArgumentType.AA_ToAngle, ArgumentType.AA_ByAngle);
        }

        // Add angle arguments to acceptable list.
        protected override bool IsAcceptableArgument(ArgumentType arg)
        {
            return (arg == ArgumentType.AA_ByAngle || arg == ArgumentType.AA_ToAngle || arg == ArgumentType.AA_Velocity || base.IsAcceptableArgument(arg));
        }

        // Validate angle arguments.
        protected override void Validate()
        {
            base.Validate();
            if (PrimaryArgumentAngle == null)
            {
                Log(string.Format("Missed angle argument (either {0} or {1} must be set)", ArgumentType.AA_ToAngle, ArgumentType.AA_ByAngle));
                IsValid = false;
                return;
            }

            float angleValue;
            if (!float.TryParse(PrimaryArgumentAngle.Value, out angleValue))
            {
                Log("Invalid angle format");
                IsValid = false;
            }
        }

        public override void Run(MyGridProgram environment = null)
        {
            if (environment != null) Environment = environment;
            if (!IsValid) { Log(string.Format("Command {0} is invalid", Type)); return; }
            float angle = float.Parse(PrimaryArgumentAngle.Value);
            bool cumulative = (PrimaryArgumentAngle.Type == ArgumentType.AA_ByAngle);
            float velocity = 0;
            Argument velocityArg = arguments[ArgumentType.AA_Velocity];
            if (velocityArg != null) float.TryParse(velocityArg.Value, out velocity);
            if (PrimaryArgumentKey.Type == ArgumentType.AA_Group)
            {
                IMyBlockGroup group = Environment.GridTerminalSystem.GetBlockGroupWithName(PrimaryArgumentKey.Value);
                if (group == null) { Log(string.Format("Specified rotor group not found: {0}", PrimaryArgumentKey.Value)); return; }
                List<IMyMotorStator> rotors = Utils.GetBlocksOfType<IMyMotorStator>(group.Blocks);
                if (rotors.Count < group.Blocks.Count) Log(string.Format("Rotating group {0} has non-rotor block.", PrimaryArgumentKey.Value));
                foreach (var rotor in rotors) RotateRotor(rotor, angle, cumulative, velocity);
            }
            else if (PrimaryArgumentKey.Type == ArgumentType.AA_Name)
            {
                IMyMotorStator rotor = Environment.GridTerminalSystem.GetBlockWithName(PrimaryArgumentKey.Value) as IMyMotorStator;
                if (rotor != null) RotateRotor(rotor, angle, cumulative, velocity);
                else { Log(string.Format("Specified rotor not found: {0}", PrimaryArgumentKey.Value)); return; }
            }
            else if (PrimaryArgumentKey.Type == ArgumentType.AA_Tag)
            {
               // List<IMyMotorStator> rotors = Utils.GetBlocksWithTag<IMyMotorStator>(PrimaryArgumentKey.Value, environment);
                List<IMyMotorStator> rotors = GetRotorsWithTag(PrimaryArgumentKey.Value);
                if (rotors.Count == 0) { Log(string.Format("No rotors with tag \"{0}\" were found", PrimaryArgumentKey.Value)); return; }
                foreach (var rotor in rotors) RotateRotor(rotor, angle, cumulative, velocity);
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
            if (velocity == 0) velocity = 1.0f; // set default velocity.
            string limit = (angle < 0 ? "LowerLimit" : "UpperLimit");
            float curAngle = rotor.GetValueFloat(limit);
            float resAngle = (cumulativeAngle ? (curAngle + angle) : angle);
            resAngle -= (int)(resAngle / 360) * 360.0f; // subtracting full circles.
            Log(string.Format("Set angular limits to {0}. {1}", resAngle, (velocity != 1.0f ? string.Format("Set custom velocity: {0}", velocity) : "")));
            rotor.SetValueFloat("LowerLimit", resAngle);
            rotor.SetValueFloat("UpperLimit", resAngle);
            rotor.SetValueFloat("Velocity", Math.Abs(velocity) * Math.Sign(resAngle - curAngle));
        }

        /// FIXME: Hope this method is temp workaround while there is ModAPI bug with generics...
        private List<IMyMotorStator> GetRotorsWithTag(string tag)
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            List<IMyMotorStator> result = new List<IMyMotorStator>();
            Environment.GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(blocks);
            for (int i = 0; i < blocks.Count; i++)
            {
                IMyMotorStator block = blocks[i] as IMyMotorStator;
                if (Utils.HasTag(block, tag)) result.Add(block);
            }
            return result;
        }
    }
}
