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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Text;
using System.Xml.XPath;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This abstract class wraps a content based entity around a XML base object.
    /// </summary>
    /// <typeparam name="T">An XML object that implements IXPathNavigable.</typeparam>
    public abstract partial class XmlContentHolder<T>
    {
        #region Reset()
        /// <summary>
        /// This method resets the data content.
        /// </summary>
        public override void Reset()
        {
            Payload = null;
            base.Reset();

            ResetAutoLoadCheck();
        }
        #endregion // Reset()

        #region Dispose(bool disposing)
        private bool mDisposed = false;
        /// <summary>
        /// This is the dispose override.
        /// </summary>
        /// <param name="disposing">True when this is called by the dispose method.</param>
        protected override void Dispose(bool disposing)
        {
            if (!mDisposed)
            {
                if (disposing)
                {
                    Payload = null;

                    base.Dispose(disposing);
                }
                mDisposed = true;
            }
        }
        #endregion // Dispose(bool disposing)

        #region ResetAutoLoadCheck()
        /// <summary>
        /// This method will autoload the content object based on the Default Data set in the 
        /// attribute when the content is reset. If you want to change this behaviour, you can
        /// override this method.
        /// </summary>
        protected virtual void ResetAutoLoadCheck()
        {
            //If the content is set to autoload, then go ahead.
            if (attrDefaultData != null && attrDefaultData.AutoLoad)
            {
                Load();
            }
        }
        #endregion // ResetAutoLoadCheck()
    }
}
