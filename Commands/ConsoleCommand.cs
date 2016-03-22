using Sandbox.ModAPI.Ingame;

namespace SE_Mods.CommandRunner.Commands
{
    class ConsoleCommand : Command
    {
        /// <summary>
        /// Reference to targeted TextPanel (LCD).
        /// </summary>
        protected IMyTextPanel Console { get { if (Targets != null && Targets.Length > 0) return Targets[0] as IMyTextPanel; else return null; } }

        public ConsoleCommand(MyGridProgram environment, params Argument[] args) : base(environment, CommandType.Console, args) { }

        protected override bool IsAcceptableArgument(ArgumentType arg)
        {
            return arg == ArgumentType.SingleMode || base.IsAcceptableArgument(arg);
        }

        protected override void Prepare()
        {
            base.Prepare();
            Targets = Utils.GetBlocksOfType<IMyTextPanel>(Targets);
        }

        public override void Run(MyGridProgram environment = null)
        {
            if (environment != null) Environment = environment;
            if (!IsValid) { Log(string.Format("Command {0} is invalid.", Type)); return; }
            if (Console == null) return; // shouldn't be the case.
            string[] lines = Utils.GetLines(Console, Utils.HasTag(Console, Tag.AA_LogPublic));

            ///TODO: Implement single line mode.
            Log(string.Format("Got {0} lines", lines.Length));
            for (int i = 0; i < lines.Length; i++)
            { 
                var c = CommandBuilder.BuildCommand(Environment, lines[i].Trim());
                if (c != null)
                {
                    c.Run();
                    Log(string.Format("{0} Command completed!", i+1));
                }
                else Log(string.Format("Not a command string at line {0}.", i + 1));
            }
        }
    }
}
