#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class SiteControllerState : State
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public SiteControllerState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public SiteControllerState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region Initialize()
        /// <summary>
        /// This method initializes the request.
        /// </summary>
        /// <param name="context">The active context</param>
        public virtual void Initialize(SiteControllerContext context)
        {
            throw new NotImplementedException(this.Identifier + " -> Initialize is not implemented.");
        }
        #endregion // Initialize()
        #region MessageDecode()
        /// <summary>
        /// This method decodes the incoming message.
        /// </summary>
        /// <param name="context">The active context</param>
        public virtual bool MessageDecode(SiteControllerContext context)
        {
            throw new NotImplementedException(this.Identifier + " -> CookiePrepare is not implemented.");
        }
        #endregion
        #region SessionResolve()
        /// <summary>
        /// This method resolves the session.
        /// </summary>
        /// <param name="context">The active context</param>
        public virtual bool SessionResolve(SiteControllerContext context)
        {
            throw new NotImplementedException(this.Identifier + " -> SessionResolve is not implemented.");
        }
        #endregion
        #region RequestAuthenticate(SiteManagerContext context)
        /// <summary>
        /// This method validates the incoming request authentication.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <returns>Returns true if the state is successfully authenticated.</returns>
        public virtual bool RequestAuthenticate(SiteControllerContext context)
        {
            throw new NotImplementedException(this.Identifier + " -> Authenticate is not implemented.");
        }
        #endregion // Authenticate(SiteManagerContext context)

        #region RequestScriptAuthSet(SiteManagerContext context)
        /// <summary>
        /// This method should be used to set any auth settings in the script.
        /// </summary>
        /// <param name="context">The current context.</param>
        public virtual void RequestScriptAuthSet(SiteControllerContext context)
        {
            throw new NotImplementedException(this.Identifier + " -> RequestScriptAuthSet is not implemented.");
        }
        #endregion // RequestScriptAuthSet(SiteManagerContext context)

        #region Log()
        /// <summary>
        /// This method logs the current request.
        /// </summary>
        /// <param name="context">The active context</param>
        public virtual void Log(SiteControllerContext context)
        {
            throw new NotImplementedException(this.Identifier + " -> RequestValidate is not implemented.");
        }
        #endregion

        #region RequestResolve()
        /// <summary>
        /// This method resolves the request and sets the correct state prior to processing.
        /// </summary>
        /// <param name="context">The active context</param>
        public virtual bool RequestResolve(SiteControllerContext context)
        {
            throw new NotImplementedException(this.Identifier + " -> RequestResolve is not implemented.");
        }
        #endregion
        #region RequestProcess()
        /// <summary>
        /// This method processes the actual request.
        /// </summary>
        /// <param name="context">The active context</param>
        public virtual bool RequestProcess(SiteControllerContext context)
        {
            throw new NotImplementedException(this.Identifier + " -> RequestProcess is not implemented.");
        }
        #endregion

        #region ResponsePrepare()
        /// <summary>
        /// This method prepares the output for returning to the protocol.
        /// </summary>
        /// <param name="context">The active context</param>
        public virtual void ResponsePrepare(SiteControllerContext context)
        {
            throw new NotImplementedException(this.Identifier + " -> OutputPrepare is not implemented.");
        }
        #endregion
        #region ResponseComplete()
        /// <summary>
        /// This method prepares the output for returning to the protocol.
        /// </summary>
        /// <param name="context">The active context</param>
        public virtual void ResponseComplete(SiteControllerContext context)
        {
            throw new NotImplementedException(this.Identifier + " -> ResponseComplete is not implemented.");
        }
        #endregion
    }
}
