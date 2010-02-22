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
    [TestClass]
    public class Test_Helper_BitHelper
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public Test_Helper_BitHelper()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #endregion // The default constructor.
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

        #region Byte
        [TestMethod]
        public void Test_Bitmap_Byte()
        {
            byte test1 = 0;

            test1 = test1.BitSet(7);

            test1 = test1.BitSet(6);
            Assert.IsTrue(test1.BitCheck(6));

            test1 = test1.BitUnset(6);
            Assert.IsFalse(test1.BitCheck(6));
        }
        #endregion

        #region Int16
        [TestMethod]
        public void Test_Bitmap_Int16()
        {
            short test1 = 0;

            test1 = test1.BitSet(10);

            test1 = test1.BitSet(11);
            Assert.IsTrue(test1.BitCheck(10));

            test1 = test1.BitUnset(10);
            Assert.IsFalse(test1.BitCheck(10));
        }
        #endregion
        #region UInt16
        [TestMethod]
        public void Test_Bitmap_UInt16()
        {
            ushort test1 = 0;

            test1 = test1.BitSet(10);

            test1 = test1.BitSet(11);
            Assert.IsTrue(test1.BitCheck(10));

            test1 = test1.BitUnset(10);
            Assert.IsFalse(test1.BitCheck(10));
        }
        #endregion

        #region Int32
        [TestMethod]
        public void Test_Bitmap_Int32()
        {
            int test1 = 0;

            test1 = test1.BitSet(2);

            test1 = test1.BitSet(1);
            Assert.IsTrue(test1.BitCheck(2));

            test1 = test1.BitUnset(2);
            Assert.IsFalse(test1.BitCheck(2));
        }
        #endregion

        #region Int64
        [TestMethod]
        public void Test_Bitmap_Int64()
        {
            long test1 = 0;

            test1 = test1.BitSet(29);

            test1 = test1.BitSet(28);
            Assert.IsTrue(test1.BitCheck(29));

            test1 = test1.BitUnset(29);
            Assert.IsFalse(test1.BitCheck(29));
        }
        #endregion
    }
}
