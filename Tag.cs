using System.Collections.Generic;

namespace SE_Mods.CommandRunner
{
    sealed class Tag
    {
        // Names of tags
        private const string AA_LogPrivate_Name = "AA_LogPrivate"; /// Tag for text panels (LCDs) to specify that panel should use private text to log;
        private const string AA_LogPublic_Name = "AA_LogPublic";  /// Tag for text panels (LCDs) to specify that panel should use public text to log;

        // Tags
        public static Tag AA_LogPrivate { get { return AA_LogPrivate_Name; } } /// Tag for text panels (LCDs) to specify that panel should use private text to log;
        public static Tag AA_LogPublic { get { return AA_LogPublic_Name; } }   /// Tag for text panels (LCDs) to specify that panel should use public text to log;

        private static List<Tag> tags;
        static Tag()
        {
            tags = new List<Tag>();
            tags.Add(new Tag(AA_LogPrivate_Name));
            tags.Add(new Tag(AA_LogPublic_Name));
        }
        public int Value { get; private set; }
        public string Name { get; private set; }

        private Tag(string name) : this(tags.Count, name) { }
        private Tag(int value, string name) { Value = value; Name = name; }

        public static implicit operator int(Tag type) { return type.Value; }
        public static implicit operator Tag(int value) { return tags.Find(e => e.Value == value); }
        public static implicit operator string(Tag type) { return type.Name; }
        public static implicit operator Tag(string name) { return tags.Find(e => e.Name == name); ; }
    }
}
