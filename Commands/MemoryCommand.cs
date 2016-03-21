using System;
using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;

namespace SE_Mods.CommandRunner.Commands
{
    abstract class MemoryCommand : Command
    {
        private const string MEMORY_PATTERN = "^\\s*(?:({0})\\s*=\\s*([^=/]+)\\s*)\\s*(?://(.*))?$";

        /// <summary>
        /// Argument which is used to identify memory blocks.
        /// </summary>
        protected Argument PrimaryArgumentMemory { get; set; }


        protected MemoryCommand(MyGridProgram environment, CommandType type, params Argument[] args) : base(environment, type, args) { }

        /// <summary>
        /// Checks whether this command is setup and ready to run.
        /// </summary>
        protected override void Validate()
        {
            PrimaryArgumentMemory = GetPrimaryArgument(ArgumentType.AA_Memory);
            if (PrimaryArgumentMemory == null)
            {
                Log(string.Format("Missed memory argument ({0})", ArgumentType.AA_Memory));
                IsValid = false;
                return;
            }
        }

        protected override bool IsAcceptableArgument(ArgumentType arg)
        {
            return arg == ArgumentType.AA_Memory || base.IsAcceptableArgument(arg);
        }

        protected void Memorize(string varName, object value)
        {
            List<string> memories = ReadMemories();
            var mem = GetMemory(varName, memories);
            bool exists = (mem.Key != -1);
            string newMem = string.Format("{0} = {1} // [{4}] {2} by {3}.", varName, value.ToString(), (exists? "Modified" : "Added"), Environment.Me.CustomName, DateTime.Now.ToLongTimeString());
            if (exists) memories[mem.Key] = newMem;
            else memories.Add(newMem);
            WriteMemories(memories);

        }

        /// <summary>
        /// Returns memorized value of specified variable.
        /// </summary>
        /// <param name="name">Name of the variable.</param>
        /// <returns>Returns string value or null.</returns>
        protected string GetMemory(string name) { return GetMemory(name, ReadMemories()).Value; }

        /// <summary>
        /// Retrieves memories array from primary memory storage block.
        /// </summary>
        private List<string> ReadMemories()
        {
            IMyTextPanel panel = Environment.GridTerminalSystem.GetBlockWithName(PrimaryArgumentMemory.Value) as IMyTextPanel;
            if (panel == null) { Environment.Echo(string.Format("Specified text panel (LCD) not found: {0}.", PrimaryArgumentMemory.Value)); return null; }
            return new List<string>(Utils.GetLines(panel, Utils.HasTag(panel, Tag.AA_LogPublic)));
        }

        private void WriteMemories(List<string> memories)
        {
            IMyTextPanel panel = Environment.GridTerminalSystem.GetBlockWithName(PrimaryArgumentMemory.Value) as IMyTextPanel;
            if (panel == null) { Environment.Echo(string.Format("Specified text panel (LCD) not found: {0}.", PrimaryArgumentMemory.Value)); return; }
            bool isPublic = Utils.HasTag(panel, Tag.AA_LogPublic);
            string text = string.Join("\n", memories);
            if (isPublic) panel.WritePublicText(text);
            else panel.WritePrivateText(text);
        }

        /// <summary>
        /// Gets pair of memorized value and its' index. <para/>
        /// Note: If such memory wasn't found it will return (-1; null) pair.
        /// </summary>
        private KeyValuePair<int, string> GetMemory(string name, List<string> memories)
        {
            for (int i = 0; i < memories.Count; i++)
            {
                System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(memories[i], BuildVariablePattern(name));
                if (m.Success)
                    return new KeyValuePair<int, string>(i, m.Groups[2].Value.Trim());
            }
            return new KeyValuePair<int, string>(-1, null);
        }

        private string BuildVariablePattern(string varName)
        {
            return string.Format(MEMORY_PATTERN, varName);
        }

    }
}
