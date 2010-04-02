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
using Ximura.Communication;
using CH = Ximura.Common;
#endregion // using
namespace Ximura.Data
{
    public abstract partial class Content : IXimuraMessageLoadData, IXimuraMessageLoad, ISupportInitializeNotification
    {
        #region Declarations
        private EventHandler mOnInitialized;
        /// <summary>
        /// This boolean property identifies whether the content can load.
        /// </summary>
        protected bool mCanLoad;
        /// <summary>
        /// This boolean value identifies whether the data content is currently initializing
        /// </summary>
        protected bool mInitializing = false;
        /// <summary>
        /// This boolean value identifies if the content is initialized
        /// </summary>
        protected bool mInitialized = false;
        #endregion // Declarations

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
        public virtual bool CanLoad
        {
            get { return mCanLoad; }
        }
        #endregion // CanLoad
        #region Loaded
        public virtual bool Loaded
        {
            get { return !mCanLoad; }
        }
        #endregion // Loaded

        #region ISupportInitialize Members
        /// <summary>
        /// This method is used by the component model to inform the DataContent that 
        /// the initialization process has begun
        /// </summary>
        public virtual void BeginInit()
        {
            if (mInitializing || mInitialized) return;

            mInitializing = true;
            mInitialized = false;
        }
        /// <summary>
        /// This method is used to inform the DataContent that the initialization is complete
        /// </summary>
        public virtual void EndInit()
        {
            if (mInitialized) return;

            mInitializing = false;
            mInitialized = true;
            OnInitialized();
        }
        #endregion

        #region ISupportInitializeNotification Members
        /// <summary>
        /// This event is used to signal the completion of initialization.
        /// </summary>
        public event EventHandler Initialized
        {
            add
            {
                this.mOnInitialized = (EventHandler)Delegate.Combine(this.mOnInitialized, value);
            }
            remove
            {
                this.mOnInitialized = (EventHandler)Delegate.Remove(this.mOnInitialized, value);
            }
        }

        /// <summary>
        /// This property returns true if the content has been initialized.
        /// </summary>
        public bool IsInitialized
        {
            get { return mInitialized; }
        }

        /// <summary>
        /// This protected method can be overriden to add additional code on the completion
        /// of initialization.
        /// </summary>
        protected virtual void OnInitialized()
        {
            if (this.mOnInitialized != null)
            {
                this.mOnInitialized(this, EventArgs.Empty);
            }
        }

        #endregion

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


        #region Unused IXimuraMessageLoadData Members

        public virtual int Load(IXimuraMessageTermination terminator, byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public virtual int Load(IXimuraMessageTermination terminator, Stream data)
        {
            throw new NotImplementedException();
        }

        public virtual int Load(IXimuraMessageTermination terminator, string data)
        {
            throw new NotImplementedException();
        }

        public virtual int Load(IXimuraMessageTermination terminator, string data, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
