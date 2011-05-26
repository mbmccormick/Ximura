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
    [TestClass]
    public class Test_CSVRowItem
    {
        [TestMethod]
        public void TestCSVRowItem_SimpleLine()
        {
            CSVRowItem item1 = new CSVRowItem(CSVStreamEnumeratorOptions.Default
                , "123456789ABCDEFG".ToCharArray()
                , new KeyValuePair<int,int>[]
                {
                     new KeyValuePair<int,int>(0,0)
                    ,new KeyValuePair<int,int>(0,4)
                    ,new KeyValuePair<int,int>(4,3)
                    ,new KeyValuePair<int,int>(7,3)
                    ,new KeyValuePair<int,int>(10,6)
                });

            string[] items = item1.ToArray();

            Assert.IsTrue(items.Length == 5);
        }

        //[TestMethod]
        //public void TestCSVRowItem_SimpleLineUnixLinefeed()
        //{
        //    CSVRowItem item1 = new CSVRowItem(null, "123,456,789,101112\n");

        //    string[] items = item1.ToArray();

        //    Assert.IsTrue(items.Length == 4);
        //}

        //[TestMethod]
        //public void TestCSVRowItem_SimpleLineEmptyRecord()
        //{
        //    CSVRowItem item1 = new CSVRowItem(null, "123,456,,101112\r\n");

        //    string[] items = item1.ToArray();

        //    Assert.IsTrue(items.Length == 4);
        //}

        //[TestMethod]
        //public void TestCSVRowItem_SimpleLineNoLineEnd1()
        //{
        //    CSVRowItem item1 = new CSVRowItem(null, "123,456,789,101112");

        //    string[] items = item1.ToArray();
        //    Assert.IsTrue(items.Length == 4);
        //}

        //[TestMethod]
        //public void TestCSVRowItem_SimpleLineNoLineEnd2()
        //{
        //    CSVRowItem item1 = new CSVRowItem(null, "123,456,789,");

        //    string[] items = item1.ToArray();
        //    Assert.IsTrue(items.Length == 4);
        //}

        //[TestMethod]
        //public void TestCSVRowItem_SimpleSpeechMarks()
        //{
        //    CSVRowItem item1 = new CSVRowItem(null, "123,456,\"789\",101112\r\n");

        //    string[] items = item1.ToArray();

        //    Assert.IsTrue(items.Length == 4);
        //}

    }
}
