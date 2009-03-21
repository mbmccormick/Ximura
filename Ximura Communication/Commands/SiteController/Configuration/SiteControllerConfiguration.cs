#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    [XimuraContentTypeID("2C249739-D346-4eec-BF2B-E43B4A9BCB3C")]
    [XimuraDataContentSchema("http://schema.ximura.org/configuration/sitecontroller/1.0",
       "xmrres://XimuraComm/Ximura.Communication.SiteControllerConfiguration/Ximura.Communication.Commands.SiteController.Configuration.SiteControllerConfiguration.xsd")]
    public class SiteControllerConfiguration : FSMCommandConfiguration
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor for the Content object.
        /// </summary>
        public SiteControllerConfiguration()
        {
        }

        #endregion

        #region XPScAdd(Dictionary<string, string> mappingShortcuts)
        /// <summary>
        /// This method adds the XPath shortcuts in to the collection. You should
        /// override this method to add your own shorcuts.
        /// </summary>
        /// <param name="mappingShortcuts">The mapping shorcut collection.</param>
        protected override void XPScAdd(Dictionary<string, string> mappingShortcuts)
        {
            string basePath = "//r:SiteControllerConfiguration";
            mappingShortcuts.Add("r", basePath);
        }
        #endregion // XPScAdd(Dictionary<string, string> mappingShortcuts)
    }
}
