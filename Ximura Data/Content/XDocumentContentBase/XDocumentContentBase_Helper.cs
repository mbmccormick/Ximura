﻿#region Copyright
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
﻿#region using
using System;
using System.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This is the base XML content class using the new .NET 3.5 Linq XML classes.
    /// </summary>
    public partial class XDocumentContentBase
    {
        //#region DumpXML
        ///// <summary>
        ///// This method is used in debug to dump the internal contents of the xml object to a file
        ///// </summary>
        ///// <param name="filename">The filename to save the file to.</param>
        //public override string DumpXML(string filename)
        //{
        //    this.XDataDoc.Save(filename);
        //    //Debug.WriteLine(filename);
        //    return filename;
        //}
        //#endregion // DumpXML()

        #region XPScBuild()
        /// <summary>
        /// This method creates the XPath shorcut collection.
        /// </summary>
        private void XPScBuild()
        {
            mNSM = new Dictionary<string, XNamespace>();
            mXMLMappingShortcuts = new Dictionary<string, string>();
            XPScAdd(mXMLMappingShortcuts);
        }
        #endregion // XPScBuild()

        #region XPScAdd(Dictionary<string, string> mappingShortcuts)
        /// <summary>
        /// This method adds the XPath shortcuts in to the collection. You should
        /// override this method to add your own shorcuts.
        /// </summary>
        /// <param name="mappingShortcuts">The mapping shorcut collection.</param>
        protected virtual void XPScAdd(Dictionary<string, string> mappingShortcuts)
        {
        }
        #endregion // XPScAdd(Dictionary<string, string> mappingShortcuts)
    }
}
