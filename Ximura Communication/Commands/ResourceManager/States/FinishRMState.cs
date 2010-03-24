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
    public class FinishRMState : ResourceManagerState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public FinishRMState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public FinishRMState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        public override void Finish(ResourceManagerContext context)
        {

        }
    }
}
