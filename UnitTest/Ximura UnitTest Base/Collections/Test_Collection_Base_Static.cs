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

using Ximura.Collections;
#endregion // using
namespace Ximura.UnitTest
{
    /// <summary>
    /// This abstract class is used for testing the collection classes.
    /// </summary>
    /// <typeparam name="T">The collection parameter type.</typeparam>
    public abstract partial class Test_Collection_Base<T,U>
    {

        #region CreateCollection(int seed, int count)
        /// <summary>
        /// This static method creates a random collection from 0 to count-1, where 
        /// each value exists only one time.
        /// </summary>
        /// <param name="seed">The seed value.</param>
        /// <param name="count">The collection count.</param>
        /// <returns>Returns a collection of integers.</returns>
        public static int[] CreateCollection(int seed, int count)
        {
            int[] items = null;
            int attempts = 0;

            do
            {
                attempts++;
                if (attempts > 10)
                    throw new Exception();

                Random rnd = new Random(seed);

                List<int> coll = new List<int>();

                for (int i = 0; i < (attempts + 14) * count; i++)
                    coll.Add(rnd.Next(0, count));

                items = coll.Distinct().ToArray();
            }
            while (items.Length < count);

            return items;
        }
        #endregion // CreateCollection(int seed, int count)


        public static byte[] CreateByteArray(int[] items)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                items.ForEach(i => StreamHelper.Write(ms, i));

                ms.Flush();
                return ms.ToArray();
            }
        }

    }
}
