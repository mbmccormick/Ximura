#region using
using System;
using System.Linq;
using System.IO;
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
    public partial class Test_Helper_Linq
    {
        [TestMethod]
        public void TestString()
        {
            try
            {
                MemoryStream ms = new MemoryStream();

                var hmm = new string[]{"hello","you big fat","hairy","hunt"};
                //Enumerable.Range(10, 50).StreamWrite(ms);
                hmm.StreamWrite(ms);

                ms.Position = 0;

                string[] result = ms.StreamRead<string>().ToArray();

                Assert.IsTrue(hmm.Compare(result));

                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
