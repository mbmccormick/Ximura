#region using
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

using Ximura;
using Ximura.Helper;
using Ximura.Collections;
#endregion // using
namespace Ximura.UnitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int startRaw = Environment.TickCount;
            HashSet<int> testRaw = new HashSet<int>();
            for (int i = 0; i < 4000000; i++)
                testRaw.Add(i);
            int timeRaw = Environment.TickCount - startRaw;
            for (int i = 0; i < 4000000; i++)
                testRaw.Contains(i);
            int timeRaw2 = Environment.TickCount - startRaw;

            Console.WriteLine(timeRaw.ToString());
            Console.WriteLine(timeRaw2.ToString());



            //Action<int, int> fnOutput = (total, start) =>
            //    Enumerable.Range(start, total).ForEach(i => Console.WriteLine(i));

            //for (int i = 0; i < 40; i++)
            //    Console.WriteLine(i);

            ////fnOutput(40, 0);

            //Action<int> fnOutput40 = fnOutput.Curry()(40);

            //fnOutput40(0);
            //fnOutput40(30);

            //ICollection<int> coll = new HashSet<int>(); 
            //int[] data = { 42, 46, 77, 345, -12 }; 
            //foreach (var local in data)
            //    coll.Add(local);

            //int test5 = TestFunctions.fnThunk(new HashSet<int>(), 1);
            //Console.Write("None\t");
            //Console.WriteLine(test5);

            //var test6 = TestFunctions.CalcP<LockFreeList<int>>();
            //Console.Write("LFLS\t");
            //Console.WriteLine(test6);

            //var test1 = TestFunctions.CalcP<CourseGrainedICollectionWrapper<int>, HashSet<int>>();
            //Console.Write("Lock\t");
            //Console.WriteLine(test1);
            //var test2 = TestFunctions.CalcP<ReadWriteICollectionWrapper<int>, HashSet<int>>();
            //Console.Write("RWLS\t");
            //Console.WriteLine(test2);
            //var test3 = TestFunctions.CalcP<InterlockedICollectionWrapper<int>, HashSet<int>>();
            //Console.Write("Intr\t");
            //Console.WriteLine(test3);

            CollectionWrapperCourseGrained<int> wrp = new CollectionWrapperCourseGrained<int>();
            wrp.Collection = new HashSet<int>();
            var resultn = TestFunctions.fnTest4MAdd(wrp, 4);
            Console.WriteLine(resultn.ToString());

            var resultnc = TestFunctions.fnTest4MContains(wrp, 4);
            Console.WriteLine(resultnc.ToString());

            var resultnr = TestFunctions.fnTest4MRemove(wrp, 4);
            Console.WriteLine(resultnr.ToString());

            int start = Environment.TickCount;

            //LockFreeList<int> list = new LockFreeList<int>();
            //ConcurrentHashSet<int> list = new ConcurrentHashSet<int>(4100000,true);
            ConcurrentHashSet<int> list = new ConcurrentHashSet<int>(4100000, true);

            var result2 = TestFunctions.fnTest4MAdd(list, 4);
            Console.Write("INSERTS = ");
            Console.WriteLine(result2.ToString());

            Console.Write("TICKS = ");
            Console.WriteLine(Environment.TickCount - start);

#if (DEBUG)
            Console.WriteLine(list.DebugDump);
            list.DebugReset();

            //list.DebugDataValidate();

            bool success = list.Contains(1000000);
#endif
            start = Environment.TickCount;

            var resultnc2 = TestFunctions.fnTest4MContains(list, 4);
            Console.Write("CONTAIN = ");
            Console.WriteLine(resultnc2.ToString());

            Console.Write("TICKS = ");
            Console.WriteLine(Environment.TickCount - start);

#if (DEBUG)
            Console.WriteLine(list.DebugDump);
#endif
            //start = Environment.TickCount;

            //var resultnr2 = TestFunctions.fnTest4MRemove(list, 4);
            //Console.Write("REMOVE = ");
            //Console.WriteLine(resultnr2.ToString());

            //Console.Write("TICKS = ");
            //Console.WriteLine(Environment.TickCount - start);

            //start = Environment.TickCount;

            list.Clear();
            Console.Write("CLEAR = ");
            Console.WriteLine(Environment.TickCount - start);

#if (DEBUG)
            //Console.WriteLine(list.DebugEmpty);
#endif
            start = Environment.TickCount;

            var result2b = TestFunctions.fnTest4MAdd(list, 4);
            Console.Write("INSERTS = ");
            Console.WriteLine(result2b.ToString());

            Console.Write("TICKS = ");
            Console.WriteLine(Environment.TickCount - start);
#if (DEBUG)
            Console.WriteLine(list.DebugDump);
#endif
            Console.ReadLine();
        }
    }
}
