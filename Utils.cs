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
        public static List<IMyTerminalBlock> GetBlocksOfType<T>(List<IMyTerminalBlock> blocks)
        {
            List<IMyTerminalBlock> result = new List<IMyTerminalBlock>();
            foreach (var block in blocks)
                if ((T)block != null) result.Add(block);
            return result;       
        }

        // Really hope they will fix generics crash...
        /// <summary>
        /// Finds blocks of given type which have specified tag.
        /// </summary>
        /// <typeparam name="T">Filtering type of blocks.</typeparam>
        /// <param name="tag">Filtering tag.</param>
        /// <param name="environment">Environment to look up blocks. </param>
        /// <returns>Returns either list of found blocks or empty list. </returns>
        public static List<IMyTerminalBlock> GetBlocksWithTag<T>(string tag, MyGridProgram environment)
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            environment.GridTerminalSystem.GetBlocksOfType<T>(blocks);
            environment.Echo(string.Format("Found {0} blocks of type {1}", blocks.Count, typeof(T).Name));
            for (int i = blocks.Count - 1; i >= 0; i--)
                if (!HasTag(blocks[i], tag)) blocks.Remove(blocks[i]);
            return blocks;
        }


        public static string[] GetLines(IMyTextPanel panel, bool fromPrivate) { return (fromPrivate ? panel.GetPrivateText() : panel.GetPublicText()).Split('\n'); }
    }
}
}
