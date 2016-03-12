using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE_Mods.CommandRunner
{
    static class Utils
    {
        public static List<T> GetBlocksWithTag<T>(string tag, MyGridProgram environment) where T : IMyTerminalBlock
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            List<T> result = new List<T>();
            environment.GridTerminalSystem.GetBlocksOfType<T>(blocks);
            for (int i = 0; i < blocks.Count; i++)
            {
                T block = (T)blocks[i];
                if (block.CustomName.Contains("[" + tag.Trim() + "]"))
                    result.Add(block);
            }
            return result;
        }
    }
}
