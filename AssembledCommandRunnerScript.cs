using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using System;
using Sandbox.ModAPI.Interfaces;

namespace Assembled.SE_Mods.CommandRunner
{
	class AssembledCommandRunnerScript : MyGridProgram
	{
        /// <summary>
        /// This script parses and runs available commands with agruments. 
        /// </summary>
        public void Main(string args)
        {
            var c = CommandBuilder.BuildCommand(this, args);
            c.Run(this);
        }

		// CommandType.cs
	    /// <summary>
	    /// Defines commands.
	    /// Note: This is replacement for enums, since enum is not supported in Space Engineers Scripting.
	    /// </summary>
	    class CommandType
	    {
	        // Names of commands
	        private const string AA_Rotate_Name = "AA_Rotate";
	
	        // Commands
	        public static CommandType AA_Rotate { get { return AA_Rotate_Name; } }  // Rotate. Applied to rotors.
	
	        private static List<CommandType> commands;
	        static CommandType()
	        {
	            commands = new List<CommandType>();
	            commands.Add(new CommandType(0, AA_Rotate_Name));
	        }
	        public int Value { get; private set; }
	        public string Name { get; private set; }
	
	        private CommandType(int value, string name) { Value = value; Name = name; }
	
	        public static implicit operator int(CommandType type) { return type.Value; }
	        public static implicit operator CommandType(int value) { return commands.Find(e => e.Value == value); }
	        public static implicit operator string(CommandType type) { return type.Name; }
	        public static implicit operator CommandType(string name) { return commands.Find(e => e.Name == name); ; }
	    }

		// ArgumentType.cs
	    /// <summary>
	    /// Defines commands' arguments.
	    /// Note: This is replacement for enums, since enum is not supported in Space Engineers Scripting.
	    /// </summary>
	    class ArgumentType
	    {
	        // Names of arguments
	        private const string AA_Name_Name = "AA_Name";          // Name
	        private const string AA_Tag_Name = "AA_Tag";            // Tag
	        private const string AA_Log_Name = "AA_Log";            // Log
	        private const string AA_Group_Name = "AA_Group";        // Group name
	        private const string AA_ByAngle_Name = "AA_ByAngle";    // Rotate arg. Rotate by angle
	        private const string AA_ToAngle_Name = "AA_ToAngle";    // Rotate arg. Rotate to angle
	
	        // Commands
	        public static ArgumentType AA_Name { get { return AA_Name_Name; } }              // Name
	        public static ArgumentType AA_Tag { get { return AA_Tag_Name; } }           // Tag
	        public static ArgumentType AA_Log { get { return AA_Log_Name; } }      // Log lines
	        public static ArgumentType AA_Group { get { return AA_Group_Name; } }       // Group name
	        public static ArgumentType AA_ByAngle { get { return AA_ByAngle_Name; } }   // Rotate arg. Rotate by angle
	        public static ArgumentType AA_ToAngle { get { return AA_ToAngle_Name; } }   // Rotate arg. Rotate to angle
	
	        private static List<ArgumentType> arguments;
	        static ArgumentType()
	        {
	            arguments = new List<ArgumentType>();
	            arguments.Add(new ArgumentType(AA_Name_Name));
	            arguments.Add(new ArgumentType(AA_Tag_Name));
	            arguments.Add(new ArgumentType(AA_Log_Name));
	            arguments.Add(new ArgumentType(AA_Group_Name));
	            arguments.Add(new ArgumentType(AA_ByAngle_Name));
	            arguments.Add(new ArgumentType(AA_ToAngle_Name));
	        }
	        public int Value { get; private set; }
	        public string Name { get; private set; }
	
	        private ArgumentType(string name) : this(arguments.Count, name) {}
	        private ArgumentType(int value, string name) { Value = value; Name = name; }
	
	        public static implicit operator int(ArgumentType type) { return type.Value; }
	        public static implicit operator ArgumentType(int value) { return arguments.Find(e => e.Value == value); }
	        public static implicit operator string(ArgumentType type) { return type.Name; }
	        public static implicit operator ArgumentType(string name) { return arguments.Find(e => e.Name == name); }
	    }

		// Argument.cs
	    class Argument
	    {
	        public ArgumentType Type { get; private set; }
	        public string Value { get; private set; }
	        public Argument(ArgumentType type, string value) { Type = type; Value = value; }
	        public override string ToString() { return Type.ToString() + " : " + Value; }
	    }

		// ArgumentBuilder.cs
	    static class ArgumentBuilder
	    {                                                                       //  G1          G2
	        private const string ARG_TEMPLATE = "([^:;]+)\\s*:\\s*([^:;]+)";    // ARG_NAME : ARG_VALUE
	
	        public static Argument BuildArgument(string strArg)
	        {
	            System.Text.RegularExpressions.Match match = new System.Text.RegularExpressions.Regex(ARG_TEMPLATE).Match(strArg);
	            if (match.Success)
	            {
	                string name = match.Groups[1].Value.Trim();
	                ArgumentType type = name;
	                if (type == null) throw new ArgumentException("Unknown argument: " + name);
	                string value = match.Groups[2].Value.Trim();
	                return new Argument(type, value);
	            }
	            return null;
	        }
	    }

