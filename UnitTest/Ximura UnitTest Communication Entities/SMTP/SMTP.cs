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
    /// Summary description for SMTP
    /// </summary>
    [TestClass]
    public class SMTP
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public SMTP()
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
        public void SMTPMessages()
        {
            //
            // TODO: Add test logic	here
            //
        }
    }
}
