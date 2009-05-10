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

            CourseGrainedICollectionWrapper<int> wrp = new CourseGrainedICollectionWrapper<int>();
            wrp.Collection = new HashSet<int>();
            var resultn = TestFunctions.fnTest4MAdd(wrp, 4);
            Console.WriteLine(resultn.ToString());

            var resultnc = TestFunctions.fnTest4MContains(wrp, 4);
            Console.WriteLine(resultnc.ToString());

            int start = Environment.TickCount;

            LockFreeList<int> list = new LockFreeList<int>(4200000);

            var result2 = TestFunctions.fnTest4MAdd(list, 4);
            Console.Write("INSERTS = ");
            Console.WriteLine(result2.ToString());

            Console.Write("TICKS = ");
            Console.WriteLine(Environment.TickCount - start);

#if (DEBUG)
            Console.WriteLine(list.DebugDump);
            list.DebugReset();
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
            Console.ReadLine();
        }
    }
}
