#region using
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ximura;
#endregion // using
namespace Ximura.UnitTest
{
    /// <summary>
    /// This test class is used to test the CSVStreamEnumerator.
    /// </summary>
    [TestClass]
    public class Test_CSVStreamEnumerator
    {
        #region TestCSVFile(string resource, int? totalItems, int? totalHeaders)
        /// <summary>
        /// This is a generic test method that processes a stream through the CSV enumerator.
        /// </summary>
        /// <param name="resource">The resource name.</param>
        /// <param name="totalItems">The number of items to check against, or null if not required.</param>
        /// <param name="totalHeaders">The total number of headers to confirm, or null if not needed.</param>
        /// <returns>Returns the number of records in the CSV file.</returns>
        private double TestCSVFile(string resource, int? totalItems, int? totalHeaders)
        {
            int items = 0;
            DateTime start = DateTime.Now;
            
            try
            {
                using (Stream data = GetType().ResourceAsStream(resource))
                {
                    CSVStreamEnumerator enumCSV = new CSVStreamEnumerator(data);

                    if (totalHeaders.HasValue)
                    {
                        Assert.AreEqual(totalHeaders.Value, enumCSV.Headers.Count);
                    }

                    foreach (CSVRowItem item in enumCSV)
                    {
                        string line = item.ToString();

                        items++;
                    }
                }
            }
            catch (CSVStreamEnumeratorException csvex)
            {

            }
            catch (Exception ex)
            {

            }

            TimeSpan length = DateTime.Now - start;

            if (totalItems.HasValue)
                Assert.AreEqual(items, totalItems.Value);

            return length.TotalSeconds/(double)(items+1);
        }
        #endregion // TestCSVFile(string resource, int? totalItems, int? totalHeaders)

        [TestMethod]
        public void TestCSV_LargeFile()
        {
            double average = TestCSVFile("Ximura.UnitTest.Messaging.CSV.Examples.stop_times.txt", null, null);       
        }

        [TestMethod]
        public void TestCSV_VeryLargeFile()
        {
            double average = TestCSVFile("Ximura.UnitTest.Helper.Enumerators.Data.stop_times.txt", null, null);
        }

        [TestMethod]
        public void TestCSV_RFC4180()
        {
            using (Stream data = GetType().ResourceAsStream("Ximura.UnitTest.Messaging.CSV.Examples.TestRFC4180.csv"))
            {
                CSVStreamEnumerator enumCSV = new CSVStreamEnumerator(data);

                int items = 0;
                foreach (CSVRowItem item in enumCSV)
                {
                    string line = item.ToString();

                    items++;
                }
            }
        }


        [TestMethod]
        public void TestCSV_Error1()
        {
            double average = TestCSVFile("Ximura.UnitTest.Helper.Enumerators.Data.Error1.csv", null, null);
        }

        [TestMethod]
        public void TestCSV_Problem1()
        {
            double average = TestCSVFile("Ximura.UnitTest.Helper.Enumerators.Data.Test1.csv", null, null);
        }

        [TestMethod]
        public void TestCSV_Problem2()
        {
            double average = TestCSVFile("Ximura.UnitTest.Helper.Enumerators.Data.Test2.csv", null, null);
        }

        [TestMethod]
        public void TestCSV_Problem3()
        {
            double average = TestCSVFile("Ximura.UnitTest.Helper.Enumerators.Data.Test3.csv", null, null);
        }

        [TestMethod]
        public void TestCSV_Problem4()
        {
            double average = TestCSVFile("Ximura.UnitTest.Helper.Enumerators.Data.Test4.csv", null, null);
        }

        [TestMethod]
        public void TestCSV_Problem5()
        {
            double average = TestCSVFile("Ximura.UnitTest.Helper.Enumerators.Data.Test5.csv", null, null);
        }

        [TestMethod]
        public void TestCSV_Problem6()
        {
            double average = TestCSVFile("Ximura.UnitTest.Helper.Enumerators.Data.Test6.csv", null, null);
        }
    }
}
