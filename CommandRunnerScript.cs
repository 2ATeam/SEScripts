using Sandbox.ModAPI.Ingame;

namespace SE_Mods.CommandRunner
{
    // This class extends MyGridProgram which represents actual in-game script. 
    // This encapsulates environment available to the script.

    public class CommandRunnerScript : MyGridProgram
    {
        /// <summary>
        /// This script parses and runs available commands with agruments. 
        /// </summary>
        public void Main(string args)
        {
            CommandBuilder.BuildCommand(this, args).Run();
        }
    }
}
