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
        private const string Rotate_Name = "Rotate";          /// Rotation command designed to rotate target rotors to or by specified angle.
        private const string Console_Name = "Console";        /// Console command which will read commands line by line from specified console LCD.
        private const string SolarOptimize_Name = "Solar";    /// Solar optimization command desinged to maximize solar input power by manipulating supporting rotors.

        // Commands
        public static CommandType Rotate { get { return Rotate_Name; } }
        public static CommandType Console { get { return Console_Name; } }
        public static CommandType SolarOptimize { get { return SolarOptimize_Name; } }

        private static List<CommandType> commands;
        static CommandType()
        {
            commands = new List<CommandType>();
            commands.Add(new CommandType(Rotate_Name));
            commands.Add(new CommandType(Console_Name));
            commands.Add(new CommandType(SolarOptimize_Name));
        }
        public int Value { get; private set; }
        public string Name { get; private set; }

        private CommandType(string name) : this(commands.Count, name) { }
        private CommandType(int value, string name) { Value = value; Name = name; }

        public static implicit operator int(CommandType type) { return type.Value; }
        public static implicit operator CommandType(int value) { return commands.Find(e => e.Value == value); }
        public static implicit operator string(CommandType type) { return type.Name; }
        public static implicit operator CommandType(string name) { return commands.Find(e => e.Name.ToLower() == name.ToLower()); ; }
        public override string ToString() { return this; }
    }
}
