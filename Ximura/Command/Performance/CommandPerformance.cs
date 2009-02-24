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
using System.Threading;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;

using Ximura.Server;


#endregion // using
namespace Ximura.Command
{
    /// <summary>
    /// This method holds the standard performance indicators the a command.
    /// </summary>
    public class CommandPerformance : PerformanceBase, IXimuraCommandPerformance
    {
        #region Declarations
        private Guid mCommandID;
        private Guid mPCID;
        private string mName;
        private string mCategory;

        private long mRequestCallbacks;
        private long mRequestCallbacksCurrent;
        private long mRequest;
        private long mRequestCurrent;
        #endregion // Declarations
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Content object.
		/// </summary>
		public CommandPerformance()
        {
        }
		#endregion

        public virtual void RequestCallbackStart(Guid ID)
        {
            Interlocked.Increment(ref mRequestCallbacks);
            Interlocked.Increment(ref mRequestCallbacksCurrent);
        }

        public virtual void RequestCallbackEnd(Guid ID)
        {
            Interlocked.Decrement(ref mRequestCallbacksCurrent);
        }

        public virtual void RequestStart(Guid ID)
        {
            Interlocked.Increment(ref mRequest);
            Interlocked.Increment(ref mRequestCurrent);
        }

        public virtual void RequestEnd(Guid ID)
        {
            Interlocked.Decrement(ref mRequestCurrent);
        }

        #region IXimuraPerformanceCounterCollection Members

        /// <summary>
        /// The command id.
        /// </summary>
        public Guid CommandID
        {
            get { return mCommandID; }
            set { mCommandID = value; }
        }

        /// <summary>
        /// The parent command id.
        /// </summary>
        public Guid PCID
        {
            get { return mPCID; }
            set { mPCID = value; }
        }

        /// <summary>
        /// The counter name.
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// The category name.
        /// </summary>
        public string Category
        {
            get { return mCategory; }
            set { mCategory = value; }
        }

        #endregion

        #region ICollection<IXimuraPerformanceCounter> Members

        public void Add(IXimuraPerformanceCounter item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(IXimuraPerformanceCounter item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IXimuraPerformanceCounter[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(IXimuraPerformanceCounter item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<IXimuraPerformanceCounter> Members

        public IEnumerator<IXimuraPerformanceCounter> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