		// Command.cs
	    //class ArgumentDictionary
	    //{
	    //    List<ArgumentType> types;
	    //    List<Argument> args;
	        
	    //    public List<Argument> Values { get { return args; } }
	
	    //    public void Add(ArgumentType type, Argument arg)
	    //    {
	    //        int index = -1;
	    //        if ((index = types.IndexOf(type)) > -1)
	    //        {
	    //            types.RemoveAt(index);
	    //            args.RemoveAt(index);
	    //        }
	    //        types.Add(type);
	    //        args.Add(arg);
	    //    }
	
	    //    public bool Contains(ArgumentType type)
	    //    {
	    //        return types.Contains(type);
	    //    }
	
	    //    public Argument GetValueOrDefault(ArgumentType type)
	    //    {
	    //        int index = -1;
	    //        if ((index = types.IndexOf(type)) > -1)
	    //        {
	    //            return args[index];
	    //        }
	    //        return null;
	    //    }
	    //}
	
	    abstract class Command
	    {
	        /// <summary>
	        /// Dictionary of all given arguments.
	        /// </summary>
	        protected readonly Dictionary<ArgumentType, Argument> arguments;
	
	        /// <summary>
	        /// Argument which is used to identify target blocks.
	        /// </summary>
	        protected Argument PrimaryArgumentKey { get; set; }
	
	        /// <summary>
	        /// Type of the command.
	        /// </summary>
	        public CommandType Type { get; protected set; }
	
	        public MyGridProgram Environment { get; private set; }
	
	        public bool IsValid { get; protected set; }
	
	        /// <summary>
	        /// Collection of defined arguments.
	        /// </summary>
	        public ICollection<Argument> Arguments { get { return arguments.Values; } }
	
	        protected Command(MyGridProgram environment, CommandType type, params Argument[] args)
	        {
	            Type = type;
	            Environment = environment;
	            if (Environment == null)
	            {
	                Log(string.Format("Command \"{0}\" received null environment!", type));
	                IsValid = false;
	            }
	            arguments = new Dictionary<ArgumentType, Argument>();
	            for (int i = 0; i < args.Length; ++i)
	            {
	                Argument arg = args[i];
	                if (IsAcceptableArgument(arg.Type))
	                    arguments.Add(arg.Type, arg);
	            }
	            IsValid = true;
	            Validate();
	        }
	
	        /// <summary>
	        /// Checks whether this command is setup and ready to run.
	        /// </summary>
	        protected virtual void Validate()
	        {
	            PrimaryArgumentKey = GetPrimaryArgument(ArgumentType.AA_Group, ArgumentType.AA_Name, ArgumentType.AA_Tag);
	            if (PrimaryArgumentKey == null)
	            {
	                Log(string.Format("Missed search key argument (at least one of {0}/{1}/{2} must be set)", ArgumentType.AA_Group, ArgumentType.AA_Name, ArgumentType.AA_Tag));
	                IsValid = false;
	            }
	        }
	
	
	        /// <summary>
	        /// Gets first defined argument of given types.
	        /// </summary>
	        /// <param name="priorityArguments"> Argument type array which defines lookup priority.</param>
	        /// <returns> Returns defined argument or null.</returns>
	        protected Argument GetPrimaryArgument(params ArgumentType[] priorityArguments)
	        {
	            ArgumentType primary;
	            for (int i = 0; i < priorityArguments.Length; i++)
	            {
	                primary = priorityArguments[i];
	                Argument arg = arguments.GetValueOrDefault(primary);
	                if (arg != null) return arg;
	            }
	
	            return null;
	        }
	
	        /// <summary>
	        /// Implements actually command's logic.
	        /// </summary>
	        /// <param name="environment"> Script environment.</param>
	        public abstract void Run(MyGridProgram environment);
	
	        /// <summary>
	        /// Checks whether this argument is acceptable by current command.
	        /// </summary>
	        /// <param name="arg"> Argument type passed to this command.</param>
	        /// <returns>Returns flag indicating whether this argument type acceptable or not.</returns>
	        protected virtual bool IsAcceptableArgument(ArgumentType arg)
	        {
	            return (arg == ArgumentType.AA_Group || arg == ArgumentType.AA_Name || arg == ArgumentType.AA_Tag || arg == ArgumentType.AA_Log);
	        }
	
	       
	        /// <summary>
	        /// Logs message to specified panel if set. In case critical message was received - throws excpetion of type T containing that message.
	        /// </summary>
	        /// <typeparam name="T">Type of exception to be thrown. </typeparam>
	        /// <param name="message">Message.</param>
	        /// <param name="isCritical">Flag indicating whether this message is critical to command execution. </param>
	        protected void Log(string message)
	        {
	            string logPanel = arguments.GetValueOrDefault(ArgumentType.AA_Log).Value;
	            IMyTextPanel panel = Environment.GridTerminalSystem.GetBlockWithName(logPanel) as IMyTextPanel;
	            if (panel != null)
	            {
	                panel.SetShowOnScreen(Sandbox.Common.ObjectBuilders.ShowTextOnScreenFlag.PRIVATE);
	                List<string> lines = new List<string>(panel.GetPrivateText().Split('\n'));
	
	                Environment.Echo(string.Format("Log has {0} lines.", lines.Count));
	                lines.Add(string.Format("[{0}] : {1}.", DateTime.Now.ToString(), message));
	                while (lines.Count > 22) lines.RemoveAt(0);
	                panel.WritePrivateText(string.Join("\n", lines));
	            }
	            Environment.Echo(message);
	        }
	        public override string ToString()
	        {
	            List<Argument> args = new List<Argument>(Arguments);
	            string output = Type.ToString() + " [";
	            for (int i = 0; i < args.Count; ++i)
	            {
	                output += args[i].ToString() + "; ";
	            }
	            output = output.TrimEnd(new char[] { ' ', ';' }) + "]";
	            return output;
	        }
	    }

