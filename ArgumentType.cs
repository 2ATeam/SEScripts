using System.Collections.Generic;

namespace SE_Mods.CommandRunner
{
    /// <summary>
    /// Defines commands' arguments.
    /// Note: This is a replacement for enums, since enum is not supported in Space Engineers Scripting.
    /// </summary>
    sealed class ArgumentType
    {
        // Base arguments
        private const string Target_Name = "Target";          /// Search key for target block. It will search blocks in the following priority: name, group name or tag.

        // Log arguments
        private const string Log_Name = "Log";                /// Search key for TextPanel/LCD used as log output. It will search panels in the following priority: name, group name or tag.

        // Memory arguments
        private const string Memory_Name = "Memory";          /// Name of TextPanel/LCD used as memory storage.
        ///TODO: Add Memory auto-recovery argument (command will automatically rewrites its value to default if it is corrupted)

        // RotateCommand arguments
        private const string ByAngle_Name = "ByAngle";        /// Rotation angle. Specifies for how much rotor should be rotated.
        private const string ToAngle_Name = "ToAngle";        /// Rotor angle. Specifies exact angle for rotor.
        private const string Velocity_Name = "Velocity";      /// Rotor velocity.

        // ConsoleCommand arguments
        private const string SingleMode_Name = "SingleMode";  /// Flag indicating whether ConsoleCommand should execute single command per run or all commands at one run.

        // Base arguments
        public static ArgumentType Target { get { return Target_Name; } }        

        // Log arguments
        public static ArgumentType Log { get { return Log_Name; } }  

        // Memory arguments
        public static ArgumentType Memory { get { return Memory_Name; } }

        // RotateCommand arguments
        public static ArgumentType ByAngle { get { return ByAngle_Name; } }  
        public static ArgumentType ToAngle { get { return ToAngle_Name; } }  
        public static ArgumentType Velocity { get { return Velocity_Name; } } 

        // ConsoleCommand arguments
        public static ArgumentType SingleMode { get { return SingleMode_Name; } }

        private static List<ArgumentType> arguments;
        static ArgumentType()
        {
            arguments = new List<ArgumentType>();
            arguments.Add(new ArgumentType(Target_Name));
            arguments.Add(new ArgumentType(Log_Name));
            arguments.Add(new ArgumentType(Memory_Name));
            arguments.Add(new ArgumentType(Velocity_Name));
            arguments.Add(new ArgumentType(ByAngle_Name));
            arguments.Add(new ArgumentType(ToAngle_Name));
            arguments.Add(new ArgumentType(SingleMode_Name));
        }
        public int Value { get; private set; }
        public string Name { get; private set; }

        private ArgumentType(string name) : this(arguments.Count, name) {}
        private ArgumentType(int value, string name) { Value = value; Name = name; }

        public static implicit operator int(ArgumentType type) { return type.Value; }
        public static implicit operator ArgumentType(int value) { return arguments.Find(e => e.Value == value); }
        public static implicit operator string(ArgumentType type) { return type.Name; }
        public static implicit operator ArgumentType(string name) { return arguments.Find(e => e.Name.ToLower() == name.ToLower()); }
        public override string ToString() { return this; }
    }
}
