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
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Text;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
#endregion // using
namespace Ximura.Data
{
    public abstract partial class Content : IXimuraMessageLoadData, IXimuraMessageLoad
    {
        #region Declarations
        /// <summary>
        /// This boolean property identifies whether the content can load.
        /// </summary>
        protected bool mCanLoad;
        #endregion 

        public abstract int Load(byte[] buffer, int offset, int count);

        #region Load(Stream data)
        public virtual int Load(Stream data)
        {
            byte[] blob = new byte[data.Length-data.Position];
            data.Read(blob, 0, blob.Length);
            return Load(blob, 0, blob.Length);
        }
        #endregion // Load(Stream data)

        #region Load(string data)
        public virtual int Load(string data)
        {
            return Load(data, Encoding.UTF8);
        }
        #endregion // Load(string data)
        #region Load(string data, System.Text.Encoding encoding)
        public virtual int Load(string data, System.Text.Encoding encoding)
        {
            byte[] blob = encoding.GetBytes(data);

            return Load(blob, 0, blob.Length);
        }
        #endregion // Load(string data, System.Text.Encoding encoding)

        #region CanLoad
        /// <summary>
        /// This boolean property that specifies whether the message can be loaded.
        /// </summary>
        public virtual bool CanLoad
        {
            get { return mCanLoad; }
        }
        #endregion // CanLoad
        #region Loaded
        /// <summary>
        /// This boolean property specifies whether the entity has been loaded.
        /// </summary>
        public virtual bool Loaded
        {
            get { return !mCanLoad; }
        }
        #endregion // Loaded

        #region LoadCheck()
        /// <summary>
        /// This helper method will throw an exception if the content has not been loaded.
        /// </summary>
        /// <exception cref="Ximura.Data.ContentLoadException">This exception is thrown if the content has not been loaded.</exception>
        protected virtual void LoadCheck()
        {
            if (!Loaded)
                throw new ContentLoadException("The content has not been loaded.");
        }
        #endregion // LoadCheck()
    }
}
