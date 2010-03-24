#region using
using System;
using System.Runtime.Serialization;
using System.IO;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This class is the base class for Site Manager components.
    /// </summary>
    public class SiteControllerComponentBase : AppCommandProcess<RQRSFolder, RQRSFolder, RQRSFolder, RQRSFolder,
        CommandConfiguration, CommandPerformance>
    {
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public SiteControllerComponentBase() : this(null) { }

        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public SiteControllerComponentBase(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        #endregion

        protected override void CommandBridgeRegister(bool register)
        {

        }
    }
}
