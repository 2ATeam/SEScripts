using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SE_Mods.CommandRunner
{

    //class ArgumentDictionary
    //{
    //    List<ArgumentType> types;
    //    List<Argument> args;
        
    //    public List<Argument> Values { get { return args; } }

    //    public void Add(ArgumentType type, Argument arg)
    //    {
    //        int index = -1;
    //        if ((index = types.IndexOf(type)) > -1)
    //        {
    //            types.RemoveAt(index);
    //            args.RemoveAt(index);
    //        }
    //        types.Add(type);
    //        args.Add(arg);
    //    }

    //    public bool Contains(ArgumentType type)
    //    {
    //        return types.Contains(type);
    //    }

    //    public Argument GetValueOrDefault(ArgumentType type)
    //    {
    //        int index = -1;
    //        if ((index = types.IndexOf(type)) > -1)
    //        {
    //            return args[index];
    //        }
    //        return null;
    //    }
    //}

    abstract class Command
    {
        /// <summary>
        /// Dictionary of all given arguments.
        /// </summary>
        protected readonly Dictionary<ArgumentType, Argument> arguments;

        /// <summary>
        /// Argument which is used to identify target blocks.
        /// </summary>
        protected Argument PrimaryArgumentKey { get; set; }

        /// <summary>
        /// Type of the command.
        /// </summary>
        public CommandType Type { get; protected set; }

        /// <summary>
        /// Collection of defined arguments.
        /// </summary>
        public ICollection<Argument> Arguments { get { return arguments.Values; } }

        protected Command(CommandType type, params Argument[] args)
        {
            Type = type;
            arguments = new Dictionary<ArgumentType, Argument>();
            for (int i = 0; i < args.Length; ++i)
            {
                Argument arg = args[i];
                if (IsAcceptableArgument(arg.Type))
                    arguments.Add(arg.Type, arg);
            }
            Validate();
        }

        /// <summary>
        /// Checks whether this command is setup and ready to run.
        /// </summary>
        protected virtual void Validate()
        {
            PrimaryArgumentKey = GetPrimaryArgument(ArgumentType.AA_Group, ArgumentType.AA_Name, ArgumentType.AA_Tag);
            if (PrimaryArgumentKey == null) throw new ArgumentException("Missed search key argument (name/tag/group)");
        }


        /// <summary>
        /// Gets first defined argument of given types.
        /// </summary>
        /// <param name="priorityArguments"> Argument type array which defines lookup priority.</param>
        /// <returns> Returns defined argument or null.</returns>
        protected Argument GetPrimaryArgument(params ArgumentType[] priorityArguments)
        {
            ArgumentType primary;
            for (int i = 0; i < priorityArguments.Length; i++)
            {
                primary = priorityArguments[i];
                Argument arg = arguments.GetValueOrDefault(primary);
                if (arg != null) return arg;
            }

            return null;
        }

        /// <summary>
        /// Implements actually command's logic.
        /// </summary>
        /// <param name="environment"> Script environment.</param>
        public abstract void Run(Sandbox.ModAPI.Ingame.MyGridProgram environment);

        /// <summary>
        /// Checks whether this argument is acceptable by current command.
        /// </summary>
        /// <param name="arg"> Argument type passed to this command.</param>
        /// <returns>Returns flag indicating whether this argument type acceptable or not.</returns>
        protected virtual bool IsAcceptableArgument(ArgumentType arg)
        {
            switch (arg)
            {
                case ArgumentType.AA_Group:
                case ArgumentType.AA_Name:
                case ArgumentType.AA_Tag:
                    return true;
                default: return false;
            }
        }

        public override string ToString()
        {
            List<Argument> args = new List<Argument>(Arguments);
            string output = Type.ToString() + " [";
            for (int i = 0; i < args.Count; ++i)
            {
                output += args[i].ToString() + "; ";
            }
            output = output.TrimEnd(new char[] { ' ', ';' }) + "]";
            return output;
        }
    }
}
