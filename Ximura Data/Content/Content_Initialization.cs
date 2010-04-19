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
    public abstract partial class Content : ISupportInitializeNotification
    {
        #region Declarations
        private EventHandler mOnInitialized;

        /// <summary>
        /// This boolean value identifies whether the data content is currently initializing
        /// </summary>
        protected bool mInitializing = false;
        /// <summary>
        /// This boolean value identifies if the content is initialized
        /// </summary>
        protected bool mInitialized = false;
        #endregion // Declarations

        #region BeginInit()
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
        #endregion 
        #region EndInit()
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

        #region Initialized
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
        #endregion 

        #region IsInitialized
        /// <summary>
        /// This property returns true if the content has been initialized.
        /// </summary>
        public bool IsInitialized
        {
            get { return mInitialized; }
        }
        #endregion 

        #region OnInitialized()
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
    }
}
