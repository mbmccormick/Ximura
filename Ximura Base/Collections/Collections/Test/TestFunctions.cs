#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// Contributors:
//     Paul Stancer - initial implementation
// *******************************************************************************
#endregion
#region using
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    public static class TestFunctions
    {
        #region Functions
        public static Func<Action<int, int>, int, int, int, IEnumerable<Action>> fnBlocksCreate =
            (action, start, count, blocks) =>
                LinqHelper.RangeTuple(start, count, blocks).Select((t) => new Action(() => action(t.Item1, t.Item2)));

        public static Action<ICollection<int>, int, int> fnCollectionAdd =
            (coll, start, count) =>
                Enumerable.Range(start, count).ForEach(i => coll.Add(i));

        public static Action<ICollection<int>, int, int> fnCollectionRemove =
            (coll, start, count) =>
                Enumerable.Range(start, count).ForEach(i => coll.Remove(i));

        public static Action<ICollection<int>, int, int> fnCollectionContains =
            (coll, start, count) =>
                Enumerable.Range(start, count).ForEach(i => 
                    {
                        if (!coll.Contains(i))
                            throw new IndexOutOfRangeException();
                    });

        /// <summary>
        /// This test function loads a collection with 4 million records in blocks as specified by the blocks parameter.
        /// </summary>
        public static Func<ICollection<int>, int, TimeSpan> fnTest4MAdd =
            (coll, blocks) =>
                fnBlocksCreate(fnCollectionAdd.Curry()(coll), 0, 4000000, blocks).Execute();

        public static Func<ICollection<int>, int, TimeSpan> fnTest4MRemove =
            (coll, blocks) =>
                fnBlocksCreate(fnCollectionRemove.Curry()(coll), 0, 4000000, blocks).Execute();

        public static Func<ICollection<int>, int, TimeSpan> fnTest4MContains =
            (coll, blocks) =>
                fnBlocksCreate(fnCollectionContains.Curry()(coll), 0, 4000000, blocks).Execute();


        public static Tuple<TimeSpan, TimeSpan> CalcAndCompare<WRP, COL>()
            where WRP : WrapperBase<int>, ICollection<int>, new()
            where COL : class, ICollection<int>, new()
        {
            WRP coll1 = new WRP { Collection = new COL() };

            WRP coll2 = new WRP { Collection = new COL() };

            var result = new Tuple<TimeSpan, TimeSpan>(fnTest4MAdd(coll1, 1), fnTest4MAdd(coll2, 16));

            if (coll1.Count != coll2.Count)
                throw new Exception("Collection mismatch.");

            return result;
        }

        public static Tuple<TimeSpan, TimeSpan> CalcAndCompare<COL>()
            where COL : class, ICollection<int>, new()
        {
            COL coll1 = new COL();

            COL coll2 = new COL();

            var result = new Tuple<TimeSpan, TimeSpan>(fnTest4MAdd(coll1, 1), fnTest4MAdd(coll2, 16));

            if (coll1.Count != coll2.Count)
                throw new Exception("Collection mismatch.");

            return result;
        }
        #endregion // Functions

        //public static Action<ICollection<int>> collAdd = (coll) => Enumerable.Range(1, 10).ForEach(i => coll.Expand(i * 10));



        public static Func<ICollection<int>, Action<ICollection<int>, int, int>, Action<int, int>> fnCurry =
            (coll, bigAction) => new Action<int, int>((a, b) => bigAction(coll, a, b));

        //Func<ICollection<int>, Action<ICollection<int>, int, int>, Action<int, int>> fnCurry2 =
        //    (coll, action) => Linq.Curry(action)(coll);

    }
}
