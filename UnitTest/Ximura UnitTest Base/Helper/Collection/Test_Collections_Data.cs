#region using
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ximura;
using Ximura.Collections;
using Ximura.Collections.Data;
#endregion // using
namespace Ximura.UnitTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class Test_Collections_Data
    {
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

        [TestMethod]
        public void VertexEdgeChecks()
        {
            Vertex v1 = new Vertex(24);
            Vertex v2 = new Vertex(25);
            Vertex v3 = new Vertex(3783825);

            Edge e = new Edge(v1, v2);
            Edge e2 = v1 + v2;
            Edge e3 = v2 + v1;

            Assert.IsTrue(e == e2, "Edge addition check");
            Assert.IsFalse(e2 == e3, "Edge inequality check");

            Vertex vMax = new Vertex(uint.MaxValue);

            Edge eMaxf = v2 + vMax;
            Edge eMaxb = vMax + v2;

            Edge eRev = eMaxf.Reverse;
            Edge eRev2 = !eMaxf;

            Assert.IsTrue(eRev == eMaxb);
            Assert.IsFalse(eRev != eMaxb);

            Assert.IsTrue(e.Contains(v1));
            Assert.IsFalse(e.Contains(v3));

            // K < 23 <[0]< 24 <[1]< 25 <[2]< 26 <S
            Edge et0 = new Edge(23, 24);
            Edge et1 = new Edge(24, 25);
            Edge et2 = new Edge(25, 26);

            Edge et3 = new Edge(25, 24);

            Assert.IsTrue(et0 < et1);
            Assert.IsTrue(et1 < et2);
            Assert.IsTrue(et2 > et1);
            Assert.IsFalse(et1 < et3);
        }

        [TestMethod]
        public void GraphChecks()
        {
            VertexReference ref1 = new VertexReference("A", "B", Guid.NewGuid().ToByteArray());
            VertexReference ref2 = new VertexReference("A", "B", Guid.NewGuid().ToByteArray());
            VertexReference ref3 = ref2;

            Assert.IsFalse(ref1 == ref2);
            Assert.IsTrue(ref2 == ref3);

        }

        [TestMethod]
        public void ExpressionTests()
        {
            Expression<Func<string, int>> length = s => s.Length - s.ToCharArray().Max(c => c.CompareTo((char)53));
            string myString = "some string";
            Func<string, int> lengthMethod = length.Compile();
            int stringLength = lengthMethod(myString); 

            var rule = new Rule<User>();
            rule.BindRuleTo(u => u.Username);  
        }

        public class User
        {
            public string Username { get; set; }
        }

        public class Rule<T>
        {
            public Rule<T> BindRuleTo<U>(Expression<Func<T, U>> expression)
            {
                //do something with the expression here   
                return this;
            }
        }  

    }
}
