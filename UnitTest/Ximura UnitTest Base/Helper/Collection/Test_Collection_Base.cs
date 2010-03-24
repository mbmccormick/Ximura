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
        where T: ICollection<U>, new()
    {
        #region Enum -> CollectionAction
        /// <summary>
        /// This enumeration is used to run sets of tests from a stream.
        /// </summary>
        public enum CollectionAction : int
        {
            Add = 0,
            Remove = 1,
            Contains = 2,
        }
        #endregion // Enum -> CollectionAction

        #region Declarations
        protected int[] data1, data2, data3, data4, data5, data6, data7, data8;

        protected ICollection<U> mColl;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default class.
        /// </summary>
        public Test_Collection_Base()
        {
            InitializeData();
        }
        #endregion // Constructor

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

        #region MyTestInitialize()
        [TestInitialize()]
        public void MyTestInitialize()
        {

        }
        #endregion // MyTestInitialize()
        #region MyTestCleanup()
        [TestCleanup()]
        public void MyTestCleanup()
        {

        }
        #endregion // MyTestCleanup()

        #region InitializeData()
        /// <summary>
        /// This method initializes the data set.
        /// </summary>
        protected virtual void InitializeData()
        {
            data1 = CreateCollection(452636, 500000);
            data2 = CreateCollection(2342636, 500000);
            data3 = CreateCollection(6636, 500000);
            data4 = CreateCollection(4568, 500000);
            data5 = CreateCollection(98965, 500000);
            data6 = CreateCollection(1443564, 500000);
            data7 = CreateCollection(45, 500000);
            data8 = CreateCollection(654756, 500000);
        }
        #endregion // InitializeData()

        #region CreateCollection()
        /// <summary>
        /// This method initializes the collection.
        /// </summary>
        /// <returns></returns>
        protected virtual T CreateCollection()
        {
            return new T();
        }
        #endregion // CreateCollection()
        #region CreateData()
        /// <summary>
        /// This method returns the test data.
        /// </summary>
        /// <returns>Returns the data collection.</returns>
        protected abstract U[] CreateData();
        #endregion // TestData()


        int[] item1 = new int[] { 0, 42, 22, -64, 55, 42, 142, 2, -64, 2222, 41, int.MinValue, int.MaxValue };

        int[] item2 = new int[] { 
            int.MinValue, int.MaxValue
            , 42, 22
            , -64, 55
            , 42, 142
            , 2, 2222
            , 41, -122
            , -223, 56 };


        #region Test_DefaultTSupported(T coll)
        /// <summary>
        /// This is method checks the default(T) behaviour of the collection.
        /// </summary>
        /// <param name="coll">The collection.</param>
        /// <returns>Returns the change in the collection count.</returns>
        private int Test_DefaultTSupported(T coll)
        {
            int count = coll.Count;
            coll.Add(default(U));
            Assert.IsTrue(coll.Count == count+1);
            Assert.IsTrue(coll.Contains(default(U)));
            Assert.IsTrue(coll.Remove(default(U)));
            Assert.IsTrue(coll.Count == count);
            Assert.IsFalse(coll.Contains(default(U)));

            return 0;
        }
        #endregion

        #region Test_DefaultTNotSupported(T coll)
        /// <summary>
        /// This is method checks the default(T) behaviour of the collection.
        /// </summary>
        /// <param name="coll">The collection.</param>
        /// <returns>Returns the change in the collection count.</returns>
        private int Test_DefaultTNotSupported(T coll)
        {
            int count = coll.Count;
            coll.Add(default(U));
            Assert.IsTrue(coll.Count == count + 1);
            Assert.IsTrue(coll.Contains(default(U)));
            Assert.IsTrue(coll.Remove(default(U)));
            Assert.IsTrue(coll.Count == count);
            Assert.IsFalse(coll.Contains(default(U)));

            return 0;
        }
        #endregion

        #region Test_CopyTo(T coll)
        /// <summary>
        /// This method copies the data to a collection, and then checks that each item in the copied collection
        /// are contained in the collection.
        /// </summary>
        /// <param name="coll">The collection.</param>
        private void Test_CopyTo(T coll)
        {
            int count = coll.Count + 2;
            U[] data = new U[count];
            coll.CopyTo(data, 1);

            Assert.IsTrue(EqualityComparer<U>.Default.Equals(data[0], default(U)));

            for (int i = 1; i < count - 2; i++)
            {
                Assert.IsTrue(coll.Contains(data[i]));
            }

            Assert.IsTrue(EqualityComparer<U>.Default.Equals(data[count-1], default(U)));
        }
        #endregion // Test_CopyTo(T coll)

        #region Test_List(U[] item)
        /// <summary>
        /// This method runs a standard set of tests for the list based collection.
        /// </summary>
        /// <param name="item">The set of items to use in the test.</param>
        protected virtual void Test_List(U[] item)
        {
            T list = CreateCollection();

            list.Add(item[1]);//42
            list.Add(item[2]);//22
            list.Add(item[2]);//22
            list.Add(item[3]);//-64
            list.Add(item[4]);//55

            Assert.IsTrue(list.Count == 5);

            Test_DefaultTSupported(list);

            Assert.IsTrue(list.Contains(item[5]));//42
            Assert.IsFalse(list.Contains(item[6]));//142
            Assert.IsFalse(list.Contains(item[7]));//2
            Assert.IsTrue(list.Contains(item[8]));//-64

            list.Remove(item[2]);//22
            list.Remove(item[9]);//2222

            Assert.IsTrue(list.Contains(item[2]));
            Assert.IsTrue(list.Remove(item[2]));
            Assert.IsFalse(list.Remove(item[2]));
            Assert.IsFalse(list.Contains(item[2]));

            Assert.IsTrue(list.Count == 3);

            list.Add(item[1]);
            list.Add(item[10]);
            list.Add(item[0]);

            Assert.IsTrue(list.Count == 6);

            list.Add(item[0]);
            Assert.IsTrue(list.Count == 7);
            Assert.IsTrue(list.Remove(item[0]));

            Assert.IsTrue(list.Count == 6);

            Assert.IsTrue(list.Contains(item[10]));

            Assert.IsTrue(list.Remove(item[0]));
            Assert.IsFalse(list.Remove(item[0]));
            Assert.IsFalse(list.Remove(item[0]));

            Assert.IsTrue(list.Remove(item[5]));
            Assert.IsTrue(list.Remove(item[6]));

            //list.Add(-122);
            //list.Add(-223);
            //list.Add(int.MaxValue);
            //list.Add(int.MinValue);
            //Assert.IsTrue(list.Contains(int.MaxValue));
            //Assert.IsTrue(list.Contains(int.MinValue));

            //list.Add(56);

            Assert.IsTrue(list.Count == 8);

            Test_CopyTo(list);
        }
        #endregion // Test_List(U[] item)
        #region Test_HashSet(U[] item)
        /// <summary>
        /// This method runs a standard set of test for the hashset based collection.
        /// </summary>
        /// <param name="item">The set of items to use in the test.</param>
        protected virtual void Test_HashSet(U[] item)
        {
            T list = CreateCollection();

            list.Add(item[1]);//42
            list.Add(item[2]);//22
            list.Add(item[2]);//22
            list.Add(item[3]);//-64
            list.Add(item[4]);//55

            Assert.IsTrue(list.Count == 5);

            Test_DefaultTSupported(list);

            Assert.IsTrue(list.Contains(item[5]));//42
            Assert.IsFalse(list.Contains(item[6]));//142
            Assert.IsFalse(list.Contains(item[7]));//2
            Assert.IsTrue(list.Contains(item[8]));//-64

            list.Remove(item[2]);//22
            list.Remove(item[9]);//2222

            Assert.IsTrue(list.Contains(item[2]));
            Assert.IsTrue(list.Remove(item[2]));
            Assert.IsFalse(list.Remove(item[2]));
            Assert.IsFalse(list.Contains(item[2]));

            Assert.IsTrue(list.Count == 3);

            list.Add(item[1]);
            list.Add(item[10]);
            list.Add(item[0]);

            Assert.IsTrue(list.Count == 6);

            list.Add(item[0]);
            Assert.IsTrue(list.Count == 7);
            Assert.IsTrue(list.Remove(item[0]));

            Assert.IsTrue(list.Count == 6);

            Assert.IsTrue(list.Contains(item[10]));

            Assert.IsTrue(list.Remove(item[0]));
            Assert.IsFalse(list.Remove(item[0]));
            Assert.IsFalse(list.Remove(item[0]));

            Assert.IsTrue(list.Remove(item[5]));
            Assert.IsTrue(list.Remove(item[6]));

            //list.Add(-122);
            //list.Add(-223);
            //list.Add(int.MaxValue);
            //list.Add(int.MinValue);
            //Assert.IsTrue(list.Contains(int.MaxValue));
            //Assert.IsTrue(list.Contains(int.MinValue));

            //list.Add(56);

            Assert.IsTrue(list.Count == 8);

            Test_CopyTo(list);
        }
        #endregion // Test_HashSet(U[] item)
        #region Test_Dictionary(U[] item)
        /// <summary>
        /// This method runs a standard set of tests for a dictionary.
        /// </summary>
        /// <param name="item"></param>
        protected virtual void Test_Dictionary(U[] item)
        {

        }
        #endregion // Test_Dictionary(U[] item)
    }
}
