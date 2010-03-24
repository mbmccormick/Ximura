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
﻿#region using
using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

using Ximura;
using CH = Ximura.Common;
using RH = Ximura.Reflection;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This base class provides the base attribute functionality for Ximura Attributes that have to load
    /// data from the assembly.
    /// </summary>
    public abstract class XimuraSchemaAttribute : Attribute
    {
        #region Static Declarations
        private static Dictionary<Uri, byte[]> sResourceCache;
        private static object syncCollection = new object();
        #endregion // Static Declarations
        #region Static Constructor
        static XimuraSchemaAttribute()
        {
            sResourceCache = new Dictionary<Uri, byte[]>();
        }
        #endregion // Static Constructor

        #region Declarations
        private Uri mResPath = null;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="resPath">The resource path of the default data for the DataContent.</param>
        public XimuraSchemaAttribute(string resPath)
        {
            mResPath = new Uri(resPath);
        }
        #endregion // Constructor
        #region ResPath
        /// <summary>
        /// This is the Uri of the corresponding internal resource for the external path.
        /// </summary>
        public Uri ResPath
        {
            get { return mResPath; }
        }
        #endregion // ResPath
        #region ResPathData
        /// <summary>
        /// This property resolves the embedded resource and returns it as a byte array.
        /// </summary>
        public byte[] ResPathData
        {
            get
            {
                return LoadSchemaFromUri(mResPath);
            }
        }
        #endregion // ResPathData

        #region LoadSchemaFromUri(Uri location)
        private static byte[] LoadSchemaFromUri(Uri location)
        {
            RH.ResourceCheck(location);

            if (sResourceCache.ContainsKey(location))
                return sResourceCache[location];

            Type locationType = RH.ResourceResolveType(location);
            byte[] data = RH.ResourceResolve(locationType, location.Segments[2]);

            lock (syncCollection)
            {
                if (!sResourceCache.ContainsKey(location))
                    sResourceCache.Add(location, data);
            }

            return data;
        }
        #endregion // LoadSchemaFromUri(Uri location)

        //#region LoadSchemaFromAssembly(Type classType, string resourceFileName)
        ///// <summary>
        ///// This method loads a stream from the assembly or related child assemblies.
        ///// </summary>
        ///// <param name="classType">The class type.</param>
        ///// <param name="resourceFileName">The resource file name.</param>
        ///// <returns>The stream, or null if the stream cannot be found.</returns>
        //protected Stream LoadSchemaFromAssembly(Type classType, string resourceFileName)
        //{
        //    Stream strmConfig = classType.Assembly.GetManifestResourceStream(resourceFileName);

        //    if (strmConfig == null)
        //    {
        //        //Stream is still null let's try the loaded assemblies instead
        //        foreach (Assembly theAssembly in AppDomain.CurrentDomain.GetAssemblies())
        //        {
        //            try
        //            {
        //                strmConfig = theAssembly.GetManifestResourceStream(resourceFileName);
        //            }
        //            catch { }
        //            if (strmConfig != null)
        //                break;
        //        }
        //    }
        //    return strmConfig;
        //}
        //#endregion // LoadSchemaFromAssembly(Type classType, string resourceFileName)

        //#region schemaName
        ///// <summary>
        ///// This is the schema name for the DataContent object.
        ///// </summary>
        //public string schemaName
        //{
        //    //get { return mschemaName; }
        //    get { return ""; }
        //}
        //#endregion // schemaName
        //#region dataName
        ///// <summary>
        ///// The new record content location.
        ///// </summary>
        //public string dataName
        //{
        //    //get { return mdataName; }
        //    get { return ""; }
        //}
        //#endregion // dataName
        //#region supportNewRecord
        ///// <summary>
        ///// This is true if the DataContent supports a new record
        ///// </summary>
        //public bool supportNewRecord
        //{
        //    //get { return mdataName != null; }
        //    get { return false; }
        //}
        //#endregion // supportNewRecord
        //#region getSchemaStream(Type classType)
        ///// <summary>
        ///// This method returns the specific schema stream
        ///// </summary>
        ///// <param name="classType">The class type.</param>
        ///// <returns>The stream, or null if the stream cannot be found.</returns>
        //public Stream getSchemaStream(Type classType)
        //{
        //    //return LoadSchemaFromAssembly(classType, this.schemaName);
        //    return null;
        //}
        //#endregion // getSchemaStream(Type classType)
        //#region getNewRecordStream(Type classType)
        ///// <summary>
        ///// This method returns a new reocrd stream.
        ///// </summary>
        ///// <param name="classType">The class type.</param>
        ///// <returns>The stream, or null if the stream cannot be found.</returns>
        //public Stream getNewRecordStream(Type classType)
        //{
        //    if (!this.supportNewRecord) return null;
        //    //return LoadSchemaFromAssembly(classType, this.dataName);
        //    return null;
        //}
        //#endregion // getNewRecordStream(Type classType)
    }
}
