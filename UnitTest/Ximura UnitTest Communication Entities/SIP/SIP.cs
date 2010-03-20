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
    /// SIP messaging tests
    /// </summary>
    [TestClass]
    public class SIP
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public SIP()
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
        public void SIPMessages()
        {
            //
            // TODO: Add test logic	here
            //
        }
    }
}
