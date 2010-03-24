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
using System.Diagnostics;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

using Ximura;

using Ximura.Framework;
using Ximura.Framework;
#endregion // 
namespace Ximura.Framework
{
	/// <summary>
	/// State is part of the Finite State Machine.
	/// </summary>
    [ToolboxBitmap(typeof(XimuraResourcePlaceholder), "Ximura.Resources.State.bmp")]
    public class State : XimuraComponentService, IXimuraFSMState
	{
		#region Declarations
		private string mIdentifier = null;
		#endregion // Declarations
		#region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public State():this(null){}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The container</param>
		public State(IContainer container)
		{
			if (container != null)
				container.Add(this);
		}
		#endregion // Constructors

		#region Identifier
		/// <summary>
		/// This is the state identifier string.
		/// </summary>
        [Category("State")]
        [DefaultValue(null)]
        [Description("This is the state identifier string.")]
        [RefreshProperties(RefreshProperties.All)]
        public virtual string Identifier
		{
			get
			{
				return mIdentifier;
			}
			set
			{
				mIdentifier = value;
			}
		}
		#endregion // Identifier

        #region ToString()
        /// <summary>
        /// This override is primarily used in debugging to make the state easy to identify.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return " State -> " + Identifier + " -> " + this.GetType().Name;
        }
        #endregion // ToString()

        #region Internal service control methods - Start, Stop etc.
        /// <summary>
        /// This method starts the AppServerProcess and registers any services
        /// </summary>
        protected override void InternalStart()
        {
            ServicesReference();
        }
        /// <summary>
        /// The method stops the AppServerProcess and unregisters and services
        /// </summary>
        protected override void InternalStop()
        {
            ServicesDereference();
        }
        #endregion

	}
}