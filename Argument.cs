namespace SE_Mods.CommandRunner
{
    class Argument
    {
        public ArgumentType Type { get; private set; }
        public string Value { get; private set; }
        public Argument(ArgumentType type, string value) { Type = type; Value = value; }
        public override string ToString() { return Type.ToString() + " : " + Value; }
    }
}
