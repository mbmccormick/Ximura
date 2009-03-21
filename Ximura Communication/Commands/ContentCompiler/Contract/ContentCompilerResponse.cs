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
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The content compiler response contains the message body response from the content compiler command.
    /// </summary>
    public class ContentCompilerResponse : RSServer
    {
        #region Declarations
        private InternetMessageFragmentBody mBody;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constuctor.
        /// </summary>
        public ContentCompilerResponse()
            : base()
        {
        }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The base container.</param>
        public ContentCompilerResponse(System.ComponentModel.IContainer container)
            : base(container)
        {
        }
        /// <summary>
        /// This is the deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public ContentCompilerResponse(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This method resets the class to its original value so that it can be reused.
        /// </summary>
        public override void Reset()
        {
            mBody = null;
            base.Reset();
        }
        #endregion // Reset()

        #region Body
        /// <summary>
        /// This is the compiled body that can be inserted in to an internet based message.
        /// </summary>
        public InternetMessageFragmentBody Body
        {
            get { return mBody; }
            set { mBody = value; }
        }
        #endregion // Body
    }
}
