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
    /// <summary>
    /// Summary description for Test_Misc
    /// </summary>
    [TestClass]
    public class Test_Misc
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

        [TestMethod]
        public void TestLogCount1()
        {
            int start = Environment.TickCount;
            for (int i = 10000000; i > 0; i--)
            {
                int newLevel = (int)(Math.Log(i) / 0.693147181);
            }

            int end = Environment.TickCount - start;
        }

        [TestMethod]
        public void TestLogCount2()
        {
            int level, remainder;
            int start = Environment.TickCount;

            for (int i = 10000000; i > 0; i--)
            {
                BitHelper.SplitOnMostSignificantBit(i, 24, out level, out remainder);
            }

            int end = Environment.TickCount - start;
        }

        [TestMethod]
        public void TestLogCount3()
        {
            int mSlotInitialCapacity = 1000;
            int mSlotsLevelOffset = BitHelper.FindMostSignificantBit(mSlotInitialCapacity, 31);

            int level, levelPosition;
            SlotsCalculateLevelPosition(mSlotInitialCapacity, mSlotsLevelOffset
                , 1, out level, out levelPosition);
            Assert.AreEqual(level, 0);

            SlotsCalculateLevelPosition(mSlotInitialCapacity, mSlotsLevelOffset
                , 65512, out level, out levelPosition);
            Assert.AreEqual(level, 7);

            SlotsCalculateLevelPosition(mSlotInitialCapacity, mSlotsLevelOffset
                , 2097127, out level, out levelPosition);
            Assert.AreEqual(level, 11);

            SlotsCalculateLevelPosition(mSlotInitialCapacity, mSlotsLevelOffset
                , 134217703, out level, out levelPosition);
            Assert.AreEqual(level, 14);

            SlotsCalculateLevelPosition(mSlotInitialCapacity, mSlotsLevelOffset
                , int.MaxValue - 21, out level, out levelPosition);
            Assert.AreEqual(level, 22);

        }

        #region SlotsCalculateLevelPosition(int indexID, out int level, out int levelPosition)
        /// <summary>
        /// This method calculates the specific bucket level and the position within that bucket.
        /// </summary>
        /// <param name="index">The slot index.</param>
        /// <param name="level">The slot level.</param>
        /// <param name="levelPosition">The slot level position.</param>
        protected virtual void SlotsCalculateLevelPosition(
            int mSlotInitialCapacity, int mSlotsLevelOffset,int initialIndex, 
            out int level, out int levelPosition)
        {
            if (initialIndex < mSlotInitialCapacity)
            {
                level = 0;
                levelPosition = initialIndex;
                return;
            }

            //Base line the binary progression by removing the initial capacity.
            int index = initialIndex - mSlotInitialCapacity;

            //Check bottom bounds, are we within the first bit block.
            if (index < (1 << mSlotsLevelOffset))
            {
                level = 1;
                levelPosition = index;
                return;
            }
            initialIndex = -(1 << mSlotsLevelOffset);

            //OK, are we within the final block?
            if (index >= (1 << 30))
            {
                level = 30 - mSlotsLevelOffset;
                levelPosition = index - (1 << 30);
                return;
            }

            //Ok, find the most significant bit for the number, starting at the mSlotsLevelCurrent+1
            //BitHelper.SplitOnMostSignificantBit(
            //    index, mSlotsLevelCurrent + 1, out level, out levelPosition);
            BitHelper.SplitOnMostSignificantBit(index, 31, out level, out levelPosition);
            level -= mSlotsLevelOffset;
        }
        #endregion
    }
}
