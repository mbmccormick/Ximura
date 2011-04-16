#region using
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ximura;
using System.IO;
#endregion // using
namespace Ximura.UnitTest
{
    /// <summary>
    /// Summary description for CSVStreamEnumerator
    /// </summary>
    [TestClass]
    public class Test_CSVStreamEnumerator
    {
        [TestMethod]
        public void TestEnumeration()
        {
            using (Stream data = GetType().ResourceAsStream("Ximura.UnitTest.Messaging.CSV.Examples.stop_times.txt"))
            {
                CSVStreamEnumerator enumCSV = new CSVStreamEnumerator(data, true);

                int items = 0;
                foreach (var item in enumCSV)
                {
                    string line = item.ToString();

                    items++;
                }

                //4560
            }
            
        }
    }
}
