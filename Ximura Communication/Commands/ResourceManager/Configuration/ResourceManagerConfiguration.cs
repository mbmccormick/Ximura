#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using CDS = Ximura.Persistence.CDSHelper;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    [XimuraContentTypeID("78E0B5B3-B59C-42b3-A302-2098B66CDAA6")]
    [XimuraDataContentSchema("http://schema.ximura.org/configuration/resourcemanager/1.0",
       "xmrres://XimuraComm/Ximura.Communication.ResourceManagerConfiguration/Ximura.Communication.Commands.ResourceManager.Configuration.ResourceManagerConfiguration.xsd")]
    public class ResourceManagerConfiguration : FSMCommandConfiguration
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public ResourceManagerConfiguration() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public ResourceManagerConfiguration(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        #region XPScAdd(Dictionary<string, string> mappingShortcuts)
        /// <summary>
        /// This method adds the XPath shortcuts in to the collection. You should
        /// override this method to add your own shorcuts.
        /// </summary>
        /// <param name="mappingShortcuts">The mapping shorcut collection.</param>
        protected override void XPScAdd(Dictionary<string, string> mappingShortcuts)
        {
            string basePath = "//r:ResourceManagerConfiguration";
            mappingShortcuts.Add("r", basePath);
        }
        #endregion // XPScAdd(Dictionary<string, string> mappingShortcuts)
    }
}
