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
        [TestMethod()]
        public void FoldTest()
        {
            IEnumerable<int> items = new int[] { 234, 21, 34, 23423, 6, 8, 34, 34, 67, 8, 85, 5745, 978978, 2354346 };

            Func<long, int, long> f = (a, b) => a + b;

            long seed = 0;
            long expected = 3363023;
            long actual = LinqHelper.Fold<long, int>(items, f, seed);

            int hmm = items.Sum();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void FoldLeftTest()
        {
            IEnumerable<int> items = new int[] { 234, 21, 34, 23423, 6, 8, 34, 34, 67, 8, 85, 5745, 978978, 2354346 };

            Func<long, int, long> f = (a, b) => a + b;

            long seed = 0;
            long expected = 3363023;
            long actual = LinqHelper.FoldLeft<long, int>(items, f, seed);

            int sum = items.Sum();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void FoldRightTest()
        {
            IEnumerable<int> items = new int[] { 234, 21, 34, 23423, 6, 8, 34, 34, 67, 8, 85, 5745, 978978, 2354346 };

            Func<long, int, long> f = (a, b) => a + b;

            long seed = 0;
            long expected = 3363023;
            long actual = LinqHelper.FoldRight<long, int>(items, f, seed);

            int hmm = items.Sum();

            Assert.AreEqual(expected, actual);
        }
    }
}
