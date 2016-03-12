using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE_Mods.CommandRunner
{
    /// <summary>
    /// Defines commands' arguments.
    /// </summary>
    enum ArgumentType
    {
        AA_Name,        // Name
        AA_Tag,         // Tag
        AA_Group,       // Group name
        AA_ByAngle,     // Rotate arg. Rotate by angle
        AA_ToAngle      // Rotate arg. Rotate to angle
    }
}
