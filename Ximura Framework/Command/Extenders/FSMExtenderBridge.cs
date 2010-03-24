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

using Ximura;

#endregion // using
namespace Ximura.Framework
{
	/// <summary>
	/// FSMExtenderBridge is used to link the various components within the finite state machine.
	/// </summary>
    public class FSMExtenderBridge<ST> : IXimuraFSMExtenderBridge<ST>
        where ST : class,IXimuraFSMState
    {
		#region Declaration
		private IXimuraFSMStateMetadataExtender<ST> mStateEx;
		#endregion // Declaration
		#region Constructor
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public FSMExtenderBridge()
		{
			mStateEx=null;
		}
		/// <summary>
		/// This is the default constructor.
		/// </summary>
        public FSMExtenderBridge(IXimuraFSMStateMetadataExtender<ST> StateEx)
		{
			mStateEx=StateEx;
		}
		#endregion // Constructor

		#region StateEx
		/// <summary>
		/// This is the state extender.
		/// </summary>
        public IXimuraFSMStateMetadataExtender<ST> StateEx
		{
			get
			{
				return mStateEx;
			}
			set
			{
				mStateEx=value;
			}
		}
		#endregion // StateEx
	}
}
