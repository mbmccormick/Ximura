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
using System.Text;
using System.IO;
using System.Security;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Linq;

using Ximura;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// The reflection helper provides help on creating objects using reflection.
    /// </summary>
    public static partial class Reflection
    {
#if (!SILVERLIGHT)

        #region ResourceLoadFromUri(Uri location)
        /// <summary>
        /// This method loads a resource from the xmrres resource location specified.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>Returns a byte array containing the resource.</returns>
        /// <exception cref="System.ArgumentException">This exception is thrown when the resource cannot be resolved.</exception>
        public static byte[] ResourceLoadFromUri(Uri location)
        {
            ResourceCheck(location);

            Type locationType = ResourceResolveType(location);
            byte[] data = ResourceResolve(locationType, location.Segments[2]);

            return data;
        }
        #endregion // ResourceLoadFromUri(Uri location)
        #region ResourceResolveType(Uri location)
        /// <summary>
        /// This method will return the type specified in the xmrres resource string.
        /// </summary>
        /// <param name="location">The resource location.</param>
        /// <returns>Returns the type specified.</returns>
        public static Type ResourceResolveType(Uri location)
        {
            ResourceCheck(location);
            string typeName = StripSlash(location.Segments[1]) + ", " + location.Host;
            Type theType = Type.GetType(typeName, false, true);

            if (theType == null)
                theType = CreateTypeFromString(typeName);

            return theType;
        }
        #endregion // ResourceResolveType(Uri location)
        #region ResourceCheck(Uri location)
        /// <summary>
        /// This method checks whether the xmrres resource uri is correctly formatted.
        /// </summary>
        /// <param name="location">The xmrres resource location.</param>
        public static void ResourceCheck(Uri location)
        {
            if (location.Scheme.ToLower() != "xmrres")
                throw new NotSupportedException(@"The scheme """ + location.Scheme + @""" is not supported.");

            if (location.Segments.Length < 3)
                throw new ArgumentException(
                    "location does not have the required number of parts: " + location.ToString(), "location");
        }
        #endregion

        
        public static Stream ResourceLoadFromUriAsStream(Uri location)
        {
            ResourceCheck(location);

            Type locationType = ResourceResolveType(location);
            return ResourceResolveAsStream(locationType, location.Segments[2]);
        }
#endif

        #region ResourceResolve(Type locationType, string resource)
        /// <summary>
        /// This method resolves an embedded resource as a byte array.
        /// </summary>
        /// <param name="locationType">The type</param>
        /// <param name="resource">The resource to resolve.</param>
        /// <returns>Returns a byte array.</returns>
        /// <exception cref="System.ArgumentException">This exception is thrown when the resource cannot be resolved.</exception>
        public static byte[] ResourceResolve(Type locationType, string resource)
        {
            byte[] blob;

            using (Stream data = locationType.Assembly.GetManifestResourceStream(resource))
            {
                if (data == null)
                    throw new ArgumentException(@"The resource """ + resource + @""" cannot be resolved.");

                blob = new byte[data.Length];
                data.Read(blob, 0, (int)data.Length);
            }

            return blob;
        }
        #endregion // ResourceResolve(Type locationType, string resource)

        #region ResourceAsStream(this Type locationType, string resource)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationType"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static Stream ResourceAsStream(this Type locationType, string resource)
        {
            return ResourceResolveAsStream(locationType, resource);
        }
        #endregion
        #region ResourceResolveAsStream(Type locationType, string resource)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationType"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static Stream ResourceResolveAsStream(Type locationType, string resource)
        {
            Stream data = locationType.Assembly.GetManifestResourceStream(resource);

            if (data == null)
                throw new ArgumentException(@"The resource """ + resource + @""" cannot be resolved.");

            return data;
        }
        #endregion

#if (!SILVERLIGHT)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static Stream ResolveResourceFromName(string resourceName)
        {
            Stream toReturn = null;
            toReturn = ResolveResourceFromName(resourceName, AppDomain.CurrentDomain.GetAssemblies());
            if (toReturn == null)
                toReturn = ResolveResourceFromName(resourceName, Assembly.GetExecutingAssembly().GetReferencedAssemblies());
            return toReturn;
        }
#endif

        public static Stream ResolveResourceFromName(string resourceName, AssemblyName[] Asses)
        {
            Stream streamResx = null;

            foreach (AssemblyName thisAssName in Asses)
            {
                Assembly thisAss = GetAssemblyFromName(thisAssName);

                if (thisAss != null)
                {
                    streamResx = thisAss.GetManifestResourceStream(resourceName);
                    if (streamResx != null)
                        return streamResx;
                }
            }

            return null;
        }

        public static Stream ResolveResourceFromName(string resourceName, Assembly[] Asses)
        {
            Stream streamResx = null;

            foreach (Assembly thisAss in Asses)
            {
                if (thisAss != null)
                {
                    streamResx = thisAss.GetManifestResourceStream(resourceName);
                    if (streamResx != null)
                        return streamResx;
                }
            }

            return null;
        }
    }
}
