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
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using Ximura.Server;

using AH = Ximura.Helper.AttributeHelper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Command
{
    /// <summary>
    /// This helper class is used to pass the command definition without passing a reference to the command.
    /// </summary>
    public class CommandDefinition : IXimuraCommand
    {
        #region Declarations
        private Guid mCommandID;
        private string mCommandName;
        private string mCommandDescription;
        #endregion // Declarations

        #region Constructor
        /// <summary>
        /// This constructor copies the command properties in to the helper class.
        /// </summary>
        /// <param name="command">The command whose definition needs to be encapsulated.</param>
        public CommandDefinition(IXimuraCommand command)
        {
            mCommandID = command.CommandID;
            mCommandName = command.CommandName;
            mCommandDescription = command.CommandDescription;
        }
        #endregion // Constructor

        #region IXimuraCommand Members
        /// <summary>
        /// The command unique identifier.
        /// </summary>
        public Guid CommandID
        {
            get { return mCommandID; }
        }
        /// <summary>
        /// The command name. This is used in to the config file to retrieve the
        /// settings.
        /// </summary>
        public string CommandName
        {
            get
            {
                return mCommandName;
            }
            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }
        /// <summary>
        /// The command friendly description.
        /// </summary>
        public string CommandDescription
        {
            get
            {
                return mCommandDescription;
            }
            set
            {
                throw new NotImplementedException("The method or operation is not implemented.");
            }
        }
        #endregion
    }
}
