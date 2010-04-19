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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

using Ximura;
using Ximura.Data;
using Ximura.Data;
using Ximura.Framework;
using Ximura.Framework;

using CH = Ximura.Common;
using AH = Ximura.AttributeHelper;
#endregion
namespace Ximura.Data
{
    /// <summary>
    /// This is the base persistence manager for SQL based storage.
    /// </summary>
    //[CDSStateActionPermit(CDSStateAction.Create)]
    //[CDSStateActionPermit(CDSStateAction.Update)]
    //[CDSStateActionPermit(CDSStateAction.Read)]
    //[CDSStateActionPermit(CDSStateAction.VersionCheck)]
    //[CDSStateActionPermit(CDSStateAction.ResolveReference)]
    //[CDSStateActionPermit(CDSStateAction.Delete)]
    //[CDSStateActionPermit(CDSStateAction.Restore)]
    //[CDSStateActionPermit(CDSStateAction.Browse)]
    public partial class SQLDBPersistenceManager<CONT, DCONT> : PersistenceManagerCDSState<CONT, DCONT, SQLDBPMCDSConfiguration>
        where CONT : DCONT
        where DCONT : Content
    {
        #region Declarations
        protected IXimuraStorageSQLServerConnectivity sqlStorageManager = null;

        /// <summary>
        /// This structure holds the identifier data for a request.
        /// </summary>
        protected struct RQType
        {
            public Type EntityType;
            public Guid Tid;
            public Guid? Cid;
            public Guid? Vid;

            public RQType(CDSRequestFolder Request)
            {
                EntityType = Request.DataType;
                Tid = EntityType.GetContentTypeAttributeID();
                Cid = Request.DataContentID;
                Vid = Request.DataVersionID;
            }
        }

        #endregion // Declarations
        #region Constructors
		/// <summary>
		/// This is the empty constructor for the persistence manager
		/// </summary>
		public SQLDBPersistenceManager():this(null)
		{
		}
		/// <summary>
		/// This is the component model constructor for the persistence manager.
		/// </summary>
		/// <param name="container">The container.</param>
        public SQLDBPersistenceManager(System.ComponentModel.IContainer container) : base(container) { }
		#endregion

        #region ServicesReference()
        /// <summary>
        /// This override gets the reference to the SQL server connectivity service.
        /// </summary>
        protected override void ServicesReference()
        {
            base.ServicesReference();
            sqlStorageManager = GetService<IXimuraStorageSQLServerConnectivity>();
        }
        #endregion // ServicesReference()
        #region ServicesDereference()
        /// <summary>
        /// This override removes the reference to the SQL server connectivity service.
        /// </summary>
        protected override void ServicesDereference()
        {
            sqlStorageManager = null;
            base.ServicesDereference();
        }
        #endregion // ServicesDereference()


        #region SQLConnection
        protected virtual string SQLConnectionResolve(CDSContext context)
        {
            return SQLConnectionResolve(context, "");
        }

        protected virtual string SQLConnectionResolve(CDSContext context, string connType)
        {
            return context.ContextSettings.ResolveConnectionString(connType);
        }
        #endregion // SQLConnection

        protected virtual TypeConverter GetTypeConverter()
        {
            return null;
        }

        protected virtual byte[] ParseEntity(Content data)
        {
            return data.Serialize();
        }

        protected virtual byte[] ParseAttributes(Content data, TypeConverter conv)
        {
            try
            {
                List<KeyValuePair<PropertyInfo, CDSAttributeAttribute>> attrList = 
                    AH.GetPropertyAttributes<CDSAttributeAttribute>(data.GetType());

                List<KeyValuePair<CDSAttributeAttribute, string>> listData = 
                    new List<KeyValuePair<CDSAttributeAttribute, string>>();

                foreach (KeyValuePair<PropertyInfo, CDSAttributeAttribute> reference in attrList)
                {
                    PropertyInfo pi = reference.Key;

                    try
                    {
                        if (pi.PropertyType == typeof(string))
                        {
                            listData.Add(new KeyValuePair<CDSAttributeAttribute, string>(reference.Value, pi.GetValue(data, null) as string));
                        }
                        else if (pi.PropertyType == typeof(IEnumerable<string>))
                        {
                            IEnumerable<string> enumerator = pi.GetValue(data, null) as IEnumerable<string>;
                            foreach (string value in enumerator)
                                listData.Add(new KeyValuePair<CDSAttributeAttribute, string>(reference.Value, value));
                        }
                        else if (conv!=null && conv.CanConvertFrom(pi.PropertyType))
                        {
                            listData.Add(new KeyValuePair<CDSAttributeAttribute, string>(reference.Value, conv.ConvertToString(pi.GetValue(data, null) )));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8);
                    WriteValue(bw, listData);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected virtual byte[] ParseReferences(Content data, TypeConverter conv)
        {
            try
            {
                List<KeyValuePair<PropertyInfo, CDSReferenceAttribute>>
                    attrList = AH.GetPropertyAttributes<CDSReferenceAttribute>(data.GetType());

                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8);

                    bw.Write(attrList.Count);

                    foreach (KeyValuePair<PropertyInfo, CDSReferenceAttribute> reference in attrList)
                    {
                        PropertyInfo pi = reference.Key;

                        if (pi.PropertyType != typeof(string) && 
                            (conv==null || !conv.CanConvertFrom(pi.PropertyType)))
                            continue;

                        string value;

                        if (pi.PropertyType == typeof(string))
                            value = pi.GetValue(data, null) as string;
                        else
                            value = conv.ConvertToString(pi.GetValue(data, null));

                        string type = reference.Value.Name;

                        bw.Write(type);
                        bw.Write(value);
                    }

                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void WriteValue(BinaryWriter bw, List<KeyValuePair<CDSAttributeAttribute, string>> data)
        {
            bw.Write(data.Count);
            foreach (KeyValuePair<CDSAttributeAttribute, string> key in data)
                WriteValue(bw, key.Key, key.Value);
        }

        protected void WriteValue(BinaryWriter bw, CDSAttributeAttribute attr, string value)
        {
            bw.Write(attr.Name);
            bw.Write(attr.Description);
            bw.Write(attr.Language);
            bw.Write(value);
        }
    }
}
