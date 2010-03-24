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
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This class holds the request data for the Content compiler command.
    /// </summary>
    public class ContentCompilerRequest : RQServer
    {
        #region Declarations
        private ControllerRequest mData;
        private MappingSettings mSettings;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constuctor.
        /// </summary>
        public ContentCompilerRequest()
            : base()
        {
        }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The base container.</param>
        public ContentCompilerRequest(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public ContentCompilerRequest(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This method resets the request so that it can be reused.
        /// </summary>
        public override void Reset()
        {
            mData = null;
            mSettings = null;
            base.Reset();
        }
        #endregion // Reset()

        #region Data
        /// <summary>
        /// This property contains the Controller Request.
        /// </summary>
        public ControllerRequest Data
        {
            get { return mData; }
            set { mData = value; }
        }
        #endregion // Request

        #region Settings
        /// <summary>
        /// This property contains the Controller Settings.
        /// </summary>
        public MappingSettings Settings
        {
            get { return mSettings; }
            set { mSettings = value; }
        }
        #endregion // Request

    }
}
