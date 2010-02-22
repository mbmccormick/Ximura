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
using Ximura.Helper;
using Ximura.Collections;
#endregion // using
namespace Ximura.UnitTest
{
    /// <summary>
    /// Summary description for TestCollections
    /// </summary>
    [TestClass]
    public class Test_Collections_Dictionary
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
        public void TestLockFreeDictionary()
        {
            IDictionary<int, Guid> dict = new ConcurrentDictionary<int, Guid>();

            KeyValuePair<int, Guid> test = new KeyValuePair<int, Guid>(-64, Guid.NewGuid());

            dict.Add(42, Guid.NewGuid());
            dict.Add(22, Guid.NewGuid());
            dict.Add(test);
            dict.Add(55, Guid.NewGuid());

            Assert.IsTrue(dict.ContainsKey(-64));
            Assert.IsTrue(dict.Contains(test));
            Assert.IsFalse(dict.Contains(new KeyValuePair<int, Guid>(-64, new Guid())));

            dict[-64] = Guid.NewGuid();
            Assert.IsFalse(dict.Contains(test));

            Guid newID = Guid.NewGuid();
            dict[12] = newID;
            Guid id = dict[12];

            Assert.IsTrue(newID == id);

            Assert.IsTrue(dict.Count == 5);

            dict.Remove(-64);
            Assert.IsTrue(dict.Count == 4);

        }
    }
}
