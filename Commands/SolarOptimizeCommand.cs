using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame; // solar panels

namespace SE_Mods.CommandRunner.Commands
{
    class SolarOptimizeCommand : MemoryCommand
    {
        private const string VAR_TEST = "TestVar";

        public SolarOptimizeCommand(MyGridProgram environment, params Argument[] args) : base(environment, CommandType.AA_SolarOptimize, args) { }

        public override void Run(MyGridProgram environment = null)
        {
            int val = 0;
            string mem = GetMemory(VAR_TEST);
            if (mem != null) {
                Log(string.Format("Found memorized variable {0} with value {1}.", VAR_TEST, mem));
                if (!int.TryParse(mem, out val))
                    Log(string.Format("Memorized variable {0} is corrupted.", VAR_TEST));
            }
            Log(string.Format("Updating variable {0} with value.", VAR_TEST, val+1));
            Memorize(VAR_TEST, ++val);
        }
    }
}
