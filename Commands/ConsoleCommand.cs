using Sandbox.ModAPI.Ingame;

namespace SE_Mods.CommandRunner.Commands
{
    class ConsoleCommand : Command
    {
        public ConsoleCommand(MyGridProgram environment, params Argument[] args) : base(environment, CommandType.AA_Console, args) { }

        protected override bool IsAcceptableArgument(ArgumentType arg)
        {
            return arg == ArgumentType.AA_SingleMode || base.IsAcceptableArgument(arg);
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

            if (panel == null) { Log(string.Format("Specified text panel (LCD) was not found ({0}).", PrimaryArgumentKey.Value)); return; }
            string[] lines = Utils.GetLines(panel, Utils.HasTag(panel, Tag.AA_LogPublic));

            ///TODO: Implement single line mode.
            for (int i = 0; i < lines.Length; i++)
            { 
                var c = CommandBuilder.BuildCommand(Environment, lines[i].Trim());
                if (c != null) c.Run();
                else environment.Echo(string.Format("Not a command string at line {0}.", i + 1));
            }
        }
    }
}
