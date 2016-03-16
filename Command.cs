using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using Sandbox.Common.ObjectBuilders;

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
        /// Argument which is used to identify log output panels.
        /// </summary>
        private Argument PrimaryArgumentLog { get; set; }

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
            PrimaryArgumentKey = GetPrimaryArgument(ArgumentType.AA_Group, ArgumentType.AA_Name, ArgumentType.AA_Tag);
            PrimaryArgumentLog = GetPrimaryArgument(ArgumentType.AA_LogGroup, ArgumentType.AA_LogName, ArgumentType.AA_LogTag);
            Validate();
        }

        /// <summary>
        /// Checks whether this command is setup and ready to run.
        /// </summary>
        protected virtual void Validate()
        {
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
            return (arg == ArgumentType.AA_Group || arg == ArgumentType.AA_Name || arg == ArgumentType.AA_Tag || arg == ArgumentType.AA_LogGroup || arg == ArgumentType.AA_LogName || arg == ArgumentType.AA_LogTag);
        }

       /// <summary>
       /// Logs message to panel
       /// </summary>
       /// <param name="panel"></param>
       /// <param name="message"></param>
        private void LogToPanel(IMyTextPanel panel, string message)
        {
            if (panel != null)
            {
                bool toPublic = Utils.HasTag(panel, Tag.AA_LogPublic);
                panel.SetShowOnScreen(toPublic ? ShowTextOnScreenFlag.PUBLIC : ShowTextOnScreenFlag.PRIVATE);
                List<string> lines = new List<string>(GetLines(panel));
                Argument logLinesArg = arguments[ArgumentType.AA_LogLines];
                int logLines = 22;
                if (logLinesArg != null) int.TryParse(logLinesArg.Value, out logLines);
                lines.Add(string.Format("[{0}] : {1}.", DateTime.Now.ToString(), message));
                while (lines.Count > logLines) lines.RemoveAt(0);

                if (toPublic) panel.WritePublicText(string.Join("\n", lines));
                else panel.WritePrivateText(string.Join("\n", lines));
            }
            
        }

        private void ChainLog(string message, List<IMyTextPanel> panels)
        {
            foreach (var panel in panels)
                LogToPanel(panel, message);
        }

        private string[] GetLines(IMyTextPanel panel) { return panel.GetPrivateText().Split('\n'); }


        /// <summary>
        /// Logs message to specified panel if set and echoes it to programmable block output.
        /// </summary>
        /// <param name="message">Message.</param>
        protected void Log(string message)
        {
            Environment.Echo(message);
            if (PrimaryArgumentLog == null) { Environment.Echo("Log output wasn't set."); return; }

            if (PrimaryArgumentLog.Type == ArgumentType.AA_LogGroup)
            {
                IMyBlockGroup group = Environment.GridTerminalSystem.GetBlockGroupWithName(PrimaryArgumentLog.Value);
                if (group != null) ChainLog(message, Utils.GetBlocksOfType<IMyTextPanel>(group.Blocks));
                else Environment.Echo(string.Format("Specified text panels (LCDs) group not found: {0}", PrimaryArgumentLog.Value));
            }
            else if (PrimaryArgumentLog.Type == ArgumentType.AA_LogName)
            {
                IMyTextPanel panel = Environment.GridTerminalSystem.GetBlockWithName(PrimaryArgumentLog.Value) as IMyTextPanel;
                if (panel != null) LogToPanel(panel, message);
                else Log(string.Format("Specified text panel (LCD) not found: {0}", PrimaryArgumentLog.Value));
            }
            else if (PrimaryArgumentLog.Type == ArgumentType.AA_LogTag)
            {
                List<IMyTextPanel> panels = GetTextPanelsWithTag(PrimaryArgumentLog.Value);
                if (panels.Count != 0) ChainLog(message, panels);
                else Environment.Echo(string.Format("No text panels (LCDs) with tag \"{0}\" were found", PrimaryArgumentLog.Value));
            }
        }

        /// FIXME: Hope this method is temp workaround while there is ModAPI bug with generics...
        private List<IMyTextPanel> GetTextPanelsWithTag(string tag)
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            List<IMyTextPanel> result = new List<IMyTextPanel>();
            Environment.GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(blocks);
            for (int i = 0; i < blocks.Count; i++)
            {
                IMyTextPanel block = blocks[i] as IMyTextPanel;
                if (Utils.HasTag(block, tag)) result.Add(block);
            }
            return result;
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
