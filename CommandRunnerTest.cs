using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE_Mods.CommandRunner
{
    class CommandRunnerTest
    {
        public static void Main(string[] args)
        {
            new CommandRunnerScript().Main("AA_Rotate [AA_Name : SM 1 Rotation; AA_ByAngle : 10; AA_Log: LOG]");
        }
    }
}
