#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;

using Ximura;
using Ximura.Data;
using Ximura.Persistence;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class OutputFileRMState : OutputRMState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public OutputFileRMState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public OutputFileRMState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

    }
}
