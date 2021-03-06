﻿#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// Contributors:
//     Paul Stancer - initial implementation
// *******************************************************************************
#endregion
﻿#region using
using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using RH = Ximura.Reflection;

using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Communication
{
    [XimuraContentTypeID("1B9FE1F3-484A-47e6-B350-0CCD82C0A43A")]
    [XimuraDataContentSchema("http://schema.ximura.org/configuration/transport/1.0",
        "xmrres://Ximura/Ximura.Communication.TransportConfiguration/Ximura.Communication.TransportCommand.Configuration.TransportConfiguration.xsd")]
    public class TransportConfiguration : FSMCommandConfiguration
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public TransportConfiguration() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public TransportConfiguration(SerializationInfo info, StreamingContext context)
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
            string basePath = "//r:TransportConfiguration";
            mappingShortcuts.Add("r", basePath);
        }
        #endregion // XPScAdd(Dictionary<string, string> mappingShortcuts)

    }
}
