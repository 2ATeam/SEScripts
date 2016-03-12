using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            var c = CommandBuilder.BuildCommand(args);
            c.Run(this);

        }
    }
}
