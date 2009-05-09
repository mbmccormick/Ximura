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
    public partial class Test_Helper_Linq
    {
        [TestMethod]
        public void TestCurry()
        {
            List<int> list = new List<int>();

            var AddAndSumList = fnAddSome.Curry()(Enumerable.Range(25, 40)).Curry()(list);

            int result = AddAndSumList(5);

            Assert.AreEqual<int>(135, result);
        }
    }
}
