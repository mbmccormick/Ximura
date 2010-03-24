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
using System.Collections;
using System.Net;
using System.Net.Sockets;

using Ximura;

using CH = Ximura.Common;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The IPEndPointExtended is used by the define
    /// to identify which IP addresses and ports to listen on, and adds an 
    /// additional parameter "SocketOptions" which can be used for the 
    /// listener to set specific options such as TLS.
    /// </summary>
    public class IPEndPointExtended : IPEndPoint
    {
        #region Static methods
        private static IPAddress ParseAddress(string location)
        {
            int index = location.IndexOf(':');
            if (index >= 0)
                location = location.Substring(0, index).ToLower();

            if (location == "{any}" || location=="0.0.0.0")
                return IPAddress.Any;

            if (location == "localhost" || location == "127.0.0.1")
                return IPAddress.Loopback;

            return IPAddress.Parse(location);
        }

        private static int ParsePort(string location)
        {
            string port = location.Substring(location.IndexOf(':') + 1);

            return int.Parse(port);

        }
        #endregion // Static methods

        #region Constructors
        public IPEndPointExtended(Uri Location):
            base(ParseAddress(Location.Authority),Location.Port)
        {

        }
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="location">The location of the IPEndPoint in the form of
        /// location:port</param>
        public IPEndPointExtended(string location)
            :
            base(ParseAddress(location), ParsePort(location)) { }

        /// <summary>
        /// Initializes a new instance of the TCPIPListenerIPEndPoint class with the specified 
        /// address and port number, which is an overriden class of the IPEndPoint class.
        /// </summary>
        /// <param name="address">The IP address of the Internet host.</param>
        /// <param name="port">The listening port required.</param>
        public IPEndPointExtended(IPAddress address, Int32 port)
            :
            base(address, port) { }

        /// <summary>
        /// Initializes a new instance of the TCPIPListenerIPEndPoint class with the specified 
        /// address and port number, which is an overriden class of the IPEndPoint class.
        /// </summary>
        /// <param name="address">The IP address of the Internet host.</param>
        /// <param name="port">The listening port required.</param>
        public IPEndPointExtended(Int64 address, Int32 port)
            :
            base(address, port) { }
        #endregion
    }
}
