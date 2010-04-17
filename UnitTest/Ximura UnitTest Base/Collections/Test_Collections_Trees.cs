#region using
using System;
using System.Linq;
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
    /// Summary description for TestTree
    /// </summary>
    [TestClass]
    public class Test_Collections_Trees
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
        public void Test_TreeAdd()
        {
            LockFreeRedBlackTree<int, int> Tree = new LockFreeRedBlackTree<int, int>();

            int start = 50;

            ////LinqHelper.For(() => 42, (i) => i < 400, i => i + 2);
            //Threading.ExecuteParallel(new Action[]
            //{
            //    () => Enumerable.Range(0,100000).ForEach(i => Tree.Add(i,i)),
            //    () => Enumerable.Range(100000,100000).ForEach(i => Tree.Add(i,i)),
            //    () => Enumerable.Range(200000,100000).ForEach(i => Tree.Add(i,i)),
            //    () => Enumerable.Range(300000,100000).ForEach(i => Tree.Add(i,i)),
            //});

            Tree.Add(10, 100);
            Tree.Add(2, 100);
            Tree.Add(3, 100);
            Tree.Add(11, 100);
            Tree.Add(1, 100);

            Assert.IsTrue(Tree.ContainsKey(3));
            Assert.IsFalse(Tree.ContainsKey(19));
        }
    }
}