		// RotateCommand.cs
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

		// CommandBuilder.cs
	    static class CommandBuilder
	    {                                                                                    // G1      G2
	        private static string CMD_TEMPLATE = "(.*)\\s*\\[\\s*(?:([^;]+)\\s*;?\\s*)*\\]"; // CMD [ARG1; ARG2]
	
	        /// <summary>
	        /// Builds command from given string. If any error occurs.
	        /// </summary>
	        /// <exception cref="ArgumentException">Throws ArgumentException if any parsing error occurs. </exception>
	        /// <param name="strCmd"> Incoming string argument.</param>
	        /// <returns> Returns command object which can be run. </returns>
	        public static Command BuildCommand(MyGridProgram environment, string strCmd)
	        {
	            System.Text.RegularExpressions.Match match = new System.Text.RegularExpressions.Regex(CMD_TEMPLATE).Match(strCmd);
	            if (match.Success)
	            {
	                Command cmd;
	                string name = match.Groups[1].Value.Trim();
	                CommandType type = name;
	                if (type == null) throw new ArgumentException("Unknown command: " + name);
	
	                List<Argument> arguments = new List<Argument>();
	                System.Text.RegularExpressions.Group args = match.Groups[2];
	                for (int i = 0; i < args.Captures.Count; ++i)
	                {
	                    arguments.Add(ArgumentBuilder.BuildArgument(args.Captures[i].Value));
	                }
	                return GetCommand(environment, type, arguments.ToArray());
	
	            }
	            else
	            {
	                throw new ArgumentException("Invalid command string");
	            }
	        }
	
	        private static Command GetCommand(MyGridProgram environment, CommandType type, params Argument[] args)
	        {
	            if (type == CommandType.AA_Rotate) return new RotateCommand(environment, args);
	
	            return null;
	        }
	    }

		// Utils.cs
	    static class Utils
	    {
	        /// <summary>
	        /// Finds blocks of given type which have specified tag.
	        /// </summary>
	        /// <typeparam name="T">Filtering type of blocks.</typeparam>
	        /// <param name="tag">Filtering tag.</param>
	        /// <param name="environment">Environment to look up blocks. </param>
	        /// <returns>Returns either list of found blocks or empty list. </returns>
	        public static List<IMyMotorStator> GetRotorsWithTag(string tag, MyGridProgram environment)
	        {
	            environment.Echo("Line 1");
	            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
	            List<IMyMotorStator> result = new List<IMyMotorStator>();
	            environment.Echo("Created lists");
	            environment.GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(blocks);
	            environment.Echo("Retrieved blocks");
	            for (int i = 0; i < blocks.Count; i++)
	            {
	                IMyMotorStator block = blocks[i] as IMyMotorStator;
	                if (block != null && block.CustomName.Contains(string.Format("[{0}]", tag.Trim())))
	                    result.Add(block);
	            }
	            return result;
	        }
	
	        /// <summary>
	        /// Finds blocks of given type which have specified tag.
	        /// </summary>
	        /// <typeparam name="T">Filtering type of blocks.</typeparam>
	        /// <param name="tag">Filtering tag.</param>
	        /// <param name="environment">Environment to look up blocks. </param>
	        /// <returns>Returns either list of found blocks or empty list. </returns>
	        //public static List<T> GetBlocksWithTag<T>(string tag, MyGridProgram environment) where T : IMyTerminalBlock
	        //{
	        //    environment.Echo("Line 1");
	        //    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
	        //    List<T> result = new List<T>();
	        //    environment.Echo("Created lists");
	        //    environment.GridTerminalSystem.GetBlocksOfType<T>(blocks);
	        //    environment.Echo("Retrieved blocks");
	        //    for (int i = 0; i < blocks.Count; i++)
	        //    {
	        //        T block = (T)blocks[i];
	        //        if (block != null && block.CustomName.Contains(string.Format("[{0}]", tag.Trim())))
	        //            result.Add(block);
	        //    }
	        //    return result;
	        //}
	
	    }

	}
}
