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
        public static IMyTerminalBlock[] GetBlocksOfType<T>(IEnumerable<IMyTerminalBlock> blocks)
        {
            List<IMyTerminalBlock> result = new List<IMyTerminalBlock>();
            foreach (var block in blocks)
                if ((T)block != null) result.Add(block);
            return result.ToArray();       
        }

        // Really hope they will fix generics crash... At least for now we have temp workaround with IMyTerminalBlock
        /// <summary>
        /// Finds blocks of given type which have specified tag.
        /// </summary>
        /// <typeparam name="T">Filtering type of blocks.</typeparam>
        /// <param name="tag">Filtering tag.</param>
        /// <param name="environment">Environment to look up blocks. </param>
        /// <returns>Returns either list of found blocks or empty list. </returns>
        public static IMyTerminalBlock[] GetBlocksWithTag<T>(string tag, MyGridProgram environment)
        {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            if (environment != null)
            {
                environment.GridTerminalSystem.GetBlocksOfType<T>(blocks);
                for (int i = blocks.Count - 1; i >= 0; i--)
                    if (!HasTag(blocks[i], tag)) blocks.Remove(blocks[i]);
            }
            return blocks.ToArray();
        }

        /// <summary>
        /// Searches 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchKey"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static IMyTerminalBlock[] FindBlocks<T>(string searchKey, MyGridProgram environment)
        {
            if (environment != null)
            {
                IMyTerminalBlock block = environment.GridTerminalSystem.GetBlockWithName(searchKey);
                if (block != null && ((T)block != null)) return new IMyTerminalBlock[] { block };
                else
                {
                    IMyBlockGroup group = environment.GridTerminalSystem.GetBlockGroupWithName(searchKey);
                    if (group != null) return GetBlocksOfType<T>(group.Blocks);
                    else
                    {
                        IMyTerminalBlock[] blocks = GetBlocksWithTag<T>(searchKey, environment);
                        if (blocks.Length > 0) return blocks;
                    }
                }
            }
            return null;
        }


        public static string[] GetLines(IMyTextPanel panel, bool fromPublic) { return (fromPublic ? panel.GetPublicText() : panel.GetPrivateText()).Split('\n'); }
    }
}
