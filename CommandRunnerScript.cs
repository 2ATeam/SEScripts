using Sandbox.ModAPI.Ingame;

namespace SE_Mods.CommandRunner
{
    // This class extends MyGridProgram which represents actual in-game script. 
    // This encapsulates environment available to the script.

    class CommandRunnerScript : MyGridProgram
    {
        /// <summary>
        /// This script parses and runs available commands with agruments. 
        /// </summary>
        public void Main(string args)
        {
            var c = CommandBuilder.BuildCommand(this, args);
            if (c != null) c.Run();
            else Echo("Invalid command string.");
        }
    }
}
