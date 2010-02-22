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
    public partial class ContentXsdResolver : XmlResolver
    {
        #region Static Declarations
        private static Dictionary<Type, ContentXsdResolver> mResolverCache;
        private static Dictionary<Uri, byte[]> mXSDResourceCache;
        private static object syncXSDLock = new object();
        #endregion // Static Declarations

        #region Static constructor
        static ContentXsdResolver()
        {
            mResolverCache = new Dictionary<Type, ContentXsdResolver>();
            mXSDResourceCache = new Dictionary<Uri, byte[]>();
        }
        #endregion // Static constructor

        #region GetResolver(Type dataContentType)
        /// <summary>
        /// This is the factory constructor for the resolver.
        /// </summary>
        /// <param name="dataContentType">The object type to resolve.</param>
        /// <returns>Returns the resolver containing the object cache.</returns>
        public static ContentXsdResolver GetResolver(Type dataContentType)
        {
            if (mResolverCache.ContainsKey(dataContentType))
                return mResolverCache[dataContentType];

            lock (syncXSDLock)
            {
                if (mResolverCache.ContainsKey(dataContentType))
                    return mResolverCache[dataContentType];
                ContentXsdResolver newResolver = new ContentXsdResolver(dataContentType);
                mResolverCache.Add(dataContentType, newResolver);

                return newResolver;
            }
        }
        #endregion // GetResolver(Type dataContentType)

        #region ResolveEmbeddedResourceToStream(Uri ResourceID, Type baseType)
        /// <summary>
        /// This static method resolves an internal resource and returns it as a string.
        /// </summary>
        /// <param name="ResourceID">The resource identifier.</param>
        /// <returns>Returns a stream or null if the stream cannot be resolved.</returns>
        protected static Stream ResolveEmbeddedResourceToStream(Uri ResourceID, Type baseType)
        {
            if (mXSDResourceCache.ContainsKey(ResourceID))
                if (mXSDResourceCache[ResourceID] == null)
                    return null;
                else
                    return new MemoryStream(mXSDResourceCache[ResourceID], false);

            lock (syncXSDLock)
            {
                //Fix any race conditions
                if (mXSDResourceCache.ContainsKey(ResourceID))
                    if (mXSDResourceCache[ResourceID] == null)
                        return null;
                    else
                        return new MemoryStream(mXSDResourceCache[ResourceID], false);

                Type itemType = null;

                if (baseType != null)
                    itemType = baseType.Assembly.GetType(StripSlash(ResourceID.Segments[1]) + ", " + ResourceID.Host, false, true);

                if (itemType == null)
                    itemType = Type.GetType(StripSlash(ResourceID.Segments[1]) + ", " + ResourceID.Host, false, true);


                using (Stream schemaStream = itemType.Assembly.GetManifestResourceStream(ResourceID.Segments[2]))
                {
                    if (schemaStream == null)
                    {
                        mXSDResourceCache.Add(ResourceID, null);
                        return null;
                    }

                    byte[] buffer = new byte[schemaStream.Length];

                    schemaStream.Read(buffer, 0, buffer.Length);

                    mXSDResourceCache.Add(ResourceID, buffer);

                    return new MemoryStream(buffer, false);
                }
            }
        }
        #endregion // ResolveEmbeddedResourceToStream(Uri ResourceID, Type baseType)

        #region StripSlash(string input)
        protected static string StripSlash(string input)
        {
            return input.Trim('/');
        }
        #endregion // StripSlash(string input)
    }
}
