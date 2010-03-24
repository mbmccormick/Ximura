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
using Ximura.Framework;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Data
{
    public partial class DataContent
    {
        #region Declarations
        private static object syncXSDLock = new object();
        private static Dictionary<Type, byte[]> SchemaCache = null;
        private static Dictionary<Type, byte[]> NewEntityCache = null;

        //private delegate Stream DelGetAttrStream(Type itemType, XimuraDataContentSchemaAttribute schemaAttr);

        //private static DelGetAttrStream DelGetSchema =
        //    delegate(Type itemType, XimuraDataContentSchemaAttribute schemaAttr)
        //    { return schemaAttr.getSchemaStream(itemType); };

        //private static DelGetAttrStream DelGetNewData =
        //    delegate(Type itemType, XimuraDataContentSchemaAttribute schemaAttr)
        //    { return schemaAttr.getNewRecordStream(itemType); };
        #endregion // Declarations

        #region Static Constructor
        static DataContent()
        {
            SchemaCache = new Dictionary<Type, byte[]>();
            NewEntityCache = new Dictionary<Type, byte[]>();
        }
        #endregion



        ///// <summary>
        ///// This static method resolves an internal resource and returns it as a string.
        ///// </summary>
        ///// <param name="baseType">The base type that defines the assembly to search within.</param>
        ///// <param name="ResourceID">The resource identifier.</param>
        ///// <returns>Returns a stream or null if the stream cannot be resolved.</returns>
        //public static Stream ResolveEmbeddedResourceToStream(Type baseType, string ResourceID)
        //{
        //    if (HelperCache.ContainsKey(ResourceID))
        //        if (HelperCache[ResourceID] == null)
        //            return null;
        //        else
        //            return new MemoryStream(HelperCache[ResourceID],false);

        //    lock (syncXSDLock)
        //    {
        //        //Fix any race conditions
        //        if (HelperCache.ContainsKey(ResourceID))
        //            if (HelperCache[ResourceID] == null)
        //                return null;
        //            else
        //                return new MemoryStream(HelperCache[ResourceID], false);

        //        using (Stream schemaStream = baseType.Assembly.GetManifestResourceStream(ResourceID))
        //        {
        //            if (schemaStream == null)
        //            {
        //                HelperCache.Add(ResourceID, null);
        //                return null;
        //            }

        //            byte[] buffer = new byte[schemaStream.Length];

        //            schemaStream.Read(buffer, 0, buffer.Length);

        //            HelperCache.Add(ResourceID, buffer);

        //            return new MemoryStream(buffer,false);
        //        }

        //    }
        //}

        //protected virtual Stream ResolveEmbeddedResourceToStream(string ResourceID)
        //{
        //    Stream output = ResolveEmbeddedResourceToStream(typeof(DataContent), ResourceID);
        //    return output;
        //}

    }
}
