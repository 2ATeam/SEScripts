using System.Collections.Generic;

namespace SE_Mods.CommandRunner
{
    /// <summary>
    /// Defines commands' arguments.
    /// Note: This is replacement for enums, since enum is not supported in Space Engineers Scripting.
    /// </summary>
    class ArgumentType
    {
        // Names of arguments
        private const string AA_Name_Name = "AA_Name";          // Name
        private const string AA_Tag_Name = "AA_Tag";            // Tag
        private const string AA_Log_Name = "AA_Log";            // Log
        private const string AA_Group_Name = "AA_Group";        // Group name
        private const string AA_ByAngle_Name = "AA_ByAngle";    // Rotate arg. Rotate by angle
        private const string AA_ToAngle_Name = "AA_ToAngle";    // Rotate arg. Rotate to angle

        // Commands
        public static ArgumentType AA_Name { get { return AA_Name_Name; } }              // Name
        public static ArgumentType AA_Tag { get { return AA_Tag_Name; } }           // Tag
        public static ArgumentType AA_Log { get { return AA_Log_Name; } }      // Log lines
        public static ArgumentType AA_Group { get { return AA_Group_Name; } }       // Group name
        public static ArgumentType AA_ByAngle { get { return AA_ByAngle_Name; } }   // Rotate arg. Rotate by angle
        public static ArgumentType AA_ToAngle { get { return AA_ToAngle_Name; } }   // Rotate arg. Rotate to angle

        private static List<ArgumentType> arguments;
        static ArgumentType()
        {
            arguments = new List<ArgumentType>();
            arguments.Add(new ArgumentType(AA_Name_Name));
            arguments.Add(new ArgumentType(AA_Tag_Name));
            arguments.Add(new ArgumentType(AA_Log_Name));
            arguments.Add(new ArgumentType(AA_Group_Name));
            arguments.Add(new ArgumentType(AA_ByAngle_Name));
            arguments.Add(new ArgumentType(AA_ToAngle_Name));
        }
        public int Value { get; private set; }
        public string Name { get; private set; }

        private ArgumentType(string name) : this(arguments.Count, name) {}
        private ArgumentType(int value, string name) { Value = value; Name = name; }

        public static implicit operator int(ArgumentType type) { return type.Value; }
        public static implicit operator ArgumentType(int value) { return arguments.Find(e => e.Value == value); }
        public static implicit operator string(ArgumentType type) { return type.Name; }
        public static implicit operator ArgumentType(string name) { return arguments.Find(e => e.Name == name); }
    }
}
