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
using System.Threading;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

using Ximura;
using Ximura.Helper;
using Ximura.Data;

using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This method contains a fragment
    /// </summary>
    public class HTMLErrorBodyFragment : HTMLBodyFragment
    {
        #region Declarations
        private string mErrorCode;

        private static XslCompiledTransform sErrorTran = null;
        private static object syncSetTran = new object();

        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public HTMLErrorBodyFragment()
            : base()
        {
        }
        #endregion
        #region Reset()
        /// <summary>
        /// This override resets the error code.
        /// </summary>
        public override void Reset()
        {
            mErrorCode = null;
            base.Reset();
        }
        #endregion // Reset()

        #region ContentType
        /// <summary>
        /// This property identifies that the body has a content type.
        /// </summary>
        public override bool HasContentType
        {
            get { return true; }
        } 
        /// <summary>
        /// This is the content type for the HTML error file.
        /// </summary>
        public override string ContentType
        {
            get
            {
                return "text/html; charset=utf-8";
            }
        }
        #endregion // ContentType

        public virtual string HTTPErrorType
        {
            get
            {
                return mErrorCode;
            }
        }

        #region ErrorTransform
        /// <summary>
        /// This is the base compiled error transform.
        /// </summary>
        public static XslCompiledTransform ErrorTransform
        {
            get
            {
                if (sErrorTran != null)
                    return sErrorTran;

                lock (syncSetTran)
                {
                    if (sErrorTran != null)
                        return sErrorTran;
#if DEBUG
                    XslCompiledTransform tempTran = new XslCompiledTransform(true);
#else
                    XslCompiledTransform tempTran = new XslCompiledTransform(false);
#endif
                    using (Stream strmConfig = typeof(HTMLErrorBodyFragment).Assembly.GetManifestResourceStream(
                        "Ximura.Communication.HTTP.Server.ErrorMessages.HTML.ErrorTemplate.xslt"))
                    {
                        using (XmlReader xmlR = XmlReader.Create(strmConfig))
                        {
                            tempTran.Load(xmlR);
                        }
                    }
                    sErrorTran = tempTran;
                }
                return sErrorTran;
            }
        }
        #endregion // ErrorTransform

        public void ErrorTypeSet(string errorType)
        {
            ErrorTypeSet(errorType, null, null);
        }
        public void ErrorTypeSet(string errorType, string errorMessage)
        {
            ErrorTypeSet(errorType, errorMessage, null);
        }
        public void ErrorTypeSet(string errorType, string errorMessage, string errorDescription)
        {
            if (!Initializing)
                throw new NotSupportedException("The error type cannot be set when the fragment is not initializing.");

            mErrorCode = errorType;

            using (Stream strmWrite = new MemoryStream())
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(@"<httpresponse status=""" + errorType + @"""/>");

                ErrorTransform.Transform(xDoc, null, strmWrite);

                Load(strmWrite);
            }
        }
    }
}
