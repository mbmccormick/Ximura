#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;

using Ximura;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This state selects the appropraite output method state.
    /// </summary>
    public class StartRMState : ResourceManagerState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public StartRMState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public StartRMState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region Initialize(ResourceManagerContext context)
        /// <summary>
        /// This is the initialize state. This method does nothing.
        /// </summary>
        /// <param name="context">The current context.</param>
        public override void Initialize(ResourceManagerContext context)
        {
            string subCommand = "";

            if (context.Data.DestinationAddress.SubCommand is string)
                subCommand =(string)context.Data.DestinationAddress.SubCommand;

            switch (subCommand)
            {
                case "ETagFlush":
                    context.CheckChangeState("ETagCacheFlush");
                    break;
                default:
                    context.CheckChangeState("OutputSelector");
                    break;
            }
        }
        #endregion // Initialize(ResourceManagerContext context)
    }
}
