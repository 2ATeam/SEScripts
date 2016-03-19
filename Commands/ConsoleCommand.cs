using System;
using Sandbox.ModAPI.Ingame;

namespace SE_Mods.CommandRunner.Commands
{
    class ConsoleCommand : Command
    {
        public ConsoleCommand(MyGridProgram environment, params Argument[] args) : base(environment, CommandType.AA_Console, args) { }

        protected override bool IsAcceptableArgument(ArgumentType arg)
        {
            return arg == ArgumentType.AA_Console || base.IsAcceptableArgument(arg);
        }

        protected override void Validate()
        {
            base.Validate();
            IsValid = (PrimaryArgumentKey.Type == ArgumentType.AA_Name);
        }

        public override void Run(MyGridProgram environment = null)
        {
            if (environment != null) Environment = environment;
            if (!IsValid) { Log(string.Format("Command {0} is invalid.", Type)); return; }
            IMyTextPanel panel = Environment.GridTerminalSystem.GetBlockWithName(PrimaryArgumentKey.Value) as IMyTextPanel;

            if (panel == null) { Log(string.Format("Specified TextPanel/LCD was not found ({0})", PrimaryArgumentKey.Value)); return; }

            bool isPublic = Utils.HasTag(panel, Tag.AA_LogPublic);
            string[] lines = Utils.GetLines(panel, !isPublic);

            foreach (var line in lines)
                CommandBuilder.BuildCommand(Environment, line.Trim()).Run();
        }
    }
}
