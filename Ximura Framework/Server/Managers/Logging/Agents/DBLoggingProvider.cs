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

//#region using
//using System;
//using System.Diagnostics;
//using System.Data;
//using System.Data.Common;
//using System.Data.SqlClient;

//using Ximura.Helper;
//using SHL=Ximura.Helper.SQLHelper;

//using Ximura.Server;
//#endregion // using
//namespace Ximura.Logging
//{
//    /// <summary>
//    /// The DBLoggingProvider logs specific log data to a database.
//    /// </summary>
//    public class DBLogger : BaseLogger
//    {

//        /// <summary>
//        /// This method does nothing in the null logger.
//        /// </summary>
//        /// <param name="message">The message</param>
//        /// <param name="category">The category</param>
//        public override void WriteLine(string message)
//        {

//        }

//        /// <summary>
//        /// This method does nothing in the null logger.
//        /// </summary>
//        /// <param name="message">The message</param>
//        /// <param name="category">The category</param>
//        public override void Write(string message)
//        {

//        }
//        #region IXimuraLogging - Write
//        /// <summary>
//        /// Writes a category name and the value of the object's ToString method to 
//        /// the trace listeners in the Listeners collection.
//        /// </summary>
//        /// <param name="value">An object whose name is sent to the Listeners.</param>
//        /// <param name="category">A category name used to organize the output.</param>
//        public override void Write(Object value, String category)
//        {
//            DBWrite(category,value.ToString());
//        }
//        /// <summary>
//        /// Writes a category name and message to the trace listeners in the Listeners
//        ///  collection.
//        /// </summary>
//        /// <param name="message">A message to write. </param>
//        /// <param name="category">A category name used to organize the output.</param>
//        public override void Write(String message, String category)
//        {
//            DBWrite(category,message);
//        }
//        #endregion

//        #region DBWrite Methods

//        private void DBWrite(string category, string message)
//        {
//            DBWrite( LogMachine, this.Name, category, message, this.IndentLevel);
//        }		
//        private void DBWrite(string message)
//        {
//            DBWrite( LogMachine, this.Name, "", message, this.IndentLevel);
//        }
//// Astrar adds dbconn and dbtran everytime calling dbwrite instead of using global dbconn object
//        private void DBWrite(string machine, string source, string category, string message, int indent)
//        {
//            int DBStatus = (int)SQLDBReturnStatus.OK_200;

//            string strSQLConn = settings.GetSetting("dbConnectionString");

//            using(SqlConnection sqlConn = SHL.CreateSqlCONN(strSQLConn))
//            {
//                string strTransactionName = "LoggerWrite";
//                SqlTransaction sqlTrans = null;

//                try
//                {
//                    sqlConn.Open();

//                    // create sql transaction
//                    sqlTrans = SHL.CreateSqlTRANS(sqlConn, strTransactionName);

//                    DataSet dsResult = new DataSet();

//                    SqlCommand insertCmd = SHL.CreateSqlCMD("InsertLogEntry");

//                    SHL.AddSqlParam(ref insertCmd, "@machine", machine, SqlDbType.VarChar, 255);
//                    SHL.AddSqlParam(ref insertCmd, "@source", source, SqlDbType.VarChar, 255);
//                    SHL.AddSqlParam(ref insertCmd, "@category", category, SqlDbType.VarChar, 255);
//                    SHL.AddSqlParam(ref insertCmd, "@message", message, SqlDbType.NVarChar, 2000);
//                    SHL.AddSqlParam(ref insertCmd, "@indent", indent, SqlDbType.TinyInt);

//                    DBStatus = SHL.ExecuteSQL(insertCmd, ref dsResult, sqlConn, sqlTrans);

//                    //Commit the Transaction if DBStatus is (int)SQLDBReturnStatus.OK_200, otherwise Rollback
//                    if (DBStatus == (int)SQLDBReturnStatus.OK_200)
//                    {
//                        SHL.CommitSqlTRANS(ref sqlTrans, strTransactionName);
//                    }
//                    else
//                    {
//                        SHL.RollbackSqlTRANS(ref sqlTrans, strTransactionName);					
//                    }
//                }
//                catch (Exception)
//                {
//                    SHL.RollbackSqlTRANS(ref sqlTrans, strTransactionName);
//                }			
//                finally
//                {
//                    sqlConn.Close();
//                }
//            }
//        }
//// Astrar comments this out
////		private void DBWrite(string machine, string source, string category, string message, int indent)
////		{
////			if (conn == null) return;
////
////			try
////			{
////				SqlCommand insertCmd = SHL.CreateSqlCMD("InsertLogEntry");
////
////				SHL.AddSqlParam(ref insertCmd, "@machine", machine, SqlDbType.VarChar, 255);
////				SHL.AddSqlParam(ref insertCmd, "@source", source, SqlDbType.VarChar, 255);
////				SHL.AddSqlParam(ref insertCmd, "@category", category, SqlDbType.VarChar, 255);
////				SHL.AddSqlParam(ref insertCmd, "@message", message, SqlDbType.NVarChar, 2000);
////				SHL.AddSqlParam(ref insertCmd, "@indent", indent, SqlDbType.TinyInt);
////
////				insertCmd.Connection=conn;
////
////				// Astrar added:
////				// Add Debug info
////				Debug.WriteLine("Log Entry Command: InsertLogEntry @machine='" + machine.ToString().Trim() 
////					+ "',  @source='" + source.ToString().Trim() 
////					+ "',  @category='" + category.ToString().Trim() 
////					+ "',  @message='" + message.ToString().Trim() 
////					+ "',  @indent='" + indent.ToString() + "'");
////
////				int response = insertCmd.ExecuteNonQuery();
////			}
////			catch (Exception ex)
////			{
////				// Astrar added:
////				// Error encountered: "System.InvalidOperationException: There is already an open DataReader associated with this Connection which must be closed first.\r\n   at System.Data.SqlClient.SqlCommand.ValidateCommand(String method, Boolean executing)\r\n   at System.Data.SqlClient.SqlCommand.ExecuteReader(CommandBehavior cmdBehavior, RunBehavior runBehavior, Boolean returnStream)\r\n   at System.Data.SqlClient.SqlCommand.ExecuteNonQuery()\r\n   at Ximura.Logging.DBLogger.DBWrite(String machine, String source, String category, String message, Int32 indent) in C:\\Documents and Settings\\astrar.lam\\My Documents\\SLA Code\\Ximura Framework\\Ximura\\Applications\\Logging\\DBLoggingProvider.cs:line 125"
////				// Add Debug info
////				Debug.WriteLine("An exception of type " + ex.GetType() + "was encountered while saving the Log.\n" 
////					+ "Exception Text: " + ex.ToString());
////
////			}
////		}

//        #endregion
//    }
//}
