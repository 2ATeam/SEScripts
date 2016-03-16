using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;

namespace SE_Mods.CommandRunner
{
    static class CommandBuilder
    {                                                                                    // G1      G2
        private static string CMD_TEMPLATE = "(.*)\\s*\\[\\s*(?:([^;]+)\\s*;?\\s*)*\\]"; // CMD [ARG1; ARG2]

        /// <summary>
        /// Builds command from given string. If any error occurs.
        /// </summary>
        /// <exception cref="ArgumentException">Throws ArgumentException if any parsing error occurs. </exception>
        /// <param name="strCmd"> Incoming string argument.</param>
        /// <returns> Returns command object which can be run. </returns>
        public static Command BuildCommand(MyGridProgram environment, string strCmd)
        {
            System.Text.RegularExpressions.Match match = new System.Text.RegularExpressions.Regex(CMD_TEMPLATE).Match(strCmd);
            if (match.Success)
            {
                Command cmd;
                string name = match.Groups[1].Value.Trim();
                CommandType type = name;
                if (type == null) environment.Echo(string.Format("Unknown command: {0}", name));

                List<Argument> arguments = new List<Argument>();
                System.Text.RegularExpressions.Group args = match.Groups[2];
                for (int i = 0; i < args.Captures.Count; ++i)
                    arguments.Add(ArgumentBuilder.BuildArgument(args.Captures[i].Value));
                return GetCommand(environment, type, arguments.ToArray());

            }
            else
            {
                throw new ArgumentException("Invalid command string");
            }
        }

        private static Command GetCommand(MyGridProgram environment, CommandType type, params Argument[] args)
        {
            if (type == CommandType.AA_Rotate) return new RotateCommand(environment, args);

            return null;
        }
    }
}
