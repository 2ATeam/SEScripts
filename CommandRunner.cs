using Sandbox.ModAPI.Ingame;
using SE_Mods.CommandRunner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SE_Mods.CommandRunner
{
    // This class extends MyGridProgram which represents actual in-game script. 
    // This encapsulates environment available to the script.
    
        
    /// <summary>
    /// This script parses and runs available commands with agruments. 
    /// </summary>
    class CommandRunnerScript : MyGridProgram
    {
        protected void Main(string args)
        {
            var c = CommandBuilder.BuildCommand(args);
            c.Run(this);
            
        }

        ///// Test stuff here
        //public static void Main(string[] agrs)
        //{
        //    new CommandRunnerScript().Main("AA_Rotate [AA_Name : ROTOR 1 ; AA_Angle : 10 ]");
        //    Console.Read();
        //}
    }
}
