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
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class ContentCompilerMessageFragmentBody : InternetMessageFragmentBody
    {
        private string mContentType;
        private string mContentEncoding;

        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public ContentCompilerMessageFragmentBody()
            : base()
        {
        }
        #endregion

        public override void Reset()
        {
            mContentType = null;
            mContentEncoding = null;
            base.Reset();
        }

        #region ContentType
        public override bool HasContentType
        {
            get { return mContentType != null; }
        }

        public override string ContentType
        {
            get
            {
                return mContentType;
            }
            set
            {
                mContentType = value;
            }
        }
        #endregion // ContentType

        public override bool HasContentEncoding
        {
            get
            {
                return mContentEncoding!=null;
            }
        }

        public override string ContentEncoding
        {
            get
            {
                return mContentEncoding;
            }
            set
            {
                mContentEncoding = value;
            }
        }
    }
}
