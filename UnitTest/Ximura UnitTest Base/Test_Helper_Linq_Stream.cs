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
        #region String
        [TestMethod]
        public void Test_Stream_String()
        {
            try
            {
                TestStream<string>(() => new string[] { "hello", "you big fat", "hairy", "hunt", "在下午八時" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // String

        #region Char
        [TestMethod]
        public void Test_Stream_Char()
        {
            try
            {
                TestStream<char>(() => "A ridge of high pressure is bringing generally fine weather to southeastern China. Meanwhile, a broad cloud band is covering southwestern China. 在 下 午 八 時 ， 熱 帶 低 氣 壓 燦 鴻 集 結 在 馬 尼 拉 之 東 北 偏 東 約 870 公 里 ， 預 料 向 東 北 或 東 北 偏 東 移 動 ， 時 速 約 10 公 里 ， 橫 過 呂 宋 以 東 海 域 ".ToCharArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // String

        #region Bool
        [TestMethod]
        public void Test_Stream_Bool()
        {
            try
            {
                TestStream<bool>(() => Enumerable.Range(0, 100)
                    .Convert<int, bool>(i => i % 2 == 1));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Byte
        [TestMethod]
        public void Test_Stream_Byte()
        {
            try
            {
                TestStream<byte>(() => Enumerable.Range(0, 100)
                    .Convert<int, byte>(i => (byte)i)
                    .InsertAtStart(byte.MinValue)
                    .InsertAtEnd(byte.MaxValue));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region SByte
        [TestMethod]
        public void Test_Stream_SByte()
        {
            try
            {
                TestStream<sbyte>(() => Enumerable.Range(0, 100)
                    .Convert<int, sbyte>(i => (sbyte)i)
                    .InsertAtStart(sbyte.MinValue)
                    .InsertAtEnd(sbyte.MaxValue));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Int16
        [TestMethod]
        public void Test_Stream_Int16()
        {
            try
            {
                TestStream<short>(() => Enumerable.Range(0, 100)
                    .Convert<int, short>(i => (short)i)
                    .InsertAtStart(short.MinValue)
                    .InsertAtEnd(short.MaxValue));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region UInt16
        [TestMethod]
        public void Test_Stream_UInt16()
        {
            try
            {
                TestStream<ushort>(() => Enumerable.Range(0, 100)
                    .Convert<int, ushort>(i => (ushort)i)
                    .InsertAtStart(ushort.MinValue)
                    .InsertAtEnd(ushort.MaxValue));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Int32
        [TestMethod]
        public void Test_Stream_Int32()
        {
            try
            {
                TestStream<int>(() => Enumerable.Range(0, 100)
                    .InsertAtStart(int.MinValue)
                    .InsertAtEnd(int.MaxValue));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region UInt32
        [TestMethod]
        public void Test_Stream_UInt32()
        {
            try
            {
                TestStream<uint>(() => Enumerable.Range(0, 100)
                    .Convert<int, uint>(i => (uint)i)
                    .InsertAtStart(uint.MinValue)
                    .InsertAtEnd(uint.MaxValue));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // UInt32

        #region Int64
        [TestMethod]
        public void Test_Stream_Int64()
        {
            try
            {
                TestStream<long>(
                    ()=>Enumerable.Range(0, 100)
                        .Convert<int,long>(i => (long)i)
                        .InsertAtStart(long.MinValue)
                        .InsertAtEnd(long.MaxValue)
                        );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // Int64
        #region UInt64
        [TestMethod]
        public void Test_Stream_UInt64()
        {
            try
            {
                TestStream<ulong>(
                    () => Enumerable.Range(0, 100)
                        .Convert<int, ulong>(i => (ulong)i)
                        .InsertAtStart(ulong.MinValue)
                        .InsertAtEnd(ulong.MaxValue)
                        );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion // Int64

        #region Double
        [TestMethod]
        public void Test_Stream_Double()
        {
            try
            {
                TestStream<double>(
                    () => Enumerable.Range(0, 100)
                        .Convert<int, double>(i => (double)i)
                        .InsertAtStart(double.MinValue)
                        .InsertAtStart(13.474373D)
                        .InsertAtStart(-9641631.474373D)
                        .InsertAtEnd(double.MaxValue)
                        );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region Float
        [TestMethod]
        public void Test_Stream_Float()
        {
            try
            {
                TestStream<float>(
                    () => Enumerable.Range(0, 100)
                        .Convert<int, float>(i => (float)i)
                        .InsertAtStart(float.MinValue)
                        .InsertAtStart(13.474373F)
                        .InsertAtStart(-9641631.474373F)
                        .InsertAtEnd(float.MaxValue)
                        );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region Decimal
        [TestMethod]
        public void Test_Stream_Decimal()
        {
            try
            {
                TestStream<decimal>(
                    () => Enumerable.Range(0, 100)
                        .Convert<int, decimal>(i => (decimal)i)
                        .InsertAtStart(decimal.MinValue)
                        .InsertAtStart(13.474373m)
                        .InsertAtStart(-9641631.474373m)
                        .InsertAtEnd(decimal.MaxValue)
                        );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region TestStream<T>(Func<IEnumerable<T>> fnCreate)
        /// <summary>
        /// This function writes and then reads a array of items to a stream, and then compares that the two arrays are equal.
        /// </summary>
        /// <typeparam name="T">The type parameter for the test.</typeparam>
        /// <param name="fnCreate">This function creates the array.</param>
        private void TestStream<T>(Func<IEnumerable<T>> fnCreate)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                T[] coll = fnCreate().ToArray();

                coll.StreamWrite(ms);

                ms.Position = 0;

                T[] result = ms.StreamRead<T>().ToArray();

                Assert.IsTrue(coll.Compare(result));
            }
        }
        #endregion // TestStream<T>(Func<IEnumerable<T>> fnCreate)

        #region Tuple
        [TestMethod]
        public void Test_Stream_Tuple()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Tuple<int, int>[] coll = LinqHelper.RangeTuple(0, 500, 216).ToArray();

                    coll.StreamWrite(ms,
                        (stm, tuple) =>
                        {
                            StreamHelper.Write(stm, tuple.Item1);
                            StreamHelper.Write(stm, tuple.Item2);
                        });

                    ms.Position = 0;

                    Tuple<int, int>[] result = ms.StreamRead<Tuple<int, int>>(
                        (stm) =>
                        {
                            int item1 = StreamHelper.ReadInt32(stm);
                            int item2 = StreamHelper.ReadInt32(stm);

                            return new Tuple<int, int>(item1, item2);
                        }).ToArray();

                    Assert.IsTrue(coll.Compare(result));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion    
    }
}
