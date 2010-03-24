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
    public class ResourceManagerState : State
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ResourceManagerState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ResourceManagerState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        public virtual void Initialize(ResourceManagerContext context)
        {
            throw new NotImplementedException("ContentCompilerState->Initialize is not implemented: " + this.ToString());
        }

        public virtual bool RequestValidate(ResourceManagerContext context)
        {
            throw new NotImplementedException("ContentCompilerState->RequestValidate is not implemented: " + this.ToString());
        }

        public virtual void OutputSelect(ResourceManagerContext context)
        {
            throw new NotImplementedException("ContentCompilerState->OutputSelect is not implemented: " + this.ToString());
        }

        public virtual void OutputPrepare(ResourceManagerContext context)
        {
            throw new NotImplementedException("ContentCompilerState->OutputPrepare is not implemented: " + this.ToString());
        }

        public virtual void OutputComplete(ResourceManagerContext context)
        {
            throw new NotImplementedException("ContentCompilerState->OutputPrepare is not implemented: " + this.ToString());
        }

        public virtual void Finish(ResourceManagerContext context)
        {
            throw new NotImplementedException("ContentCompilerState->Finalize is not implemented " + this.ToString());
        }
    }
}