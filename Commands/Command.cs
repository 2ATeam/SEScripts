using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace SE_Mods.CommandRunner.Commands
{
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

        public string Comment { get; set; }

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
            PrimaryArgumentLog = GetPrimaryArgument(ArgumentType.AA_LogGroup, ArgumentType.AA_LogName, ArgumentType.AA_LogTag);
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
            return (arg == ArgumentType.AA_Group || arg == ArgumentType.AA_Name || arg == ArgumentType.AA_Tag || arg == ArgumentType.AA_LogGroup || arg == ArgumentType.AA_LogName || arg == ArgumentType.AA_LogTag || arg == ArgumentType.AA_LogLines);
        }

       /// <summary>
       /// Logs message to panel.
       /// </summary>
       /// <param name="panel"></param>
       /// <param name="message"></param>
        private void LogToPanel(IMyTextPanel panel, string message)
        {
            if (panel != null)
            {
                bool isPublic = Utils.HasTag(panel, Tag.AA_LogPublic);
                const int LOG_LINES = 22;
                panel.SetShowOnScreen(isPublic ? VRage.Game.GUI.TextPanel.ShowTextOnScreenFlag.PUBLIC : VRage.Game.GUI.TextPanel.ShowTextOnScreenFlag.PRIVATE);
                List<string> lines = new List<string>(Utils.GetLines(panel, isPublic));
                Argument logLinesArg = GetPrimaryArgument(ArgumentType.AA_LogLines);
                int logLines = LOG_LINES;
                if (logLinesArg != null)
                {
                    int.TryParse(logLinesArg.Value, out logLines);
                    Environment.Echo(string.Format("Found Log param : {1} = {0}.", logLinesArg.Value, logLinesArg.Type));
                }
                if (logLines <= 0) logLines = LOG_LINES;
                Environment.Echo(string.Format("LOG LINES : {0}.", logLines));
                lines.Add(string.Format("[{0}] : {1}", DateTime.Now.ToLongTimeString(), message));
                while (lines.Count > logLines) lines.RemoveAt(0);
                string text = string.Join("\n", lines);
                if (isPublic) panel.WritePublicText(text);
                else panel.WritePrivateText(text);
            }
            
        }

        private void ChainLog(string message, List<IMyTextPanel> panels)
        {
            foreach (var panel in panels)
                LogToPanel(panel, message);
        }

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
                //IMyBlockGroup group = Environment.GridTerminalSystem.GetBlockGroupWithName(PrimaryArgumentLog.Value);
                //if (group != null) ChainLog(message, Utils.GetBlocksOfType<IMyTextPanel>(group.Blocks));
                //else Environment.Echo(string.Format("Specified text panels (LCDs) group not found: {0}", PrimaryArgumentLog.Value));
            }
            else if (PrimaryArgumentLog.Type == ArgumentType.AA_LogName)
            {
                IMyTextPanel panel = Environment.GridTerminalSystem.GetBlockWithName(PrimaryArgumentLog.Value) as IMyTextPanel;
                if (panel != null) LogToPanel(panel, message);
                else Environment.Echo(string.Format("Specified text panel (LCD) not found: {0}.", PrimaryArgumentLog.Value));
            }
            else if (PrimaryArgumentLog.Type == ArgumentType.AA_LogTag)
            {
                List<IMyTerminalBlock> panels = Utils.GetBlocksWithTag<IMyTextPanel>(PrimaryArgumentLog.Value, Environment);
                if (panels.Count != 0) ChainLog(message, panels.ConvertAll(block => block as IMyTextPanel));
                else Environment.Echo(string.Format("No text panels (LCDs) with tag \"{0}\" were found.", PrimaryArgumentLog.Value));
            }
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
