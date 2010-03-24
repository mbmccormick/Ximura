#region using
using System;
using System.Linq;
using System.Text;
using System.IO;
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
    public partial class Test_Helper_Linq
    {
        [TestMethod()]
        public void Test_Unfold()
        {
            Stream ms = new MemoryStream();

            int[] coll = Enumerable.Range(0, 1000).ToArray();

            coll.StreamWrite(ms);

            ms.Position = 0;

            Func<Stream, Tuple<int, Stream>?> fredo = (str) =>
            {
                if (!(str.CanRead && (!str.CanSeek || (str.Position < str.Length)))) return null;
                return new Tuple<int, Stream>(StreamHelper.ReadInt32(str), str);
            };

            int[] results = ms.Unfold(fredo).ToArray();

            Assert.IsTrue(coll.Compare(results));

        }
    }
}
