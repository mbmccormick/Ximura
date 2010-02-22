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
        public void TestCurry1()
        {
            Action<int, int> fnOutput = (total, start) => Enumerable.Range(start, total).ForEach(i => Console.WriteLine(i));

            fnOutput(40, 20);
            fnOutput(40, 45);

            Action<int> fnOutput40 = fnOutput.Curry()(40);

            fnOutput40(20);
            fnOutput40(45);
        }

        [TestMethod]
        public void TestCurry2()
        {
            List<int> list = new List<int>();

            var AddAndSumList = fnAddSome.Curry()(Enumerable.Range(25, 40)).Curry()(list);

            int result = AddAndSumList(5);

            Assert.AreEqual<int>(135, result);
        }
    }
}
