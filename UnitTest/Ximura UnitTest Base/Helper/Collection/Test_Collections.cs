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
    /// <summary>
    /// Summary description for TestCollections
    /// </summary>
    [TestClass]
    public class Test_Collections
    {
        #region Constructor
        public Test_Collections()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        #endregion // Constructor
        #region TestContext
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        #endregion // TestContext

        #region TestCollectionState()
        [TestMethod]
        public void TestCollectionState()
        {
            //LockFreeCollectionState state = new LockFreeCollectionState();

            //Assert.AreEqual(Version, 0);
            //Assert.AreEqual(state.Count, 0);

            //VersionChange();

            //Assert.IsTrue(VersionCheck(1));

            //int comp = 0;

            //ThreadStart a = () => { for (int count = 0; count < 1000000; count++) { comp++; state.VersionChange(); } };

            //ThreadTest.TwoThreadCheck(a, a);

            //Assert.IsTrue(VersionCheck(2000001));
            //Assert.IsFalse(VersionCheck(45));
            //Assert.IsTrue(VersionCheck(2000001));

            //ThreadTest.TwoThreadCheck(
            //    () => { for (int count = 0; count < 1000001; count++) { comp++; state.CountInc(); } },
            //    () => { for (int count = 0; count < 1000000; count++) { comp++; state.CountDec(); } }
            //    );

            //Assert.AreEqual(state.Count, 1);
            //Assert.AreNotEqual(state.Count, 15);
            //Assert.AreEqual(state.Count, 1);


        }
        #endregion // TestCollectionState()



        [TestMethod]
        public void TestLockFreeList_Double()
        {
            ICollection<double> listTest = new ConcurrentList<double>();

            listTest.Add(7);
            listTest.Add(-64);
            listTest.Add(42);
            listTest.Add(22);
            listTest.Add(22);
            listTest.Add(55);

            Assert.IsTrue(listTest.Count == 5);

            Assert.IsTrue(listTest.Contains(42));
            Assert.IsFalse(listTest.Contains(142));
            Assert.IsFalse(listTest.Contains(2));
            Assert.IsTrue(listTest.Contains(-64));

            listTest.Remove(22);
            Assert.IsTrue(listTest.Contains(22));
            Assert.IsTrue(listTest.Remove(22));
            Assert.IsFalse(listTest.Contains(22));
            Assert.IsTrue(listTest.Count == 3);

            listTest.Add(42);
            listTest.Add(41);
            listTest.Add(0);

            Assert.IsTrue(listTest.Count == 6);

            listTest.Add(0);
            Assert.IsTrue(listTest.Count == 7);
            Assert.IsTrue(listTest.Remove(0));

            Assert.IsTrue(listTest.Count == 6);

            Assert.IsTrue(listTest.Contains(41));

            Assert.IsTrue(listTest.Remove(0));
            Assert.IsFalse(listTest.Remove(0));
            Assert.IsFalse(listTest.Remove(0));

            Assert.IsTrue(listTest.Remove(42));
            Assert.IsTrue(listTest.Remove(55));

            listTest.Add(-122);
            listTest.Add(-223.5345);
            listTest.Add(double.MaxValue);
            listTest.Add(double.MinValue);
            Assert.IsTrue(listTest.Contains(double.MaxValue));
            Assert.IsTrue(listTest.Contains(double.MinValue));
            Assert.IsTrue(listTest.Contains(-223.5345));
            listTest.Add(56);

            Assert.IsTrue(listTest.Count == 8);

        }

        [TestMethod]
        public void TestLockFreeList_Int()
        {
            ICollection<int> listTest = new ConcurrentList<int>(1);

            listTest.Add(42);
            listTest.Add(22);
            listTest.Add(22);
            listTest.Add(-64);
            listTest.Add(55);
            listTest.Add(0);

            Assert.IsTrue(listTest.Count == 6);
            Assert.IsTrue(listTest.Contains(0));
            Assert.IsTrue(listTest.Remove(0));
            Assert.IsFalse(listTest.Contains(0));

            Assert.IsTrue(listTest.Contains(42));
            Assert.IsFalse(listTest.Contains(142));
            Assert.IsFalse(listTest.Contains(2));
            Assert.IsTrue(listTest.Contains(-64));

            listTest.Remove(22);
            listTest.Remove(2222);

            Assert.IsTrue(listTest.Contains(22));
            Assert.IsTrue(listTest.Remove(22));
            Assert.IsFalse(listTest.Remove(22));
            Assert.IsFalse(listTest.Contains(22));

            Assert.IsTrue(listTest.Count == 3);

            listTest.Add(42);
            listTest.Add(41);
            listTest.Add(0);

            Assert.IsTrue(listTest.Count == 6);

            listTest.Add(0);
            Assert.IsTrue(listTest.Count == 7);
            Assert.IsTrue(listTest.Remove(0));

            Assert.IsTrue(listTest.Count == 6);

            Assert.IsTrue(listTest.Contains(41));

            Assert.IsTrue(listTest.Remove(0));
            Assert.IsFalse(listTest.Remove(0));
            Assert.IsFalse(listTest.Remove(0));

            Assert.IsTrue(listTest.Remove(42));
            Assert.IsTrue(listTest.Remove(55));

            listTest.Add(-122);
            listTest.Add(-223);
            listTest.Add(int.MaxValue);
            listTest.Add(int.MinValue);
            listTest.Add(56);

            Assert.IsTrue(listTest.Count == 8);

            int[] data = new int[10];
            listTest.CopyTo(data, 1);
        }

        [TestMethod]
        public void TestOfNoImportance()
        {
            int[] loop = new[] { 4354, 234, 23, 34, 3, 4 };
            var e = loop.GetEnumerator();
            for (e.Reset(); e.MoveNext(); )
            {
                Console.WriteLine(e.Current);
            }

            loop.ForEach(i => Console.WriteLine(i));

            int newBucketID1=0;
            int newBucketID2=0;

            int tBitsCurrent = 19;

            int ticks1 = Environment.TickCount;
            for(int i=0;i<40000000;i++)
                newBucketID1 += i % (1 << (tBitsCurrent));

            int ticks2 = Environment.TickCount;
            for (int i = 0; i < 40000000; i++)
                newBucketID2 += i & (int.MaxValue >> (31 - tBitsCurrent));
            int ticksEnd = Environment.TickCount;

            ticks1 = ticks2 - ticks1;
            ticks2 = ticksEnd - ticks2;
        }

        [TestMethod]
        public void TestLockFreeHashSet_Guid()
        {
            ICollection<Guid> listTest = new ConcurrentHashSet<Guid>();

            Enumerable.Range(0, 50000)
                .ForEach(e => listTest.Add(Guid.NewGuid()));

        }

        [TestMethod]
        public void TestLockFreeHashSet_Stream()
        {
            ICollection<Stream> listTest = new ConcurrentHashSet<Stream>();

            MemoryStream ms1 = new MemoryStream();
            MemoryStream ms2 = new MemoryStream();

            try
            {
                Enumerable.Range(0, 5000)
                    .Convert(i => new MemoryStream())
                    .InsertAtPosition(ms1, 455)
                    .ForEach(s => listTest.Add(s));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            Assert.IsTrue(listTest.Count == 5001);
            Assert.IsTrue(listTest.Contains(ms1));
            Assert.IsFalse(listTest.Contains(ms2));
        }

        [TestMethod]
        public void TestLockFreeHashSet_Long()
        {
            ConcurrentHashSet<long> listTest = new ConcurrentHashSet<long>();

            listTest.Add(42);
            listTest.Add(22);
            listTest.Add(22);
            listTest.Add(64);
            listTest.Add(55);

            Assert.IsTrue(listTest.Count == 4);

            Assert.IsTrue(listTest.Contains(42));
            Assert.IsFalse(listTest.Contains(142));
            Assert.IsFalse(listTest.Contains(2));
            Assert.IsTrue(listTest.Contains(64));

            listTest.Remove(42);
            Assert.IsFalse(listTest.Contains(42));
            Assert.IsFalse(listTest.Remove(42));
            Assert.IsTrue(listTest.Count == 3);

            listTest.Add(42);
            listTest.Add(41);
            listTest.Add(0);

            Assert.IsTrue(listTest.Count == 6);

            listTest.Add(0);
            Assert.IsTrue(listTest.Count == 6);


            Assert.IsTrue(listTest.Contains(41));
            Assert.IsTrue(listTest.Contains(0));

            Assert.IsTrue(listTest.Remove(0));

            Assert.IsFalse(listTest.Contains(0));


            Assert.IsTrue(listTest.Remove(22));
            Assert.IsTrue(listTest.Remove(55));

            listTest.Add(1212);
            listTest.Add(323);
            listTest.Add(7567);
            listTest.Add(567);

            Assert.IsTrue(listTest.Count == 7);

        }

        //[TestMethod]
        //public void TestLockFreeArray()
        //{
        //    Action<LockFreeArray<int>> collAdd = (coll) => Enumerable.Range(1, 10).ForEach(i => coll.Expand(i*10));
        //    Action<LockFreeArray<int>, int, int> collSet = (coll, a, b) => Enumerable.Range(a, b).Reverse().ForEach(i => coll[i]=i);

        //    LockFreeArray<int> testColl = new LockFreeArray<int>(50);

        //    int expand = ThreadTest.TestRun(new Action[] 
        //        { 
        //            () => { collAdd(testColl);},
        //            () => { collAdd(testColl);},
        //        });

        //    int set = ThreadTest.TestRun(new Action[] 
        //        { 
        //            () => { collSet(testColl, 0, 600);},
        //            () => { collSet(testColl, 600, 550);},
        //        });
        //}

        [TestMethod]
        public void TestLockFreeColl2()
        {
            Action<ICollection<int>, int, int> collAdd = (coll, a, b) => Enumerable.Range(a,b).Reverse().ForEach(i =>  coll.Add(i));

            ConcurrentHashSet<int> testColl = new ConcurrentHashSet<int>();

            var jobs = new Action[] {
             () => { collAdd(testColl, 0, 500000);},
             () => { collAdd(testColl, 500000, 500000);},
             () => { collAdd(testColl, 1000000, 500000);},
             () => { collAdd(testColl, 1500000, 500000);},
             () => { collAdd(testColl, 2000000, 500000);},
             () => { collAdd(testColl, 2500000, 500000);},
             () => { collAdd(testColl, 3000000, 500000);},
             () => { collAdd(testColl, 3500000, 500000);}
            };

            //var jobs = new Action[] {
            // () => { collAdd(testColl, 0, 1000000);},
            // () => { collAdd(testColl, 1000000, 1000000);},
            // () => { collAdd(testColl, 2000000, 1000000);},
            // () => { collAdd(testColl, 3000000, 1000000);}
            //};

            //var jobs = new Action[] {
            // () => { collAdd(testColl, 0, 2000000);},
            // () => { collAdd(testColl, 2000000, 2000000);}
            //};

            //var jobs = new Action[] {
            // () => { collAdd(testColl, 0, 4000000);}
            //};

            TimeSpan serial = jobs.Execute();
        }

        [TestMethod]
        public void TestWrapper()
        {
            //var result1 = TestFunctions.fnTest4M(new LockFreeList<int>(), 1);
            ConcurrentList<int> list = new ConcurrentList<int>();
            var result2 = TestFunctions.fnTest4MAdd(list, 4);

            var test4 = TestFunctions.CalcAndCompare<ConcurrentList<int>>();
            //var test1 = TestFunctions.CalcAndCompare<CourseGrainedICollectionWrapper<int>, HashSet<int>>();
            //var test2 = TestFunctions.CalcAndCompare<ReadWriteICollectionWrapper<int>, HashSet<int>>();
            var test44 = TestFunctions.CalcAndCompare<CollectionWrapperInterlocked<int>, ConcurrentList<int>>();
            var test3 = TestFunctions.CalcAndCompare<CollectionWrapperInterlocked<int>, HashSet<int>>();

            var test5a = TestFunctions.fnTest4MAdd(new HashSet<int>(), 1);
            var test5b = TestFunctions.fnTest4MAdd(new List<int>(40000005), 1);

            var test6a = TestFunctions.fnTest4MAdd(new ConcurrentList<int>(), 1);
            var test6b = TestFunctions.fnTest4MAdd(new ConcurrentList<int>(), 4);
            var test6bc = TestFunctions.fnTest4MAdd(new ConcurrentList<int>(40000005), 1);
        }

        [TestMethod]
        public void TestWrapper2()
        {
            int hel = unchecked((int)(0 | 0x80000000));
            hel++;
            int sum = 34256374;
            int hello = Test_Hmm(sum, 1);
            Console.WriteLine(hello);

            int result1 = BitReverse(3, 1);
            int result2 = BitReverse(3, 2);
            int result3 = BitReverse(3, 3);
            int result4 = BitReverse(3);
            int result5 = BitReverse(11);

        }

        private const int cnLoMask = 0x00000001;
        private const int cnHiMask = 0x00800000;

        private int BitReverse(int data, int bitSize)
        {
            int result = 0;
            int hiMask = cnHiMask;

            for (int i = 0; i < bitSize; i++)
            {
                if ((data & cnLoMask) > 0)
                    result |= hiMask;
                hiMask >>= 1;
                data >>= 1;
            }

            return result;
        }

        private int BitReverse(int data)
        {
            int result = 0;
            int hiMask = cnHiMask;

            for (; data > 0; data >>= 1)
            {
                if ((data & cnLoMask) > 0)
                    result |= hiMask;
                hiMask >>= 1;
            }

            return result;
        }

        private int Test_Hmm(int hashCode, int currentBits)
        {
            //OK, calculate the divisor, this is the number of bits that we are currently interested in for
            //the size of the collection.
            int divisor = 1 << (currentBits);
            //Ok, get the specific bucketID for the hashCode and the divisor.
            return hashCode % divisor;
        }
    }
}
