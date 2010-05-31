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
#region using
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using RH = Ximura.Reflection;
using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This class encapsulates the Content Data Store for use in standalone projects that 
    /// does not use the Ximura Framework.
    /// </summary>
    public class ContentDataStoreContainer<CDS> : FSMCommandContainer<CDS, ICDSState>
        where CDS: ContentDataStore, new()
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ContentDataStoreContainer():base(null)
        {
        }
        /// <summary>
        /// This is the state extension constructor. You can add custom persistence managers manually through the constructor.
        /// </summary>
        /// <param name="states">A collection of persistence states to add to the baser finite state machine collection.</param>
        public ContentDataStoreContainer(IEnumerable<KeyValuePair<string, ICDSState>> states):base(states)
        {
        }
        #endregion 

        #region Dispose(bool disposing)
        /// <summary>
        /// This override stops the service before it is disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
            base.Dispose(disposing);
        }
        #endregion 
    }

    #region ContentDataStoreContainer
    /// <summary>
    /// This class encapsulates the Content Data Store for use in standalone projects that 
    /// does not use the Ximura Framework.
    /// </summary>
    public class ContentDataStoreContainer : ContentDataStoreContainer<ContentDataStore>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ContentDataStoreContainer()
            : base(null)
        {
        }
        /// <summary>
        /// This is the state extension constructor. You can add custom persistence managers manually through the constructor.
        /// </summary>
        /// <param name="states">A collection of persistence states to add to the baser finite state machine collection.</param>
        public ContentDataStoreContainer(IEnumerable<KeyValuePair<string, ICDSState>> states)
            : base(states)
        {
        }
        #endregion

        #region Dispose(bool disposing)
        /// <summary>
        /// This override stops the service before it is disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
    #endregion 

}
