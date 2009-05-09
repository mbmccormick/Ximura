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
using Ximura.Helper;
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
        public Test_Collections_Trees()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

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
        public void TestMethod1()
        {
            ExpandableFineGrainedLockArray<int> hello = new ExpandableFineGrainedLockArray<int>();

            hello[3939] = 42;
            hello[42] = 42;
        }
    }
}
