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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Security.Cryptography;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using Ximura.Server;
using Ximura.Command;
using AH = Ximura.Helper.AttributeHelper;
using RH = Ximura.Helper.Reflection;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Server
{
    public partial class AppServer<CONFSYS, CONFCOM, PERF>
    {
        #region ServerComponentsCreate()
        /// <summary>
        /// This method is used to create the server components.
        /// </summary>
        protected virtual void ServerComponentsCreate()
        {
            //Create the control containers for the server components.
            ControlContainerCreate();

            PoolManagerCreate();

            //This method adds the application specific services that identify the application to its components.
            ApplicationServicesStart();
        }
        #endregion // InitializeServerComponents()
        #region ServerComponentsDispose()
        /// <summary>
        /// This method is used to dispose the server components.
        /// </summary>
        protected virtual void ServerComponentsDispose()
        {
            ApplicationServicesStop();

            PoolManagerDispose();

            ControlContainerDispose();
        }
        #endregion // ServerComponentsDestroy()

        #region ControlContainerCreate()
        /// <summary>
        /// This method initializes the base components container 
        /// and the Application control container
        /// </summary>
        protected virtual void ControlContainerCreate()
        {
            //Set up the control container
            ControlServiceContainer = new XimuraServiceContainer();

            //Check that we only do this once
            ControlContainer = new XimuraAppContainer(ControlServiceContainer, this);
        }
        #endregion
        #region ControlContainerDispose()
        /// <summary>
        /// This method disposes of the control container.
        /// </summary>
        protected virtual void ControlContainerDispose()
        {
            if (ControlContainer != null)
            {
                ControlContainer.Dispose();
            }

            ControlContainer = null;
        }
        #endregion

        #region InitializeComponents()
        private void InitializeComponents()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion // InitializeComponents()
    }
}
