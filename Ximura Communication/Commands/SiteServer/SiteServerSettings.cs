#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Communication
{
    public class SiteServerSettings<ST> : ContextSettings<ST, SiteServerConfiguration, SiteServerPerformance>
        where ST : class,IXimuraFSMState
    {
        #region Constructors
        /// <summary>
        /// This constructor is called by the FSM when initiating the settings.
        /// </summary>
        public SiteServerSettings():base()
        {
        }
        #endregion
    }
}
