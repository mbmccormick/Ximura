#region using
using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using Ximura;
using Ximura.Data;
using Ximura.Persistence;
using Ximura.Helper;
using Ximura.Server;
using Ximura.Command;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
#endregion
namespace Ximura.Communication
{
    public class ResourceBinaryPersistenceManager<CONT, DCONT, CONF> : ResourceBasePersistenceManager<CONT, DCONT, CONF>
        where CONT : DCONT
        where DCONT : BinaryContent
        where CONF : CommandConfiguration, new()
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ResourceBinaryPersistenceManager() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ResourceBinaryPersistenceManager(IContainer container)
            : base(container)
        {

        }
        #endregion // Constructors

        protected override CONT ContentCreate(CDSContext context, Guid tid, byte[] blob, KeyValuePair<string, string> key, Uri resourceUri)
        {
            //CONT data = context.GetObjectPool<CONT>().Get();
            CONT data = (CONT)context.GetObjectPool(context.Request.DataType).Get();

            data.Load();
            data.Data = blob;

            PrepareData(context, key, resourceUri, data);

            data.DCDate = CH.ConvertToISO8601DateString(DateTime.Now);
            data.PropertySet("source", this.GetType().AssemblyQualifiedName);

            return data;
        }

        protected override void PrepareData(CDSContext context, KeyValuePair<string, string> key, Uri resourceUri, CONT data)
        {
            byte[] hash = Convert.FromBase64String(data.ETag);

            data.ID = new Guid(hash);
            data.Version = new Guid(hash);

            data.FileName = key.Value;
        }
    }
}
