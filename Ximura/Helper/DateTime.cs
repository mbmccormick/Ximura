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
﻿#region using
using System;
using System.Text;
using System.IO;
using System.Security;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

using Ximura;
using Ximura.Framework;
#endregion // using
namespace Ximura.Helper
{
    [Flags]
    /// <summary>
    /// Enum value of day of week in month
    /// </summary>
    public enum DayOfWeekInMonth
    {
        None = 0x0,
        First = 0x01,
        Second = 0x02,
        Third = 0x04,
        Fourth = 0x08,
        Last = 0x11
    }

    /// <summary>
    /// Provide special functionality to deal with DateTime
    /// </summary>
    public static class DateTimeHelper
    {
        #region GetWeekNumberInMonth
        /// <summary>
        /// Obtain the week number of the date in the current month.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int GetWeekNumberInMonth(DateTime dateTime)
        {
            int result = 1;
            DateTime startDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
            int lastDayOfFirstWeek = 7 - (int)startDayOfMonth.DayOfWeek;
            if (dateTime.Day > lastDayOfFirstWeek)
            {
                result = Convert.ToInt32(Math.Ceiling((decimal)(dateTime.Day - lastDayOfFirstWeek) / 7)) + 1;
            }
            return result;
        }
        #endregion
        #region GetWeekOccuranceInMonth
        /// <summary>
        /// Obtain the week occurance number in current month.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int GetWeekOccuranceInMonth(DateTime dateTime)
        {
            int result = 1;
            DateTime startDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
            int startDayDOW = (int)startDayOfMonth.DayOfWeek;
            int currentDOW = (int)dateTime.DayOfWeek;
            int dayDiffInDOW = 1;
            if (startDayDOW > currentDOW)
            {
                dayDiffInDOW += 7 - startDayDOW + currentDOW;
            }
            return ((dateTime.Day - dayDiffInDOW) / 7) + 1;
        }
        #endregion
        #region GetLastWeekNumberInMonth
        /// <summary>
        /// Obtain the last week number of the current day of week in the current month
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int GetLastWeekNumberInMonth(DateTime dateTime)
        {
            DateTime endDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddDays(-1);
            int diffDOW = (endDayOfMonth.DayOfWeek > dateTime.DayOfWeek) ? endDayOfMonth.DayOfWeek - dateTime.DayOfWeek : 7 - (int)dateTime.DayOfWeek + (int)endDayOfMonth.DayOfWeek;
            endDayOfMonth = endDayOfMonth.AddDays(-diffDOW);
            return GetWeekNumberInMonth(endDayOfMonth);
        }
        #endregion
        #region GetLastWeekOccuranceInMonth
        /// <summary>
        /// Obtain the last week occurance number of the current day of week in current month
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int GetLastWeekOccuranceInMonth(DateTime dateTime)
        {
            DateTime endDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddDays(-1);
            int diffDOW = (endDayOfMonth.DayOfWeek > dateTime.DayOfWeek) ? endDayOfMonth.DayOfWeek - dateTime.DayOfWeek : 7 - (int)dateTime.DayOfWeek + (int)endDayOfMonth.DayOfWeek;
            endDayOfMonth = endDayOfMonth.AddDays(-diffDOW);
            return GetWeekOccuranceInMonth(endDayOfMonth);
        }
        #endregion
        #region GetNumberOfDayInMonth
        public static int GetNumberOfDayInMonth(DateTime dateTime)
        {
            DateTime endDayOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddDays(-1);
            return endDayOfMonth.Day;
        }
        #endregion
        #region GetAbsoluteDate
        public static DateTime GetAbsoluteDate(DateTime dateTime)
        {
            return new DateTime(dateTime.Year
                                , dateTime.Month
                                , dateTime.Day
                                , 0
                                , 0
                                , 0);
        }
        #endregion
        #region DiffWeeks
        public static int DiffWeeks(DateTime comparer, DateTime comparee)
        {
            TimeSpan weekDiff = comparer - comparee;
            return Math.Abs(Convert.ToInt32(Math.Floor(weekDiff.TotalDays / 7)));
        }
        #endregion
        #region DiffDays
        public static int DiffDays(DateTime comparer, DateTime comparee)
        {
            TimeSpan weekDiff = comparer - comparee;
            return Math.Abs(Convert.ToInt32(Math.Ceiling(weekDiff.TotalDays)));
        }
        #endregion
        #region InBetween
        /// <summary>
        /// Check if the verify date is equal or in between the interval start and end date.
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="intervalStart"></param>
        /// <param name="internalEnd"></param>
        /// <returns></returns>
        public static bool InBetween(DateTime comparer, DateTime intervalStart, DateTime internalEnd)
        {
            return comparer.CompareTo(intervalStart) >= 0 && comparer.CompareTo(internalEnd) <= 0;
        }
        #endregion


    }
}
