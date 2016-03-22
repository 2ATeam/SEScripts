using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using SE_Mods.CommandRunner.Commands;

namespace SE_Mods.CommandRunner
{
    static class CommandBuilder
    {                                       // G1                G2                             G3
        private static string CMD_TEMPLATE = "(.*)\\s*\\[\\s*(?:([^;]+)\\s*;?\\s*)*\\]\\s*(?://(.*))?"; // CMD [ARG1; ARG2] // comment

        /// <summary>
        /// Builds command from given string. If any error occurs.
        /// </summary>
        /// <param name="strCmd"> Incoming string argument.</param>
        /// <returns> Returns command object which can be run. </returns>
        public static Command BuildCommand(MyGridProgram environment, string strCmd)
        {
            System.Text.RegularExpressions.Match match = new System.Text.RegularExpressions.Regex(CMD_TEMPLATE).Match(strCmd);
            if (match.Success)
            {
                
                string name = match.Groups[1].Value.Trim();
                CommandType type = name;
                if (type == null) { environment.Echo(string.Format("Unknown command: {0}", name)); return null; }

                List<Argument> arguments = new List<Argument>();
                System.Text.RegularExpressions.Group args = match.Groups[2];
                for (int i = 0; i < args.Captures.Count; ++i)
                {
                    Argument arg = ArgumentBuilder.BuildArgument(environment, args.Captures[i].Value.Trim());
                    if (arg == null) continue;
                    arguments.Add(arg);
                }

                Command cmd = GetCommand(environment, type, arguments.ToArray());
                if (match.Groups[3].Captures.Count > 0) cmd.Comment = match.Groups[3].Value.Trim();
                return cmd;
            }
            else
                return null;
        }

        private static Command GetCommand(MyGridProgram environment, CommandType type, params Argument[] args)
        {
            if (type == CommandType.Rotate) return new RotateCommand(environment, args);
            if (type == CommandType.Console) return new ConsoleCommand(environment, args);
            if (type == CommandType.SolarOptimize) return new SolarOptimizeCommand(environment, args);
            return null;
        }
    }
}
