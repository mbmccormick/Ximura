#region using
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
#endregion 
namespace Ximura.UnitTest.Data
{
    /// <summary>
    /// Summary description for ContentHelper
    /// </summary>
    [TestClass]
    public class ContentHolder
    {
        #region Constructor
        public ContentHolder()
        {
            //
            // TODO: Add constructor logic here
            //
        } 
        #endregion

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
        #endregion

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

        #region Struct -> StructTest
        [Serializable]
        private struct StructTest
        {
            public string Data;
            public int Count;
            public Guid? ID;
        } 
        #endregion

        #region Class -> ClassTest
        [DataContract]
        public class ClassTest
        {
            [DataMember]
            public string Data;
            [DataMember]
            public int Count;
            [DataMember]
            public Guid? ID;
        }     
        #endregion

        [TestMethod]
        public void ContentHolder1()
        {
            string strData = "A bit fat hairy bloke.";
            ContentHolder<string> stringHolder = new ContentHolder<string>(strData);

            byte[] blob1 = new byte[1000];
            ContentHolder<byte[]> byteHolder1 = new ContentHolder<byte[]>(blob1);

            byte[] blob2 = new byte[1000];
            blob2[0] = 3;
            ContentHolder<byte[]> byteHolder2 = new ContentHolder<byte[]>(blob2);

            StructTest t1 = new StructTest() { Data = "78", Count = 8 };
            ContentHolder<StructTest> byteHolder3 = new ContentHolder<StructTest>(t1);

            ClassTest t2 = new ClassTest() { Data = "78", Count = 8 };
            ContentHolder<ClassTest> byteHolder4 = new ContentHolder<ClassTest>(t2);

        }
    }
}
