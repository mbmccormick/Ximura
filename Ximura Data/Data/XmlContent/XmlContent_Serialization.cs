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
using System.Data;

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;

using Ximura;
using Ximura.Data;
using Ximura.Server;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// The XMLContent object wraps basic persistence functionality around the XMl Data object.
    /// </summary>
    public partial class XmlContent
    {
        #region ContentBody
        /// <summary>
        /// This method returns the content body as an byte array blob.
        /// </summary>
        protected override byte[] ContentBody
        {
            get
            {
                if (XmlDataDoc == null)
                    return null;

                using (MemoryStream memStream = new MemoryStream())
                {
                    XmlDataDoc.Save(memStream);
                    return memStream.ToArray();
                }
            }
        }
        #endregion

        #region OnDeserialization(object sender)
        /// <summary>
        /// This is deserialization callback method.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public override void OnDeserialization(object sender)
        {
            mCanLoad = true;
            try
            {
                if (this.mInfo.GetInt32("bodycount") > 0)
                {
                    bool contentDirty = this.mInfo.GetBoolean("dirty");
                    byte[] blob = (byte[])this.mInfo.GetValue("body0", typeof(byte[]));

                    Load(blob, 0, blob.Length);

                    this.Dirty = contentDirty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
