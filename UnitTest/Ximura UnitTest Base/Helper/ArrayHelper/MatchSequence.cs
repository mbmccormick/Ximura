#region using
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ximura;
#endregion
namespace Ximura.UnitTest
{
    /// <summary>
    /// this set of tests matches against the 
    /// </summary>
    [TestClass]
    public class MatchSequence
    {
        #region TestMatchSimplePass()
        /// <summary>
        /// This method matches the data using a straight match. 
        /// </summary>
        [TestMethod]
        public void TestMatchSimplePass()
        {
            var match1 = "Polly put the kettle on"
                .MatchSequence("kettle".ToCharArray());

            Assert.IsTrue(match1.IsMatch);
            Assert.IsFalse(match1.IsPartialMatch);
        }
        #endregion  
        #region TestMatchSimpleFail()
        /// <summary>
        /// This method tries to matches the data using a straight match. 
        /// </summary>
        [TestMethod]
        public void TestMatchSimpleFail()
        {
            var match1 = "Polly put the ke2tle on"
                .MatchSequence("kettle", null);

            Assert.IsFalse(match1.IsMatch);
            Assert.IsFalse(match1.IsPartialMatch);
        }
        #endregion  

        #region TestMatch2Sequences()
        /// <summary>
        /// This method matches the 
        /// </summary>
        [TestMethod]
        public void TestMatch2Sequences()
        {
            var match1 = "Polly put the ket"
                .MatchSequence("kettle");

            Assert.IsFalse(match1.IsMatch);
            Assert.IsTrue(match1.IsPartialMatch);

            var match2 = "tle on"
                .MatchSequence("kettle", match1);

            Assert.IsTrue(match2.IsMatch);
            Assert.IsFalse(match2.IsPartialMatch);
        }
        #endregion  

        #region TestMatch2SequencesFail()
        /// <summary>
        /// This method matches the 
        /// </summary>
        [TestMethod]
        public void TestMatch2SequencesFail()
        {
            var match1 = "Polly put the ket"
                .MatchSequence("kettle");

            Assert.IsFalse(match1.IsMatch);
            Assert.IsTrue(match1.IsPartialMatch);

            var match2 = "2le on"
                .MatchSequence("kettle", match1);

            Assert.IsFalse(match2.IsMatch);
            Assert.IsTrue(match2.IsPartialMatch);
        }
        #endregion  
    }
}
