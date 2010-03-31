#region using
using System;
using System.Data;
using System.Xml;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the priority enumeration.
    /// </summary>
    public enum EmailPriority
    {
        //
        // Summary:
        //     The email has normal priority.
        Normal = 0,
        //
        // Summary:
        //     The email has low priority.
        Low = 1,
        //
        // Summary:
        //     The email has high priority.
        High = 2,
    }
}
