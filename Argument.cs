using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SE_Mods.CommandRunner
{
    class Argument
    {
        private const string ARG_TEMPLATE = "([^:;]+)\\s*:\\s*([^:;]+)";    // ARG_NAME : ARG_VALUE
        public ArgumentType Type { get; private set; }
        public string Value { get; private set; }


        public Argument(ArgumentType type, string value)
        {
            Type = type;
            Value = value;
        }

        public Argument(string strArg)
        {
            Match match = new Regex(Argument.ARG_TEMPLATE).Match(strArg);
            if (match.Success)
            {
                string name = match.Groups[1].Value;
                ArgumentType type;
                if (Enum.TryParse(name, out type))
                    Type = type;
                else
                    throw new ArgumentException("Unknown argument: " + name);
                Value = match.Groups[2].Value.Trim();
            }
        }

        public override string ToString()
        {
            return Type.ToString() + " : " + Value;
        }
    }
}
