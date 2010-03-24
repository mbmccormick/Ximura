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

using Ximura.Collections;
#endregion // using
namespace Ximura.UnitTest
{
    /// <summary>
    /// Summary description for TestLinq
    /// </summary>
    [TestClass]
    public partial class Test_Helper_Linq
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public Test_Helper_Linq()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #endregion // The default constructor.
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
        #endregion // TestContext
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

        Func<IEnumerable<int>, ICollection<int>, int, int> fnAddSome = 
            (list, coll, limit) => { list.Take(limit).ForEach(i => coll.Add(i)); return coll.Sum(); };


        public static IEnumerable<long> Fibonacci
        {
            get
            {
                long i = 1;
                long j = 1;
                while (true)
                {
                    yield return i;
                    long temp = i;
                    i = j;
                    j = j + temp;
                }
            }
        }

        //public static IEnumerable<T> InitializeInfinite<T>(Func<int, T> f)
        //{
        //    //return LinqHelper.Unfold<int, T>(s => Option.Some(Tuple.New(f(s), s + 1)), 0);
        //    return LinqHelper.Unfold<int, T>(s => Option.Some(Tuple.New(f(s), s + 1)), 0);
        //}

        //public static IEnumerable<T> InitializeFinite<T>(int count, Func<int, T> f)
        //{
        //    return LinqHelper.Unfold<int, T>(s =>
        //    {
        //        if (s < count)
        //            return Option.Some(Tuple.New(f(s), s + 1));
        //        else
        //            return Option<Tuple<T, int>>.None;
        //    }, 0);
        //}

        //public static IEnumerable<TResult> Generate<T, TResult>(Func<T> opener, Func<T, Option<TResult>> generator, Action<T> closer)
        //{
        //    T openerResult = opener();

        //    while (true)
        //    {
        //        var res = generator(openerResult);
        //        if (res.IsNone)
        //        {
        //            closer(openerResult);
        //            yield break;
        //        }

        //        yield return res.Value;
        //    }
        //}

        //[TestMethod]
        //public void TestFolding()
        //{
        //    var allCubes = InitializeInfinite(x => x * x);
        //    var tenCubes = InitializeFinite(10, x => x * x);

        //}

    }
}
