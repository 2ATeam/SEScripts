namespace SE_Mods.CommandRunner
{
    static class ArgumentBuilder
    {                                                                       //  G1          G2
        private const string ARG_TEMPLATE = "([^:;]+)\\s*:\\s*([^:;]+)";    // ARG_NAME : ARG_VALUE

        public static Argument BuildArgument(Sandbox.ModAPI.Ingame.MyGridProgram environment, string strArg)
        {
            System.Text.RegularExpressions.Match match = new System.Text.RegularExpressions.Regex(ARG_TEMPLATE).Match(strArg);
            if (match.Success)
            {
                string name = match.Groups[1].Value.Trim();
                ArgumentType type = name;
                if (type == null) { environment.Echo(string.Format("Unknown argument: {0}", name)); return null; }
                string value = match.Groups[2].Value.Trim();
                return new Argument(type, value);
            }
            return null;
        }
    }
}
