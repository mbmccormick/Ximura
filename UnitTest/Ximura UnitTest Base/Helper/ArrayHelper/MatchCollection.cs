#region using
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ximura;
#endregion
namespace Ximura.UnitTest
{
    /// <summary>
    /// These set of unit test, test the complex match collection logic.
    /// </summary>
    [TestClass]
    public class MatchCollection
    {
        [TestMethod]
        public void TestCSVMethod()
        {
            Assert.AreEqual<int>(MatchPosition("1,2,2,3\r\n1,4,5,6,7\r\n"), 8);
            Assert.AreEqual<int>(MatchPosition("1,2,\"2\",3\r\n"), 9);
            Assert.AreEqual<int>(MatchPosition("1,2,\"2\r\n45\",3\r\n34,22,"), 13);
        }

        private int MatchPosition(string item)
        {
            //var state = item.MatchCollection(new CSVTerminationMatchCollectionState());

            return 0;
        }
    }
}
