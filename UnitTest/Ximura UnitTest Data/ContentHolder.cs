
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

        #region ContentHolder_Test1
        /// <summary>
        /// This method tests the basic constructs for the generic content holder class.
        /// </summary>
        [TestMethod]
        public void ContentHolder_Test1()
        {
            string strData = "A bit fat hairy bloke.";
            ContentHolder<string> stringHolder = new ContentHolder<string>(strData);
            ContentHolder<string> stringHolderds = (ContentHolder<string>)stringHolder.Clone();

            byte[] blob1 = new byte[1000];
            ContentHolder<byte[]> byteHolder1 = new ContentHolder<byte[]>(blob1);
            ContentHolder<byte[]> byteHolder1ds = (ContentHolder<byte[]>)byteHolder1.Clone();

            byte[] blob2 = new byte[1000];
            blob2[0] = 3;
            ContentHolder<byte[]> byteHolder2 = new ContentHolder<byte[]>(blob2);
            ContentHolder<byte[]> byteHolder2ds = (ContentHolder<byte[]>)byteHolder2.Clone();

            StructTest t1 = new StructTest() { Data = "78", Count = 8 };
            ContentHolder<StructTest> byteHolder3 = new ContentHolder<StructTest>(t1);
            ContentHolder<StructTest> byteHolder3ds = (ContentHolder<StructTest>)byteHolder3.Clone();

            ClassTest t2 = new ClassTest() { Data = "78", Count = 8 };
            ContentHolder<ClassTest> byteHolder4 = new ContentHolder<ClassTest>(t2);
            ContentHolder<ClassTest> byteHolder4ds = (ContentHolder<ClassTest>)byteHolder4.Clone();

        } 
        #endregion
    }
}
