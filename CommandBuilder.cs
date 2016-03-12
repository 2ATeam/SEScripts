using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SE_Mods.CommandRunner
{
    static class CommandBuilder
    {
        private static string CMD_TEMPLATE = "(.*)\\s*\\[\\s*(?:([^;]+)\\s*;?\\s*)*\\]"; // CMD [ARG1; ARG2]

        /// <summary>
        /// Builds command from given string. If any error occurs.
        /// </summary>
        /// <exception cref="ArgumentException">Throws ArgumentException if any parsing error occurs. </exception>
        /// <param name="strCmd"> Incoming string argument.</param>
        /// <returns> Returns command object which can be run. </returns>
        public static Command BuildCommand(string strCmd)
        {
            Match match = new Regex(CMD_TEMPLATE).Match(strCmd);
            if (match.Success)
            {
                Command cmd;
                string name = match.Groups[1].Value;
                CommandType type;
                if (!Enum.TryParse(name, out type))
                    throw new ArgumentException("Unknown command: " + name);

                List<Argument> arguments = new List<Argument>();
                Group args = match.Groups[2];
                for (int i = 0; i < args.Captures.Count; ++i)
                {
                    arguments.Add(new Argument(args.Captures[i].Value));
                }
                return GetCommand(type, arguments.ToArray());

            }
            else
            {
                throw new ArgumentException("Invalid command string");
            }
        }

        private static Command GetCommand(CommandType type, params Argument[] args)
        {
            switch (type)
            {
                case CommandType.AA_Rotate: return new RotateCommand(args);
                default: return null;
            }
        }
    }
}
