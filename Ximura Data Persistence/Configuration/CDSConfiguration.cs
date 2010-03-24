#region Copyright
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
namespace Ximura.Data
{
    [XimuraContentTypeID("2F6A8D71-86AA-495f-A4C2-3FECD6B4F097")]
    [XimuraDataContentSchema("http://schema.ximura.org/configuration/cds/1.0",
        "xmrres://Ximura/Ximura.Data.CDSConfiguration/Ximura.Data.CDS.Configuration.CDSConfiguration.xsd")]
    [XimuraContentCachePolicy(ContentCacheOptions.CannotCache)]
    public class CDSConfiguration: FSMCommandConfiguration<CDSTimerPollJob>
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public CDSConfiguration() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public CDSConfiguration(SerializationInfo info, StreamingContext context)
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
            string basePath = "//r:CDSConfiguration";
            mappingShortcuts.Add("r", basePath);
        }
        #endregion // XPScAdd(Dictionary<string, string> mappingShortcuts)
        #region NamespaceDefaultShortName
        /// <summary>
        /// This is the short name used in the namespace manager to refer to the root namespace.
        /// </summary>
        protected override string NamespaceDefaultShortName
        {
            get
            {
                return "r";
            }
        }
        #endregion // NamespaceDefaultShortName

        //protected override bool LoadConfigInitialize(IXimuraApplicationDefinition appDef, IXimuraCommand commDef, IXimuraConfigSH sh)
        //{
        //    //return base.LoadConfigInitialize(appDef, commDef, sh);
        //    return true;
        //}
    }

    #region CDSTimerPollJob
    /// <summary>
    /// This class is used to initiate timer poll jobs for the Content Data Store.
    /// </summary>
    public class CDSTimerPollJob: TimerPollJob
    {
        /// <summary>
        /// This override sets the ContentDataStoreSubCommand enumeration for the job.
        /// </summary>
        /// <param name="element">The xml element containing the time poll data.</param>
        /// <param name="NSM">The namespace manager.</param>
        /// <param name="subCommand">The subcommand object.</param>
        public override void Configure(XmlElement element, XmlNamespaceManager NSM, object subCommand)
        {
            base.Configure(element, NSM, subCommand);

            if (IDType != "ContentDataStoreSubCommand")
                return;

            switch (ID)
            {
                case "CacheManagersPoll":
                    Subcommand = ContentDataStoreSubCommand.CacheManagersPoll;
                    break;
            }
        }
    }
    #endregion // CDSTimerPollJob
}
