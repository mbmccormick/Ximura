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
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;
using Ximura;

using Ximura.Data;
using CH = Ximura.Common;
#endregion
namespace Ximura
{
    /// <summary>
    /// This message object encapsulates the CSV file format and is in accordance with RFC 4180.
    /// </summary>
    public class CSVMessage : Message
    {
        #region Declarations
        private bool mHeadersSupported;
        private bool mHeadersLoaded;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor which specifies that 
        /// the first line in the file contains the header information
        /// </summary>
        public CSVMessage():this(true)
        {

        }
        /// <summary>
        /// This constructor can be used to specify whether the first line contains header information.
        /// </summary>
        /// <param name="HeadersSupported">A boolean property that specifies whether the first line of the file contains header information.</param>
        public CSVMessage(bool HeadersSupported)
        {
            mHeadersSupported = HeadersSupported;
            if (!HeadersSupported)
                mHeadersLoaded = false;
        }
        #endregion  

        #region HeadersLoaded
        /// <summary>
        /// This property specifies whether the headers are loaded.
        /// </summary>
        public bool HeadersLoaded { get { return mHeadersLoaded; } }
        #endregion  
        #region HeadersSupported
        /// <summary>
        /// This property specifies whether headers are supported in the first line.
        /// </summary>
        public bool HeadersSupported { get { return mHeadersSupported; } }
        #endregion  

        #region Headers
        /// <summary>
        /// This enumeration returns the headers in the order that they are set in the CSV file.
        /// </summary>
        public IEnumerable<string> Headers
        {
            get
            {
                if (!HeadersLoaded 
                    || FragmentFirst == null
                    || (!(FragmentFirst is CSVHeaderFragment)))
                    yield break;

                foreach(string item in ((CSVHeaderFragment)FragmentFirst).Items)
                    yield return item;
            }
        }
        #endregion  

        #region Load(Stream data)
        /// <summary>
        /// This method reads from the stream and loads in the incoming data.
        /// </summary>
        /// <param name="data">The stream data to read from.</param>
        /// <returns>Returns the number of items read from the stream.</returns>
        public override int Load(Stream data)
        {
            Load();

            byte[] buffer = new byte[1024];
            int length = 0;
            int read,used;

            while (data.CanRead)
            {
                read = data.Read(buffer, 0, 1024);
                if (read == 0)
                    break;

                used = Write(buffer,0,read);

                length+=read;
            }

            return length;
        }
        #endregion

        #region FragmentHeaderInitialType
        /// <summary>
        /// This method returns the initial fragment for the CSV file.
        /// </summary>
        protected override Type FragmentHeaderInitialType
        {
            get
            {
                return typeof(CSVHeaderFragment);
            }
        }
        #endregion
        #region FragmentSetNext()
        /// <summary>
        /// This method sets the next fragment type.
        /// </summary>
        /// <returns>Returns the fragment object.</returns>
        protected override IXimuraMessage FragmentSetNext()
        {
            if (FragmentFirst == null)
                return FragmentSetNext(typeof(CSVHeaderFragment));

            return FragmentSetNext(typeof(CSVDataFragment));
        }
        #endregion  
    }
}
