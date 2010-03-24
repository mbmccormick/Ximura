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
    public class HTTPSSState : SiteServerState<HTTPSiteServerRQ, HTTPSiteServerRS>
    {
        #region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public HTTPSSState():this(null){}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The container</param>
        public HTTPSSState(IContainer container)
            : base(container)
		{
		}
		#endregion // Constructors

        #region ExtractUri(InternetMessageRequest httpRQ, Uri URILocal)
        /// <summary>
        /// This method extracts the request uri from the message.
        /// </summary>
        /// <param name="httpRQ">The internet message.</param>
        /// <param name="URILocal">The local uri.</param>
        /// <returns>Returns a formatted uri containging the request.</returns>
        protected Uri ExtractUri(HTTPRequestMessage httpRQ, Uri URILocal)
        {
            string hostOnly;
            if (httpRQ.Host.Contains(":"))
            {
                hostOnly = httpRQ.Host;
            }
            else
                hostOnly = httpRQ.Host + ":" + URILocal.Port;

            return new Uri(@"http://" + hostOnly + httpRQ.Instruction.Instruction);
        }
        #endregion // ExtractUri(InternetMessageRequest httpRQ, Uri URILocal)
        #region ExtractUserAgent(HTTPRequestMessage httpRQ)
        /// <summary>
        /// This method extracts the request uri from the message.
        /// </summary>
        /// <param name="httpRQ">The internet message.</param>
        /// <returns>Returns a formatted uri containging the request.</returns>
        protected string ExtractUserAgent(HTTPRequestMessage httpRQ)
        {
            if (!httpRQ.HeaderExists("user-agent"))
                return null;

            return httpRQ.HeaderSingle("user-agent");
        }
        #endregion // ExtractUri(InternetMessageRequest httpRQ, Uri URILocal)
    }
}
