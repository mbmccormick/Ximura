#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
using AH = Ximura.Helper.AttributeHelper;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This interface is used to provide a high-speed system service shortcut for IP address resolution.
    /// </summary>
    public interface ISiteControllerLogger
    {
        /// <summary>
        /// This method resolves the ip address and returns the iso country code.
        /// </summary>
        /// <param name="address">The address to resolve.</param>
        /// <param name="isoCountryCode">The out parameter containing the country code, or null if the address cannot be resolved.</param>
        /// <returns>Returns true if the address has been resolved successfully.</returns>
        bool ResolveAddress(IPAddress address, out string isoCountryCode);

        /// <summary>
        /// This method logs the request.
        /// </summary>
        /// <param name="info">The information to log.</param>
        void LogRequestEnqueue(SiteControllerRequestInfo info);
    }
}
