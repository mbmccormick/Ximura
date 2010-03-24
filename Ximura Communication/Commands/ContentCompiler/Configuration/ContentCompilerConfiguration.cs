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
using System.Linq;
using System.Collections.Generic;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using CDS = Ximura.Data.CDSHelper;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    [XimuraContentTypeID("{36A148B1-E930-424d-8344-0DA51D6BFE7A}")]
    [XimuraDataContentSchema("http://schema.ximura.org/configuration/contentcompiler/1.0",
       "xmrres://XimuraComm/Ximura.Communication.ContentCompilerConfiguration/Ximura.Communication.Commands.ContentCompiler.Configuration.ContentCompilerConfiguration.xsd")]
    public class ContentCompilerConfiguration : FSMCommandConfiguration
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor for the Content object.
        /// </summary>
        public ContentCompilerConfiguration()
        {
        }
        #endregion

        #region XPScAdd(Dictionary<string, string> mappingShortcuts)
        /// <summary>
        /// This method adds the XPath shortcuts in to the collection. You should
        /// override this method to add your own shorcuts.
        /// </summary>
        /// <param name="mappingShortcuts">The mapping shorcut collection.</param>
        protected override void XPScAdd(Dictionary<string, string> mappingShortcuts)
        {
            string basePath = "//r:ContentCompilerConfiguration";
            mappingShortcuts.Add("r", basePath);
        }
        #endregion // XPScAdd(Dictionary<string, string> mappingShortcuts)
    }
}
