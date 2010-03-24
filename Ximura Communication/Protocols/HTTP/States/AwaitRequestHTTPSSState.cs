#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net.Mail;
using System.Diagnostics;
using System.ComponentModel;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using AH = Ximura.AttributeHelper;
using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Communication
{
    public class AwaitRequestHTTPSSState : HTTPSSState
    {
        #region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public AwaitRequestHTTPSSState():this(null){}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The container</param>
        public AwaitRequestHTTPSSState(IContainer container)
            : base(container)
		{
		}
		#endregion // Constructors

        #region IN --> Receive
        /// <summary>
        /// This method receives requests from the HTTP agent and passes the request to the Site Controller.
        /// </summary>
        /// <param name="httpContext">The current context.</param>
        /// <param name="job">The job.</param>
        /// <param name="Data">The request data.</param>
        /// <returns>Returns true if the connection should be reset.</returns>
        public override bool Receive(SiteServerContext<HTTPSiteServerRQ, HTTPSiteServerRS> context,
            SecurityManagerJob job, RQRSContract<RQCallbackServer, RSCallbackServer> Data)
        {
            context.ChangeState("ProcessData");

            return context.Receive(job, Data);
        }
        #endregion // Receive

    }
}
