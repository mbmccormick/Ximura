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
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using Ximura.Server;

using Ximura.Command;
#endregion // using
namespace Ximura.Server
{
    public class StorageManagerConfiguration<T> : StorageManagerConfiguration
    where T : TimerPollJob, new()
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public StorageManagerConfiguration() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public StorageManagerConfiguration(SerializationInfo info, StreamingContext context)
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

    public class StorageManagerConfiguration: CommandConfiguration
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public StorageManagerConfiguration(){ }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public StorageManagerConfiguration(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion
    }
}
