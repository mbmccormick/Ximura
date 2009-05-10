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
    /// This set of test classes tests the threading functions for the base framework.
    /// </summary>
    [TestClass]
    public class Test_Helper_Threading
    {
        #region Initialization
        /// <summary>
        /// The empty constructor.
        /// </summary>
        public Test_Helper_Threading()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get;
            set;
        }
        #endregion // Initialization
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
            string id = "hello";

            Thread t1 = new Thread(() => id += "\r\nBugger1 " + Thread.CurrentThread.ManagedThreadId.ToString());
            Thread t2 = new Thread(() => id += "\t\nBugger2 " + Thread.CurrentThread.ManagedThreadId.ToString());
            Thread t3 = new Thread(() => id += "\r\nBugger3 " + Thread.CurrentThread.ManagedThreadId.ToString());

            id += "\r\n\r\nHello" + Thread.CurrentThread.ManagedThreadId.ToString();
            t1.Start();
            t2.Start();
            t3.Start();

            //OK, wait for the threads to complete.
            t1.Join();
            t2.Join();
            t3.Join();

            TestContext.WriteLine("Hello");
        }
    }
}
