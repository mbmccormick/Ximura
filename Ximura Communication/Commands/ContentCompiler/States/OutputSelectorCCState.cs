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
    /// This state selects the appropraite output method state.
    /// </summary>
    public class OutputSelectorCCState : ContentCompilerState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public OutputSelectorCCState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public OutputSelectorCCState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region OutputSelect(ContentCompilerContext context)
        /// <summary>
        /// This method selects the appropriate output style.
        /// </summary>
        /// <param name="context">The current context.</param>
        public override void OutputSelect(ContentCompilerContext context)
        {
            //string id = context.Request.Data.ResponseOutputType.ToLower();
            string id = context.Request.Settings.OutputColl[0].OutputType;
            switch (id)
            {
                case "xpath":
                    context.ChangeState("OutputXPath");
                    break;
                case "xslt":
                    context.ChangeState("OutputTransform");
                    break;
                case "mime":
                    context.ChangeState("OutputMime");
                    break;
                default:
                    throw new InvalidOutputCCException(@"Output format """ + id + @""" not recognised.");
            }
        }
        #endregion // OutputSelect(ContentCompilerContext context)

    }
}
