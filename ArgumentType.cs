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
        private const string AA_Name_Name = "AA_Name";              /// Name of target block.
        private const string AA_Tag_Name = "AA_Tag";                /// Tag of target blocks. Note: Tag blocks using the following format: "[%TAG%]" (without quotes)
        private const string AA_Group_Name = "AA_Group";            /// Group name of target blocks.

        // Log arguments
        private const string AA_LogName_Name = "AA_LogName";        /// Name of TextPanel/LCD used as log output.
        private const string AA_LogTag_Name = "AA_LogTag";          /// Tag of TextPanels/LCDs used as log output.
        private const string AA_LogGroup_Name = "AA_LogGroup";      /// Group name of TextPanels/LCDs used as log output.
        private const string AA_LogLines_Name = "AA_LogLines";      /// Number of lines that can be displayed on a single panel. (default is 22 lines for FontSize = 0.8)

        // Memory arguments
        private const string AA_Memory_Name = "AA_Memory";             /// Name of TextPanel/LCD used as memory storage.
        ///TODO: Add Memory auto-recovery argument (command will automatically rewrites its value to default)

        // RotateCommand arguments
        private const string AA_ByAngle_Name = "AA_ByAngle";        /// Rotation angle. Specifies for how much rotor should be rotated.
        private const string AA_ToAngle_Name = "AA_ToAngle";        /// Rotor angle. Specifies exact angle for rotor.
        private const string AA_Velocity_Name = "AA_Velocity";      /// Rotor velocity.

        // ConsoleCommand arguments
        private const string AA_SingleMode_Name = "AA_SingleMode";  /// Flag indicating whether ConsoleCommand should execute single command per run or all commands at one run.

        // Base arguments
        public static ArgumentType AA_Name { get { return AA_Name_Name; } }        
        public static ArgumentType AA_Tag { get { return AA_Tag_Name; } }          
        public static ArgumentType AA_Group { get { return AA_Group_Name; } }      

        // Log arguments
        public static ArgumentType AA_LogName { get { return AA_LogName_Name; } }  
        public static ArgumentType AA_LogTag { get { return AA_LogTag_Name; } }    
        public static ArgumentType AA_LogGroup { get { return AA_LogGroup_Name; } }
        public static ArgumentType AA_LogLines { get { return AA_LogLines_Name; } }

        // Memory arguments
        public static ArgumentType AA_Memory { get { return AA_Memory_Name; } }

        // RotateCommand arguments
        public static ArgumentType AA_ByAngle { get { return AA_ByAngle_Name; } }  
        public static ArgumentType AA_ToAngle { get { return AA_ToAngle_Name; } }  
        public static ArgumentType AA_Velocity { get { return AA_Velocity_Name; } } 

        // ConsoleCommand arguments
        public static ArgumentType AA_SingleMode { get { return AA_SingleMode_Name; } }

        private static List<ArgumentType> arguments;
        static ArgumentType()
        {
            arguments = new List<ArgumentType>();
            arguments.Add(new ArgumentType(AA_Name_Name));
            arguments.Add(new ArgumentType(AA_Tag_Name));
            arguments.Add(new ArgumentType(AA_Group_Name));
            arguments.Add(new ArgumentType(AA_LogName_Name));
            arguments.Add(new ArgumentType(AA_LogTag_Name));
            arguments.Add(new ArgumentType(AA_LogGroup_Name));
            arguments.Add(new ArgumentType(AA_LogLines_Name));
            arguments.Add(new ArgumentType(AA_Memory_Name));
            arguments.Add(new ArgumentType(AA_Velocity_Name));
            arguments.Add(new ArgumentType(AA_ByAngle_Name));
            arguments.Add(new ArgumentType(AA_ToAngle_Name));
            arguments.Add(new ArgumentType(AA_SingleMode_Name));
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
