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
using System.Text;
using System.IO;
using System.Security;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

using Ximura;
using Ximura.Server;
#endregion // using
namespace Ximura.Helper
{
    public static partial class LinqHelper
    {
        public static IEnumerable<Tuple<int, int>> RangeTuple(int start, int count, int parts)
        {
            int block = count / parts;
            int lastblock = count % parts;

            int i = 0;

            for (int loop = 0; loop < parts; loop++)
            {
                int currentBlock = block;
                if (lastblock > 0)
                {
                    currentBlock++;
                    lastblock--;
                }

                yield return new Tuple<int,int>(i, currentBlock);

                i += currentBlock;
            }
        }

        #region RangeFromTo(Tuple<int, int> range)
        /// <summary>
        /// This extended range method takes an integer tuple as the range.
        /// </summary>
        /// <param name="range">The range of integers.</param>
        /// <returns>Returns an enumeration of integers based on the range specified.</returns>
        public static IEnumerable<int> RangeFromTo(Tuple<int, int> range)
        {
            return RangeFromTo(range.Item1, range.Item2);
        }
        #endregion // RangeFromTo(Tuple<int, int> range)

        #region RangeFromTo(int start, int end)
        /// <summary>
        /// This extended range method takes an integer range and returns an enumeration of integers.
        /// </summary>
        /// <param name="start">The first integer.</param>
        /// <param name="end">The last integer.</param>
        /// <returns>Returns an enumeration of integers based on the range specified.</returns>
        public static IEnumerable<int> RangeFromTo(int start, int end)
        {
            for (int loop = start; loop <= end; loop++)
                yield return loop;
        }
        #endregion // RangeFromTo(int start, int end)

    }
}
