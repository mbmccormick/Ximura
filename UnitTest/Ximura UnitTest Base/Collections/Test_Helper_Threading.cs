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
        
        #region TestThreading_AddBad()
        /// <summary>
        /// This test shows the problem with the increment function using multiple threads.
        /// </summary>
        [TestMethod]
        public void TestThreading_AddBad()
        {
            int starttime = Environment.TickCount;

            int output = 0;
            //Create the delegate to add 1 million
            ThreadStart add1Mill = () =>
            { 
                for (int i = 0; i < 1000000; i++) 
                    output++;
            };
            //Create the two threads
            Thread t1 = new Thread(add1Mill);
            Thread t2 = new Thread(add1Mill);
            //Start the threads
            t1.Start();
            t2.Start();
            //OK, wait for the threads to complete.
            t1.Join();
            t2.Join();
            //Output the value.
            int finish = Environment.TickCount - starttime;
            Console.WriteLine("Final value is {0}", output);
        }
        #endregion // TestThreading_AddBad()
        #region TestThreading_AddGoodLock()
        /// <summary>
        /// This example shows a safe multi-threading code using lock.
        /// </summary>
        [TestMethod]
        public void TestThreading_AddGoodLock()
        {
            int starttime = Environment.TickCount;
            object tempObject = new object();

            int output = 0;
            //Create the delegate to add 1 million
            ThreadStart add1Mill = () =>
            {
                for (int i = 0; i < 1000000; i++)
                    lock (tempObject) { output++; }
            };
            //Create the two threads
            Thread t1 = new Thread(add1Mill);
            Thread t2 = new Thread(add1Mill);
            //Start the threads
            t1.Start();
            t2.Start();
            //OK, wait for the threads to complete.
            t1.Join();
            t2.Join();
            //Output the value.
            int finish = Environment.TickCount - starttime;
            Console.WriteLine("Final value is {0}", output);
        }
        #endregion // TestThreading_AddLock()
        #region TestThreading_AddGoodInterlocked()
        /// <summary>
        /// This example shows safe multi-threaded increment code using Interlocked.
        /// </summary>
        [TestMethod]
        public void TestThreading_AddGoodInterlocked()
        {
            int starttime = Environment.TickCount;

            int output = 0;
            //Create the delegate to add 1 million
            ThreadStart add1Mill = () =>
            {
                for (int i = 0; i < 1000000; i++)
                    Interlocked.Increment(ref output);
            };
            //Create the two threads
            Thread t1 = new Thread(add1Mill);
            Thread t2 = new Thread(add1Mill);
            //Start the threads
            t1.Start();
            t2.Start();
            //OK, wait for the threads to complete.
            t1.Join();
            t2.Join();
            //Output the value.
            int finish = Environment.TickCount - starttime;
            Console.WriteLine("Final value is {0}", output);
        }
        #endregion
    }
}
