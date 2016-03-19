using System.Collections.Generic;

namespace SE_Mods.CommandRunner
{
    /// <summary>
    /// Defines commands.
    /// Note: This is a replacement for enums, since enum is not supported in Space Engineers Scripting.
    /// </summary>
    sealed class CommandType
    {
        // Names of commands
        private const string AA_Rotate_Name = "AA_Rotate";    /// Rotation command designed to rotate target rotors to or by specified angle.
        private const string AA_Console_Name = "AA_Console";  /// Console command which will read commands line by line from specified console LCD.

        // Commands
        public static CommandType AA_Rotate { get { return AA_Rotate_Name; } }
        public static CommandType AA_Console { get { return AA_Console_Name; } }

        private static List<CommandType> commands;
        static CommandType()
        {
            commands = new List<CommandType>();
            commands.Add(new CommandType(AA_Rotate_Name));
            commands.Add(new CommandType(AA_Console_Name));
        }
        public int Value { get; private set; }
        public string Name { get; private set; }

        private CommandType(string name) : this(commands.Count, name) { }
        private CommandType(int value, string name) { Value = value; Name = name; }

        public static implicit operator int(CommandType type) { return type.Value; }
        public static implicit operator CommandType(int value) { return commands.Find(e => e.Value == value); }
        public static implicit operator string(CommandType type) { return type.Name; }
        public static implicit operator CommandType(string name) { return commands.Find(e => e.Name == name); ; }
    }
}
