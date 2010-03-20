#region using
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ximura;
using RH = Ximura.Helper.Reflection;
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

        #region HTTPRequestMessages()
        /// <summary>
        /// This test validate the standard HTTP request message parsing.
        /// </summary>
        [TestMethod]
        public void HTTPRequestMessages()
        {
            try
            {
                MessageLoad<HTTPRequestMessage> testHTTP_RQ1 =
                    new MessageLoad<HTTPRequestMessage>("Ximura.UnitTest.Communication.HTTP.Examples.http1_rq.txt, XimuraUTCommEntities");

                MessageLoad<HTTPRequestMessage> testHTTP_RQ2 =
                    new MessageLoad<HTTPRequestMessage>("Ximura.UnitTest.Communication.HTTP.Examples.http2_rq.txt, XimuraUTCommEntities");

                MessageLoad<HTTPRequestMessage> testHTTP_RQ3 =
                    new MessageLoad<HTTPRequestMessage>("Ximura.UnitTest.Communication.HTTP.Examples.http3_rq.txt, XimuraUTCommEntities");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion 
        #region HTTPResponseMessages()
        /// <summary>
        /// This test validate the standard HTTP response message parsing.
        /// </summary>
        [TestMethod]
        public void HTTPResponseMessages()
        {
            try
            {
                MessageLoad<HTTPResponseMessage> testHTTP_RS1 =
                    new MessageLoad<HTTPResponseMessage>("Ximura.UnitTest.Communication.HTTP.Examples.http1_rs.txt, XimuraUTCommMessaging");

                MessageLoad<HTTPResponseMessage> testHTTP_RS2 =
                    new MessageLoad<HTTPResponseMessage>("Ximura.UnitTest.Communication.HTTP.Examples.http2_rs.txt, XimuraUTCommMessaging");

                MessageLoad<HTTPResponseMessage> testHTTP_RS3 =
                    new MessageLoad<HTTPResponseMessage>("Ximura.UnitTest.Communication.HTTP.Examples.http3_rs.txt, XimuraUTCommMessaging");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion 

        #region HTTPMultipartFormDataRequestParse()
        /// <summary>
        /// This test validates the Multipart/FormData mime parsing capabilities of the HTTPRequestMessage.
        /// </summary>
        [TestMethod]
        public void HTTPMultipartFormDataRequestParse()
        {
            try
            {
                MessageLoad<HTTPRequestMessage> testHTTP_RQ4 =
                    new MessageLoad<HTTPRequestMessage>("Ximura.UnitTest.Communication.HTTP.Examples.httpMime1.txt, XimuraUTCommMessaging");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion 


    }
}
