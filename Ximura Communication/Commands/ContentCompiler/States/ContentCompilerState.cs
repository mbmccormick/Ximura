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
    public class ContentCompilerState : State
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ContentCompilerState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ContentCompilerState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        public virtual void Initialize(ContentCompilerContext context)
        {
            throw new NotImplementedException("ContentCompilerState->Initialize is not implemented: " + this.ToString());
        }

        public virtual void RequestValidate(ContentCompilerContext context)
        {
            throw new NotImplementedException("ContentCompilerState->RequestValidate is not implemented: " + this.ToString());
        }

        public virtual void ModelTemplateLoad(ContentCompilerContext context)
        {
            throw new NotImplementedException("ContentCompilerState->ModelTemplateLoad is not implemented: " + this.ToString());
        }

        public virtual void ModelTemplateCompile(ContentCompilerContext context)
        {
            throw new NotImplementedException("ContentCompilerState->ModelTemplateCompile is not implemented: " + this.ToString());
        }

        public virtual void ModelDataLoadInserts(ContentCompilerContext context)
        {
            throw new NotImplementedException("ContentCompilerState->ModelDataLoadInserts is not implemented: " + this.ToString());
        }

        public virtual void OutputSelect(ContentCompilerContext context)
        {
            throw new NotImplementedException("ContentCompilerState->OutputSelect is not implemented: " + this.ToString());
        }

        public virtual void OutputPrepare(ContentCompilerContext context)
        {
            throw new NotImplementedException("ContentCompilerState->OutputPrepare is not implemented: " + this.ToString());
        }

        public virtual void OutputComplete(ContentCompilerContext context)
        {
            throw new NotImplementedException("ContentCompilerState->OutputPrepare is not implemented: " + this.ToString());
        }

        public virtual void Finish(ContentCompilerContext context)
        {
            throw new NotImplementedException("ContentCompilerState->Finalize is not implemented " + this.ToString());
        }
    }
}
