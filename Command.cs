using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace SE_Mods.CommandRunner
{
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

        public MyGridProgram Environment { get; protected set; }

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
        public abstract void Run(MyGridProgram environment = null);

        /// <summary>
        /// Checks whether this argument is acceptable by current command.
        /// </summary>
        /// <param name="arg"> Argument type passed to this command.</param>
        /// <returns>Returns flag indicating whether this argument type acceptable or not.</returns>
        protected virtual bool IsAcceptableArgument(ArgumentType arg)
        {
            return (arg == ArgumentType.AA_Group || arg == ArgumentType.AA_Name || arg == ArgumentType.AA_Tag || arg == ArgumentType.AA_LogName);
        }

       
        /// <summary>
        /// Logs message to specified panel if set and echoes it to programmable block output.
        /// </summary>
        /// <param name="message">Message.</param>
        protected void Log(string message)
        {
            string logPanel = arguments.GetValueOrDefault(ArgumentType.AA_LogName).Value;
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
}
