using System;
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;

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
        protected IMyTerminalBlock[] Targets { get; set; }

        /// <summary>
        /// Argument which is used to identify log output panels.
        /// </summary>
        private IMyTextPanel[] LogPanels { get; set; }

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
            PrepareLog();
            Validate();
            if (IsValid)
                Prepare();
        }

        /// <summary>
        /// This is internal Prepare() method used to prepare logging before any other arguments. To allow using logging in main Prepare() method.
        /// </summary>
        private void PrepareLog()
        {
            Argument log = GetPrimaryArgument(ArgumentType.Log);
            if (log != null)
            {
                IMyTerminalBlock[] panels = Utils.FindBlocks<IMyTextPanel>(log.Value, Environment);
                if (panels == null || panels.Length == 0) { Environment.Echo(string.Format("Couldn't find any TextPanel/LCD with log key {0}.", log.Value));}
                else LogPanels = Array.ConvertAll(panels, block => block as IMyTextPanel);
                
            }
            else Environment.Echo("Log output wasn't set.");
        }

        /// <summary>
        /// Prepares command for execution. <para/>
        /// Note: This is a good place to read arguments and 
        /// </summary>
        protected virtual void Prepare()
        {
            Argument target = GetPrimaryArgument(ArgumentType.Target);
            IMyTerminalBlock[] blocks = Utils.FindBlocks<IMyTerminalBlock>(target.Value, Environment);
            if (blocks == null || blocks.Length == 0) { Environment.Echo(string.Format("Couldn't find any target blocks with key {0}.", target.Value)); IsValid = false;}
            else Targets = blocks;
        }

        /// <summary>
        /// Validates all received arguments. <para/>
        /// Note: This is a right place to check is there any required arguments missing.
        /// </summary>
        protected virtual void Validate()
        {
            IsValid &= GetPrimaryArgument(ArgumentType.Target) != null;
            if (!IsValid) Log(string.Format("Missed {0} argument", ArgumentType.Target));
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
            return (arg == ArgumentType.Target || arg == ArgumentType.Log);
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
                const int LOG_LINES = 17; // default number of lines with fontSize = 1.0f;
                float font = panel.GetValue<float>("FontSize");
                int logLines = (int)Math.Floor(LOG_LINES / font);
                panel.SetShowOnScreen(isPublic ? VRage.Game.GUI.TextPanel.ShowTextOnScreenFlag.PUBLIC : VRage.Game.GUI.TextPanel.ShowTextOnScreenFlag.PRIVATE);
                List<string> lines = new List<string>(Utils.GetLines(panel, isPublic));
                Environment.Echo(string.Format("LOG LINES : {0}.", logLines));
                lines.Add(string.Format("[{0}] : {1}", DateTime.Now.ToLongTimeString(), message));
                while (lines.Count > logLines) lines.RemoveAt(0);
                string text = string.Join("\n", lines);
                if (isPublic) panel.WritePublicText(text);
                else panel.WritePrivateText(text);
            }
            
        }

        private void ChainLog(string message, IMyTextPanel[] panels)
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
            if (LogPanels == null) return;
            if (LogPanels.Length == 1) LogToPanel(LogPanels[0], message);
            else ChainLog(message, LogPanels);
        }

        public override string ToString()
        {
            List<Argument> args = new List<Argument>(Arguments);
            string output = Type.ToString() + " [";
            for (int i = 0; i < args.Count; ++i)
                output += args[i].ToString() + "; ";
            output = output.TrimEnd(new char[] { ' ', ';' }) + "]";
            return output;
        }
    }
}
