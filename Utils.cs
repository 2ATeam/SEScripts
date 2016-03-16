using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;

namespace SE_Mods.CommandRunner
{
    static class Utils
    {
        
        /// <summary>
        /// Checks does target block have specified tag or not.
        /// </summary>
        /// <param name="block">Target block.</param>
        /// <param name="tag">Tag to search.</param>
        /// <returns>Returns true if taget block has tag in its' name. Otherwise - false.</returns>
        public static bool HasTag(IMyTerminalBlock block, string tag) { return block != null && block.CustomName.Contains(string.Format("[{0}]", tag.Trim())); }

        /// <summary>
        /// Retrieves blocks of specified type from source list of IMyTerminalBlock objects.
        /// </summary>
        /// <typeparam name="T">Type of dest blocks.</typeparam>
        /// <param name="blocks">List of all available blocks.</param>
        /// <returns>Returns list of blocks of specified type.</returns>
        public static List<T> GetBlocksOfType<T>(List<IMyTerminalBlock> blocks)
        {
            List<T> res = new List<T>();
            foreach (T block in blocks)
                if (block != null) res.Add(block);
            return res;
        }

        // Really hope they will fix generics crash...
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
