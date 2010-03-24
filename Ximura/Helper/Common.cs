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
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

using Ximura;
using Ximura.Framework;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// The <b>Common</b> class includes a number of useful utilities.
    /// </summary>
    public static partial class Common
    {

        #region Write Bytes Functions

        public static void WriteBytes(MemoryStream Message, string strData)
        {
            byte[] byData = Encoding.ASCII.GetBytes(strData);
            Message.Write(byData, 0, byData.Length);
        }

        public static void WriteBytes(MemoryStream Message, StringBuilder strbData)
        {
            WriteBytes(Message, strbData, true);
        }

        public static void WriteBytes(MemoryStream Message, StringBuilder strbData, bool blnClearSTRB)
        {
            if (strbData.Length > 0)
            {
                byte[] byData = Encoding.ASCII.GetBytes(strbData.ToString());
                Message.Write(byData, 0, byData.Length);
            }
            if (blnClearSTRB) strbData.Length = 0;
        }

        public static void WriteBytes(MemoryStream Message, byte[] byData)
        {
            Message.Write(byData, 0, byData.Length);
        }

        /// <summary>
        /// This converts a string in to an ASCII byte array. This method
        /// adds a CRLF at the end of the string by default.
        /// </summary>
        /// <param name="strData">The string you wish to convert.</param>
        /// <returns>A byte array containing an ASCII representation of the string.</returns>
        public static byte[] ASCByt(string strData)
        {
            return ASCByt(strData, true);
        }
        /// <summary>
        /// This converts a string in to an ASCII byte array.
        /// </summary>
        /// <param name="strData">The string you wish to convert.</param>
        /// <param name="blnAddCRLF">Select true if you want a new line appended at the end.</param>
        /// <returns>A byte array containing an ASCII representation of the string.</returns>
        public static byte[] ASCByt(string strData, bool blnAddCRLF)
        {
            if (blnAddCRLF) return Encoding.ASCII.GetBytes(strData + Environment.NewLine);

            return Encoding.ASCII.GetBytes(strData);
        }

        #endregion



        #region HTTP Codes
        /// <summary>
        /// This class provides shortcuts to the HTTP codes.
        /// </summary>
        public class HTTPCodes
        {
            /// <summary>
            /// 100: Continue
            /// </summary>
            public static string Continue_100 { get { return "100"; } }
            /// <summary>
            /// 101: Switching Protocols
            /// </summary>
            public static string SwitchingProtocols_101 { get { return "101"; } }

            /// <summary>
            /// 200: OK
            /// </summary>
            public static string OK_200 { get { return "200"; } }
            /// <summary>
            /// 201: Created
            /// </summary>
            public static string Created_201 { get { return "201"; } }
            /// <summary>
            /// 202: Accepted
            /// </summary>
            public static string Accepted_202 { get { return "202"; } }
            /// <summary>
            /// 203: Non Authoritative
            /// </summary>
            public static string NonAuthoritative_203 { get { return "203"; } }
            /// <summary>
            /// 204: No Content
            /// </summary>
            public static string NoContent_204 { get { return "204"; } }
            /// <summary>
            /// 205: Reset Content
            /// </summary>
            public static string ResetContent_205 { get { return "205"; } }
            /// <summary>
            /// 206: Partial Content
            /// </summary>
            public static string PartialContent_206 { get { return "206"; } }

            /// <summary>
            /// 300: Multiple Choices
            /// </summary>
            public static string MultipleChoices_300 { get { return "300"; } }
            /// <summary>
            /// 301: Moved Permanently
            /// </summary>
            public static string MovedPermanently_301 { get { return "301"; } }
            /// <summary>
            /// 302: Found
            /// </summary>
            public static string Found_302 { get { return "302"; } }
            /// <summary>
            /// 303: See Other
            /// </summary>
            public static string SeeOther_303 { get { return "303"; } }
            /// <summary>
            /// 304: Not Modified
            /// </summary>
            public static string NotModified_304 { get { return "304"; } }
            /// <summary>
            /// 305: Use Proxy
            /// </summary>
            public static string UseProxy_305 { get { return "305"; } }
            /// <summary>
            /// 306: Redirection Command Not Used
            /// </summary>
            public static string ReDirectionCommandNotUsed_306 { get { return "306"; } }
            /// <summary>
            /// 307: Moved Temporarily
            /// </summary>
            public static string MovedTemporarily_307 { get { return "307"; } }

            /// <summary>
            /// 400: Bad Request
            /// </summary>
            public static string BadRequest_400 { get { return "400"; } }
            /// <summary>
            /// 401: Unauthorized
            /// </summary>
            public static string Unauthorized_401 { get { return "401"; } }
            /// <summary>
            /// 402: Payment Required
            /// </summary>
            public static string PaymentRequired_402 { get { return "402"; } }
            /// <summary>
            /// 403: Forbidden
            /// </summary>
            public static string Forbidden_403 { get { return "403"; } }
            /// <summary>
            /// 404: Not Found
            /// </summary>
            public static string NotFound_404 { get { return "404"; } }
            /// <summary>
            /// 405: Method Not Allowed
            /// </summary>
            public static string MethodNotAllowed_405 { get { return "405"; } }
            /// <summary>
            /// 406: Not Acceptable
            /// </summary>
            public static string NotAcceptable_406 { get { return "406"; } }
            /// <summary>
            /// 407: Proxy Authentication Required
            /// </summary>
            public static string ProxyAuthenticationRequired_407 { get { return "407"; } }
            /// <summary>
            /// 408: Request Timeout
            /// </summary>
            public static string RequestTimeout_408 { get { return "408"; } }
            /// <summary>
            /// 409: Conflict
            /// </summary>
            public static string Conflict_409 { get { return "409"; } }
            /// <summary>
            /// 410: Gone
            /// </summary>
            public static string Gone_410 { get { return "410"; } }
            /// <summary>
            /// 411: Length Required
            /// </summary>
            public static string LengthRequired_411 { get { return "411"; } }
            /// <summary>
            /// 412: Precondition Failed
            /// </summary>
            public static string PreconditionFailed_412 { get { return "412"; } }
            /// <summary>
            /// 413: Request Entity Too Large
            /// </summary>
            public static string RequestEntityTooLarge_413 { get { return "413"; } }
            /// <summary>
            /// 414: Request URI Too Long
            /// </summary>
            public static string RequestURITooLong_414 { get { return "414"; } }
            /// <summary>
            /// 415: Unsupported Media Type
            /// </summary>
            public static string UnsupportedMediaType_415 { get { return "415"; } }
            /// <summary>
            /// 416: Requested Range Not Satisfiable
            /// </summary>
            public static string RequestedRangeNotSatisfiable_416 { get { return "416"; } }
            /// <summary>
            /// 417: Expectation Failed
            /// </summary>
            public static string ExpectationFailed_417 { get { return "417"; } }

            /// <summary>
            /// 500: Internal Server Error
            /// </summary>
            public static string InternalServerError_500 { get { return "500"; } }
            /// <summary>
            /// 501: Not Implemented
            /// </summary>
            public static string NotImplemented_501 { get { return "501"; } }
            /// <summary>
            /// 502: Bad Gateway
            /// </summary>
            public static string BadGateway_502 { get { return "502"; } }
            /// <summary>
            /// 503: Service Unavailable
            /// </summary>
            public static string ServiceUnavailable_503 { get { return "503"; } }
            /// <summary>
            /// 504: Gateway Timeout
            /// </summary>
            public static string GatewayTimeout_504 { get { return "504"; } }
            /// <summary>
            /// 505: Version No tSupported
            /// </summary>
            public static string VersionNotSupported_505 { get { return "505"; } }

            #region IsHTTPError(string status)
            /// <summary>
            /// This method returns true if the status is an HTTP error.
            /// </summary>
            /// <param name="status">The status to check.</param>
            /// <returns>Returns true if the status code denotes an error.</returns>
            public static bool IsHTTPError(string status)
            {
                return IsServerError(status) || status.StartsWith("4", true, CultureInfo.InvariantCulture);
            }
            #endregion // IsHTTPError(string status)
            #region IsServerError(string status)
            /// <summary>
            /// This method returns true if the status code is an error.
            /// </summary>
            /// <param name="status">The status to check.</param>
            /// <returns>Returns true if the status code denotes an error.</returns>
            public static bool IsServerError(string status)
            {
                return status.StartsWith("5", true, CultureInfo.InvariantCulture);
            }
            #endregion // IsServerError(string status)
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// This method tests whether a string is numeric.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(string value)
        {
            if (value == null || value == "")
                return false;

            double tryIt;
            return double.TryParse(value, out tryIt);
        }



        #endregion // Helper Methods



        #region CreatePasswordHash
        /// <summary>
        /// This method creates a password hash from a byte64 encoded text seed and plain text password
        /// </summary>
        /// <param name="Seed"></param>
        /// <param name="Pass"></param>
        /// <returns></returns>
        public static string CreatePasswordHash(string Seed, string Pass)
        {
            byte[] seed = System.Text.UTF8Encoding.UTF8.GetBytes(Seed);

            byte[] pass = System.Text.UTF8Encoding.UTF8.GetBytes(Pass);

            byte[] rawSalted = new byte[pass.Length + seed.Length];
            pass.CopyTo(rawSalted, 0);
            seed.CopyTo(rawSalted, pass.Length);

            HashAlgorithm mhash = new MD5CryptoServiceProvider();

            //Create the hash value from the array of bytes.
            byte[] HashValue = mhash.ComputeHash(rawSalted);
            return Convert.ToBase64String(HashValue);
        }
        #endregion // CreatePasswordHash

        #region SplitOnChars
        /// <summary>
        /// This method is used to split string pairs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="toSplit"></param>
        /// <param name="convertT"></param>
        /// <param name="convertU"></param>
        /// <param name="split1"></param>
        /// <param name="split2"></param>
        /// <returns></returns>
        public static List<KeyValuePair<T, U>> SplitOnChars<T, U>(string toSplit,
            Converter<string, T> convertT, Converter<string, U> convertU,
                char[] split1, char[] split2)
        {
            if (toSplit == null)
                throw new ArgumentNullException("toSplit", "toSplit cannot be null.");

            List<KeyValuePair<T, U>> newList = new List<KeyValuePair<T, U>>();

            string[] pairs = toSplit.Split(split1, StringSplitOptions.RemoveEmptyEntries);

            if (pairs.Length == 0)
                return newList;

            foreach (string pair in pairs)
            {
                string[] pairSplit = pair.Split(split2);
                string secondParam = pairSplit.Length == 1 ? null : pairSplit[1];
                KeyValuePair<T, U> keyPair =
                    new KeyValuePair<T, U>(convertT(pairSplit[0]), convertU(secondParam));
                newList.Add(keyPair);
            }

            return newList;
        }
        #endregion // SplitOnChars
        #region SplitOnCharsUnique
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="toSplit"></param>
        /// <param name="convertT"></param>
        /// <param name="convertU"></param>
        /// <param name="split1"></param>
        /// <param name="split2"></param>
        /// <returns></returns>
        public static Dictionary<T, U> SplitOnCharsUnique<T, U>(string toSplit,
            Converter<string, T> convertT, Converter<string, U> convertU, char[] split1, char[] split2)
        {
            if (toSplit == null)
                throw new ArgumentNullException("toSplit", "toSplit cannot be null.");

            Dictionary<T, U> newList = new Dictionary<T, U>();

            string[] pairs = toSplit.Split(split1, StringSplitOptions.RemoveEmptyEntries);

            if (pairs.Length == 0)
                return newList;

            foreach (string pair in pairs)
            {
                string[] pairSplit = pair.Split(split2);
                string secondParam = pairSplit.Length == 1 ? null : pairSplit[1];
                newList.Add(convertT(pairSplit[0]), convertU(secondParam));
            }

            return newList;
        }
        #endregion // SplitOnCharsUnique

        #region ConvPassthru
        public static Converter<string, string> ConvPassthru = 
            delegate(string input) 
                { 
                    return input; 
                };
        #endregion // ConvPassthru
        #region ConvPassthruLowerCase
        public static Converter<string, string> ConvPassthruLowerCase = 
            delegate(string input) 
                { 
                    return input.Trim().ToLowerInvariant(); 
                };
        #endregion // ConvPassthruLowerCase
        #region ConvQParam
        public static Converter<string, string> ConvQParam = 
            delegate(string input)
                {
                    if (input == null)
                        return null;
                    input = input.Trim().ToLower();
                    if (!input.StartsWith("q="))
                        return null;
                    return input.Substring(2);
                };
        #endregion // ConvQParam
        #region ConvStripSpeechMarks
        public static Converter<string, string> ConvStripSpeechMarks = 
            delegate(string input)
                {
                    if (input == null)
                        return null;
                    if (input == @"""""")
                        return "";
                    if (!input.StartsWith(@"""") || !input.EndsWith(@"""") || input.Length < 2)
                        return input;
                    return input.Substring(1, input.Length - 2);
                };
        #endregion // ConvStripSpeechMarks
    }
}

