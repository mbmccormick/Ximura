#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using CDS = Ximura.Persistence.CDSHelper;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class SiteControllerPerformance : FSMCommandPerformance
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor for the Content object.
        /// </summary>
        public SiteControllerPerformance()
        {
        }

        #endregion

    }
}
