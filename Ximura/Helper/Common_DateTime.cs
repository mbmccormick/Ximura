#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2010 Paul Stancer.
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
    /// <summary>
    /// The <b>Common</b> class includes a number of useful utilities.
    /// </summary>
    public static partial class Common
    {
        /// <summary>
        /// This method tests whether a string is a DateTime.
        /// </summary>
        /// <param name="dateValue">The date value.</param>
        /// <returns>Returns true if the string is a date.</returns>
        public static bool IsDateTime(string dateValue)
        {
            return IsDateTime(dateValue, "000000");
        }
        /// <summary>
        /// This method tests whether a string is a DateTime.
        /// </summary>
        /// <param name="dateValue">The date value.</param>
        /// <param name="timeValue">The time value.</param>
        /// <returns>Returns true if the string is a date.</returns>
        public static bool IsDateTime(string dateValue, string timeValue)
        {
            DateTime dtN;
            return DateTime.TryParse(ConvertToISO8601DateString(dateValue + timeValue), out dtN);
        }
        /// <summary>
        /// This method tests whether a string is a DateTime.
        /// </summary>
        /// <param name="dateValue">The date value.</param>
        /// <param name="timeValue">The time value.</param>
        /// <returns>Returns true if the string is a date.</returns>
        public static bool IsDateTime(string dateValue, string timeValue, out DateTime? dtN)
        {
            dtN = null;
            if (dateValue == string.Empty)
                return false;

            if (timeValue == string.Empty)
                return false;

            DateTime dt;
            bool success = DateTime.TryParse(ConvertToISO8601DateString(dateValue + timeValue), out dt);

            if (success)
                dtN = dt;

            return success;
        }


        #region Convert Date String to ISO 8601 DateTime Format
        ///// <summary>
        ///// Function to convert string to ISO8601 datetime string yyyy-MM-ddTHH:mm:ss
        ///// </summary>
        ///// <param name="strDateTime">string in special format ( yyyyMMdd | yyyyMMddHH | yyyyMMddHHmm | yyyyMMddHHmmss</param>		
        ///// <returns>ISO8601 datetime string</returns>
        //public static string ConvertToISO8601DateString(string strDateTime)
        //{
        //    StringBuilder strDate = new StringBuilder();

        //    strDate.Append(strDateTime.Substring(0,4) + "-"); //append the year 
        //    strDate.Append(strDateTime.Substring(4,2) + "-"); //append the month
        //    strDate.Append(strDateTime.Substring(6,2) + "T"); //append the day

        //    switch (strDateTime.Length)
        //    {
        //        case 8:		//format yyyyMMdd 
        //            strDate.Append("00:00:00");					
        //            break;
        //        case 10:		//format yyyyMMddHH
        //            strDate.Append(strDateTime.Substring(8,2) + ":00:00");
        //            break;
        //        case 12:		//format yyyyMMddHHmm 
        //            strDate.Append(strDateTime.Substring(8,2) + ":");
        //            strDate.Append(strDateTime.Substring(10,2) + ":00");							
        //            break;
        //        case 14:		//format yyyyMMddHHmmss 
        //            strDate.Append(strDateTime.Substring(8,2) + ":");
        //            strDate.Append(strDateTime.Substring(10,2) + ":");
        //            strDate.Append(strDateTime.Substring(12,2));	
        //            break;
        //        default:
        //            return "";
        //    }		

        //    return strDate.ToString();
        //}
        #region ConvertToRFC1123DateString
        /// <summary>
        /// This method converts a datetime parameter to an RFC1123 string formar.
        /// </summary>
        /// <param name="dt">The date time.</param>
        /// <returns>The string representation.</returns>
        public static string ConvertToRFC1123DateString(DateTime dt)
        {
            return dt.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss") + " GMT";
        }
        #endregion // ConvertToRFC1123DateString
        #region ToRFC1123String(this DateTime dt)
        /// <summary>
        /// This method converts a datetime parameter to an RFC1123 string formar.
        /// </summary>
        /// <param name="dt">The date time.</param>
        /// <returns>The string representation.</returns>
        public static string ToRFC1123String(this DateTime dt)
        {
            return dt.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss") + " GMT";
        }
        #endregion // ToRFC1123String(this DateTime dt)


        /// <summary>
        /// Function to convert string to ISO8601 datetime string yyyy-MM-ddTHH:mm:ss
        /// </summary>
        /// <param name="strDateTime">string in special format ( yyyyMMdd | yyyyMMddHH | yyyyMMddHHmm | yyyyMMddHHmmss</param>		
        /// <returns>ISO8601 datetime string</returns>
        public static string ConvertToISO8601DateString(string strDateTime)
        {
            if (strDateTime.Length < 8)
                return "";

            string strDate = strDateTime.Substring(0, 4) + "-"; //append the year 
            strDate += strDateTime.Substring(4, 2) + "-"; //append the month
            strDate += strDateTime.Substring(6, 2) + "T"; //append the day

            switch (strDateTime.Length)
            {
                case 8:		//format yyyyMMdd 
                    strDate += "00:00:00";
                    break;
                case 10:		//format yyyyMMddHH
                    strDate += strDateTime.Substring(8, 2) + ":00:00";
                    break;
                case 12:		//format yyyyMMddHHmm 
                    strDate += strDateTime.Substring(8, 2) + ":";
                    strDate += strDateTime.Substring(10, 2) + ":00";
                    break;
                case 14:		//format yyyyMMddHHmmss 
                    strDate += strDateTime.Substring(8, 2) + ":";
                    strDate += strDateTime.Substring(10, 2) + ":";
                    strDate += strDateTime.Substring(12, 2);
                    break;
                default:
                    return "";
            }

            return strDate;
        }

        public static string ConvertToISO8601DateStringWithOffset(DateTime dtDateTime)
        {
            string offsetStr = "Z";
            TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset(dtDateTime);
            if (offset != TimeSpan.Zero)
            {
                if (offset.Hours < 0)
                    offsetStr = "-";
                else
                    offsetStr = "+";
                offsetStr += offset.Hours.ToString("00") + ":" + offset.Minutes.ToString("00");
            }
            return ConvertToISO8601DateString(dtDateTime) + offsetStr;
        }
        /// <summary>
        /// Function to convert datetime to ISO8601 datetime string yyyy-MM-ddTHH:mm:ss
        /// </summary>
        /// <param name="dtDateTime">datetime</param>		
        /// <returns>ISO8601 datetime string</returns>
        public static string ConvertToISO8601DateString(DateTime dtDateTime)
        {
            StringBuilder sbDateTime = new StringBuilder();

            sbDateTime.Append(dtDateTime.Year.ToString().PadLeft(4, '0') + "-"); //append the year 
            sbDateTime.Append(dtDateTime.Month.ToString().PadLeft(2, '0') + "-"); //append the month
            sbDateTime.Append(dtDateTime.Day.ToString().PadLeft(2, '0') + "T"); //append the day
            sbDateTime.Append(dtDateTime.Hour.ToString().PadLeft(2, '0') + ":"); //append the hour
            sbDateTime.Append(dtDateTime.Minute.ToString().PadLeft(2, '0') + ":"); //append the minute
            sbDateTime.Append(dtDateTime.Second.ToString().PadLeft(2, '0')); //append the second

            return sbDateTime.ToString();
        }
        /// <summary>
        /// This method adjusts the incoming 6 digit date to a 8 digit date with the century included.
        /// </summary>
        /// <param name="strDate">date</param>		
        /// <returns>8-digit string</returns>
        public static string AdjustCentury(string strDate)
        {
            if (strDate.Length != 6)
                return strDate;

            return GuessCentury(int.Parse(strDate.Substring(0, 2))).ToString() + strDate;
        }
        /// <summary>
        /// Guess the Year based on the Month and the Current Year
        /// </summary>
        /// <param name="Month">Month</param>
        /// <returns>Year</returns>
        public static int GuessYear(int Month)
        {
            int currMonth = DateTime.Now.Month;

            if (Month < 0 || Month > 12)
                throw new ArgumentOutOfRangeException();
            else if (currMonth < Month)
                return DateTime.Now.Year - 1;
            else
                return DateTime.Now.Year;
        }

        /// <summary>
        /// Guess the Century of the 2-digit Year, calculated based on the current century +/- 50 years
        /// </summary>
        /// <param name="Year">Year</param>
        /// <returns>Century</returns>
        public static int GuessCentury(int Year)
        {
            DateTime today = DateTime.Today;
            int currCentury;
            int currYear;

            currYear = (int)decimal.Remainder(today.Year, 100);
            currCentury = (today.Year - currYear) / 100;

            if (Math.Abs(currYear - Year) < 50)
            {
                return currCentury;
            }
            else if (currYear < Year)
            {
                return currCentury - 1;
            }
            else
            {
                return currCentury + 1;
            }
        }
        public static DateTime ConvertAgeToBirthday(int age)
        {
            // set to the Jan 1
            DateTime birthday = new DateTime(DateTime.Today.Year - age, 1, 1);
            return birthday;
        }

        public static Decimal StringDateToAge(string dateValue)
        {
            int year = 0;
            try
            {

                year = DateTime.Now.Year - DateTime.Parse(dateValue).Year;

            }
            catch
            {
                return 0;
            }
            return year;
        }

        #endregion
        #region Convert Expiry Date
        public static DateTime ConvertExpiryDate(int expiryYear, int expiryMonth)
        {
            DateTime expiryDate = DateTime.MinValue;

            // guess the century
            if (expiryYear < 100)
            {
                expiryYear += (GuessCentury(expiryYear) * 100);
            }
            try
            {
                // set to the start of the month (1st day of the month)
                expiryDate = new DateTime(expiryYear, expiryMonth, 1);
                // set it to the last day of the month by adding 1 month and subtract 1 day
                expiryDate = expiryDate.AddMonths(1).AddDays(-1);
            }
            catch
            {
                throw new ArgumentException("Expiry Date is not date time");
            }
            return expiryDate;
        }
        #endregion

    }
}
