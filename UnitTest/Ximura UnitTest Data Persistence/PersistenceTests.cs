#region using
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ximura;
using Ximura.Data;
using Ximura.Framework;
#endregion 
namespace Ximura.UnitTest.Data
{
    /// <summary>
    /// These test validate the persistence functionality.
    /// </summary>
    [TestClass]
    public class PersistenceTests
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public PersistenceTests()
        {
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

        #region CDSInitialize()
        /// <summary>
        /// This helper class creates the CDS container.
        /// </summary>
        /// <returns>Returns the container.</returns>
        private ContentDataStoreContainer CDSInitialize()
        {
            return new ContentDataStoreContainer(
                new KeyValuePair<string, ICDSState>[] 
                { 
                    new KeyValuePair<string,ICDSState>("BinaryTest",new BinaryTestPersistenceAgent())
                });
        }
        #endregion 

        #region CDS_BinaryTest_Construct
        /// <summary>
        /// This is a generic test to validate that the persistence agent can be added to the container,
        /// and that the simple create action can be performed.
        /// </summary>
        [TestMethod]
        public void CDS_BinaryTest_Construct()
        {
            ContentDataStoreContainer cont = CDSInitialize();        

            try
            {
                cont.Start();

                BinaryTest bt;
                CDSResponse res = cont.CDSConstruct<BinaryTest>(out bt);

                Assert.IsTrue(res == CDSResponse.OK);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cont.Stop();
            }
        }
        #endregion 

        #region CDS_BinaryTest_CreateRead
        /// <summary>
        /// This is a generic test to validate that the persistence agent can be added to the container,
        /// and that the simple create action can be performed.
        /// </summary>
        [TestMethod]
        public void CDS_BinaryTest_CreateRead()
        {
            ContentDataStoreContainer cont = CDSInitialize();

            try
            {
                cont.Start();

                BinaryTest bt = new BinaryTest();
                //bt.Body
                CDSResponse res = cont.CDSCreate(bt);

                Assert.IsTrue(res == CDSResponse.OK, "Create failed.");

                BinaryTest bt2;
                CDSResponse res2 = cont.CDSRead<BinaryTest>(bt.IDContent, null, out bt2);
                Assert.IsTrue(res2 == CDSResponse.OK, "Read failed.");


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cont.Stop();
            }
        }
        #endregion 

    }
}
