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
    /// This interface is implemented by all contexts in the FSM architecture.
    /// </summary>
    public interface IXimuraFSMContext : IXimuraPoolReturnable, IXimuraApplicationDefinition, IXimuraCommand
    {
        Guid? SignatureID { get;}

        void Reset(IXimuraFSMSettingsBase fsm, IXimuraSessionRQ contextSession, IXimuraFSMContextPoolAccess contextGet);

        void ChangeState();

        void ChangeState(string stateName);

        bool CheckState(string stateName);

        IXimuraSessionRQ ContextSession { get;}

        void ContextSessionInitialize(string username);

        void ContextSessionInitialize(string domain, string username);

        IXimuraPool GetObjectPool(Type objectType);

        void SenderIdentitySet(IXimuraRQRSEnvelope Env);

        bool ContextPoolAccessGranted { get;}

        IXimuraFSMContextPoolAccess ContextPoolAccess { get;}

        /// <summary>
        /// This property contains the session username.
        /// </summary>
        string UserName{get;}
        /// <summary>
        /// This property contains the session domain.
        /// </summary>
        string Domain { get;}

    }

}
