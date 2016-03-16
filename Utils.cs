using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;

namespace SE_Mods.CommandRunner
{
    static class Utils
    {
        /// <summary>
        /// Finds blocks of given type which have specified tag.
        /// </summary>
        /// <typeparam name="T">Filtering type of blocks.</typeparam>
        /// <param name="tag">Filtering tag.</param>
        /// <param name="environment">Environment to look up blocks. </param>
        /// <returns>Returns either list of found blocks or empty list. </returns>
        public static List<IMyMotorStator> GetRotorsWithTag(string tag, MyGridProgram environment)
        {
            environment.Echo("Line 1");
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            List<IMyMotorStator> result = new List<IMyMotorStator>();
            environment.Echo("Created lists");
            environment.GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(blocks);
            environment.Echo("Retrieved blocks");
            for (int i = 0; i < blocks.Count; i++)
            {
                IMyMotorStator block = blocks[i] as IMyMotorStator;
                if (block != null && block.CustomName.Contains(string.Format("[{0}]", tag.Trim())))
                    result.Add(block);
            }
            return result;
        }

        /// <summary>
        /// Finds blocks of given type which have specified tag.
        /// </summary>
        /// <typeparam name="T">Filtering type of blocks.</typeparam>
        /// <param name="tag">Filtering tag.</param>
        /// <param name="environment">Environment to look up blocks. </param>
        /// <returns>Returns either list of found blocks or empty list. </returns>
        //public static List<T> GetBlocksWithTag<T>(string tag, MyGridProgram environment) where T : IMyTerminalBlock
        //{
        //    environment.Echo("Line 1");
        //    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
        //    List<T> result = new List<T>();
        //    environment.Echo("Created lists");
        //    environment.GridTerminalSystem.GetBlocksOfType<T>(blocks);
        //    environment.Echo("Retrieved blocks");
        //    for (int i = 0; i < blocks.Count; i++)
        //    {
        //        T block = (T)blocks[i];
        //        if (block != null && block.CustomName.Contains(string.Format("[{0}]", tag.Trim())))
        //            result.Add(block);
        //    }
        //    return result;
        //}

    }
}
