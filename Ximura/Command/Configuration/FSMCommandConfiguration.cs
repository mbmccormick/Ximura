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
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;

using Ximura.Server;
using Ximura.Command;


#endregion // using
namespace Ximura.Command
{
    public class FSMCommandConfiguration<T> : FSMCommandConfiguration
        where T : TimerPollJob, new()
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public FSMCommandConfiguration() : this((IContainer)null) { }
        /// <summary>
        /// This constructor is called by .NET when it added as new to a container.
        /// </summary>
        /// <param name="container">The container this component should be added to.</param>
        public FSMCommandConfiguration(System.ComponentModel.IContainer container)
            :
            base(container) { }
        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public FSMCommandConfiguration(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        #region TimerPollJobCreate(List<TimerPollJob> jobs, XmlNode node)
        /// <summary>
        /// This method creates a specific poll job and adds it to the poll job collection.
        /// </summary>
        /// <param name="jobs">The poll job collection.</param>
        /// <param name="node">The configuration node for the poll job.</param>
        protected override void TimerPollJobCreate(List<TimerPollJob> jobs, XmlElement node)
        {
            T newJob = new T();
            newJob.Configure(node, NSM, null);
            jobs.Add(newJob);
        }
        #endregion // TimerPollJobCreate(List<TimerPollJob> jobs, XmlNode node)
    }

    [XimuraContentTypeID("28C52546-68E7-4847-AD03-C7BEA2E17C8D")]
    [XimuraDataContentSchemaReference("http://schema.ximura.org/configuration/fsm/1.0",
        "xmrres://Ximura/Ximura.Command.FSMCommandConfiguration/Ximura.Command.Configuration.FSMCommandConfiguration.xsd")]
    public class FSMCommandConfiguration : CommandConfiguration
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public FSMCommandConfiguration() : this((IContainer)null) { }
        /// <summary>
        /// This constructor is called by .NET when it added as new to a container.
        /// </summary>
        /// <param name="container">The container this component should be added to.</param>
        public FSMCommandConfiguration(System.ComponentModel.IContainer container)
            :
            base(container) { }
        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public FSMCommandConfiguration(SerializationInfo info, StreamingContext context)
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
            string basePath = "//r:FSMConfiguration";
            mappingShortcuts.Add("r", basePath);
        }
        #endregion // XPScAdd(Dictionary<string, string> mappingShortcuts)

        #region NamespaceManagerAdd(XmlNamespaceManager nsm)
        /// <summary>
        /// This override adds the ximura namespace to the default Namespace manager.
        /// </summary>
        /// <param name="nsm">The system default namespace manager.</param>
        protected override void NamespaceManagerAdd(XmlNamespaceManager nsm)
        {
            base.NamespaceManagerAdd(nsm);
            nsm.AddNamespace("fsmconf", "http://schema.ximura.org/configuration/fsm/1.0");
        }
        #endregion // NamespaceManagerAdd(XmlNamespaceManager nsm)

        #region PoolMin
        /// <summary>
        /// The minimum number of contexts to be placed in the pool.
        /// </summary>
        public virtual int PoolMin
        {
            get { return XmlMappingGetToInt32(XPScA("r", "min", "fsmconf:pool")); }
            protected set { XmlMappingSet(XPScA("r", "min", "fsmconf:pool"), value); }
        }
        #endregion // PoolMin
        #region PoolMax
        /// <summary>
        /// The maximum number of context that will be allowed in the pool. 
        /// </summary>
        public virtual int PoolMax
        {
            get { return XmlMappingGetToInt32(XPScA("r", "max", "fsmconf:pool")); }
            protected set { XmlMappingSet(XPScA("r", "max", "fsmconf:pool"), value); }
        }
        #endregion // PoolMax
        #region PoolPrefer
        /// <summary>
        /// The preferred number of context for the pool.
        /// </summary>
        public virtual int PoolPrefer
        {
            get { return XmlMappingGetToInt32(XPScA("r", "prefer", "fsmconf:pool")); }
            protected set { XmlMappingSet(XPScA("r", "prefer", "fsmconf:pool"), value); }
        }
        #endregion // PoolPrefer

        //protected override bool LoadConfigInitialize(IXimuraApplicationDefinition appDef, IXimuraCommand commDef, IXimuraConfigSH sh)
        //{
        //    bool result = base.LoadConfigInitialize(appDef, commDef, sh);

        //    IXimuraFSMConfigSH appConfig = sh as IXimuraFSMConfigSH;
        //    if (appConfig == null)
        //        return result;

        //    if (!result)
        //        return false;

        //    PoolMin = appConfig.PoolMin;
        //    PoolMax = appConfig.PoolMax;
        //    PoolPrefer = appConfig.PoolPrefer;

        //    return result;
        //}
    }
}
