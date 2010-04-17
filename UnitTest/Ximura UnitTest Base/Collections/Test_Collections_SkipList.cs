#region using
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ximura;

using Ximura.Collections;
#endregion // using
namespace Ximura.UnitTest
{
    /// <summary>
    /// Summary description for Test_Collections_SkipList
    /// </summary>
    [TestClass]
    public class Test_Collections_SkipList
    {
        #region TestContext
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get;
            set;
        }
        #endregion // TestContext
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestLockFreeSkipList_Int()
        {
            ConcurrentListSLC<int> listTest = new ConcurrentListSLC<int>();

            listTest.Add(42);
            listTest.Add(22);
            listTest.Add(22);
            listTest.Add(-64);
            listTest.Add(55);
            listTest.Add(0);

            Assert.IsTrue(listTest.Count == 6);
            Assert.IsTrue(listTest.Contains(0));
            Assert.IsTrue(listTest.Remove(0));
            Assert.IsFalse(listTest.Contains(0));

            Assert.IsTrue(listTest.Contains(42));
            Assert.IsFalse(listTest.Contains(142));
            Assert.IsFalse(listTest.Contains(2));
            Assert.IsTrue(listTest.Contains(-64));

            listTest.Remove(22);
            listTest.Remove(2222);

            Assert.IsTrue(listTest.Contains(22));
            Assert.IsTrue(listTest.Remove(22));
            Assert.IsFalse(listTest.Remove(22));
            Assert.IsFalse(listTest.Contains(22));

            Assert.IsTrue(listTest.Count == 3);

            listTest.Add(42);
            listTest.Add(41);
            listTest.Add(0);

            Assert.IsTrue(listTest.Count == 6);

            listTest.Add(0);
            Assert.IsTrue(listTest.Count == 7);
            Assert.IsTrue(listTest.Remove(0));

            Assert.IsTrue(listTest.Count == 6);

            Assert.IsTrue(listTest.Contains(41));

            Assert.IsTrue(listTest.Remove(0));
            Assert.IsFalse(listTest.Remove(0));
            Assert.IsFalse(listTest.Remove(0));

            Assert.IsTrue(listTest.Remove(42));
            Assert.IsTrue(listTest.Remove(55));

            listTest.Add(-122);
            listTest.Add(-223);
            listTest.Add(int.MaxValue);
            listTest.Add(int.MinValue);
            Assert.IsTrue(listTest.Contains(int.MaxValue));
            Assert.IsTrue(listTest.Contains(int.MinValue));

            listTest.Add(56);

            Assert.IsTrue(listTest.Count == 8);

            int[] data = new int[10];
            listTest.CopyTo(data, 1);
        }

    }
}
