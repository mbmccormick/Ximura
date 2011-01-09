#region using
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ximura;
#endregion  
namespace Ximura.UnitTest
{
    /// <summary>
    /// This class is used to test the CSV functionality.
    /// </summary>
    [TestClass]
    public class CSV
    {
        #region CSVTest1()
        /// <summary>
        /// This test loads a large standard CSV file.
        /// </summary>
        [TestMethod]
        public void CSVTest1()
        {
            CSVMessage message = new CSVMessage(true);

            using (Stream data = GetType().ResourceAsStream("Ximura.UnitTest.Messaging.CSV.Examples.stop_times.txt"))
            {
                message.Load(data);
            }
        }
        #endregion

        #region CSVTestRFC4180()
        /// <summary>
        /// This test loads a CSV file with a number of boundary conditions that should load and parse correctly.
        /// </summary>
        [TestMethod]
        public void CSVTestRFC4180()
        {
            CSVMessage message = new CSVMessage(true);

            try
            {
                using (Stream data = GetType().ResourceAsStream("Ximura.UnitTest.Messaging.CSV.Examples.TestRFC4180.csv"))
                {
                    message.Load(data);
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
