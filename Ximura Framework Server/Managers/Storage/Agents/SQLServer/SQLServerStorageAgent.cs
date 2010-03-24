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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Security;
using System.Security.Cryptography;
using System.Security.Principal;

using Ximura;

using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Framework
{
    public class SQLServerStorageAgent:StorageAgentBase
    {

        #region VerifyExternalConnectivity()
        /// <summary>
        /// This method verifies all the external connections.
        /// </summary>
        protected virtual void VerifyExternalConnectivity()
        {

            //ICDSConfigSH cdsSettings = CommandSettings as ICDSConfigSH;
            //if (cdsSettings == null)
            //    throw new DataException("No CDS Settings has been found.");

            //if (!cdsSettings.SkipConnectivityTest)
            //{
            //    Dictionary<string, string> htConnectionString = cdsSettings.GetAllConnectionStrings();

            //    foreach (string sqlConnStr in htConnectionString.Keys)
            //    {
            //        using (SqlConnection sqlConn = new SqlConnection(htConnectionString[sqlConnStr]))
            //        {
            //            try
            //            {
            //                sqlConn.Open();
            //            }
            //            catch (SqlException sqlEx)
            //            {
            //                XimuraAppTrace.WriteLine("Cannot connect to the database for connection: " + sqlConnStr,
            //                    "VerifyExternalConnectivity", EventLogEntryType.Error);
            //                throw sqlEx;
            //            }
            //            catch (Exception ex)
            //            {
            //                throw ex;
            //            }
            //            finally
            //            {
            //                sqlConn.Close();
            //            }
            //        }
            //    }
            //}
        }
        #endregion // VerifyExternalConnectivity()

    }
}
