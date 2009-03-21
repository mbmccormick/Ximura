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
    public class ResolverState : SiteControllerState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ResolverState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ResolverState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region IN --> RequestScriptAuthSet(SiteManagerContext context)
        /// <summary>
        /// This method should be used to set any auth settings in the script.
        /// </summary>
        /// <param name="context">The current context.</param>
        public override void RequestScriptAuthSet(SiteControllerContext context)
        {
            if (context.ScriptSession == null)
                return;

            foreach (RealmAuthentication auth in context.ScriptSession.Authentication)
            {
                context.ScriptRequest.AuthenticationSet(auth);
            }
        }
        #endregion // RequestScriptAuthSet(SiteManagerContext context)

        #region IN --> RequestResolve(SiteControllerContext context)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool RequestResolve(SiteControllerContext context)
        {
            string changeState = context.ScriptSettings.MappingState;

            context.CheckChangeState("RS_" + changeState);

            return true;
        }
        #endregion // RequestResolve(SiteControllerContext context)
    }
}
