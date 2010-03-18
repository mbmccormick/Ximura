#region using
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ximura;
using RH = Ximura.Helper.Reflection;
using Ximura.Data;
using Ximura.Communication;
#endregion
namespace Ximura.UnitTest.Communication
{
    /// <summary>
    /// These tests validate the HTTP message objects.
    /// </summary>
    [TestClass]
    public class HTTP
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public HTTP()
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

        [TestMethod]
        public void HTTPMessages()
        {
            try
            {
                MessageLoad<HTTPRequestMessage> testHTTP_RQ1 =
                    new MessageLoad<HTTPRequestMessage>("Ximura.UnitTest.Communication.Examples.http_rq.txt, XimuraUTCommEntities");

                MessageLoad<HTTPRequestMessage> testHTTP_RQ2 =
                    new MessageLoad<HTTPRequestMessage>("Ximura.UnitTest.Communication.Examples.http2_rq.txt, XimuraUTCommEntities");

                MessageLoad<HTTPResponseMessage> testHTTP_RS1 =
                    new MessageLoad<HTTPResponseMessage>("Ximura.UnitTest.Communication.Examples.http_rs.txt, XimuraUTCommEntities");

                MessageLoad<HTTPResponseMessage> testHTTP_RS2 =
                    new MessageLoad<HTTPResponseMessage>("Ximura.UnitTest.Communication.Examples.http2_rs.txt, XimuraUTCommEntities");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
