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
    public static class Command
    {
        #region Command Parsing
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Args"></param>
        /// <returns>Returns a dictionary containing the collection of parameters and values.</returns>
        public static Dictionary<string, string> ParseArgs(string[] Args)
        {
            return ParseArgs(Args, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Args"></param>
        /// <param name="throwErrors"></param>
        /// <returns>Returns a dictionary containing the collection of parameters and values.</returns>
        public static Dictionary<string, string> ParseArgs(string[] Args, bool throwErrors)
        {
            return ParseArgs(Args, @"/", @":", throwErrors);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Args"></param>
        /// <param name="strStart"></param>
        /// <param name="throwErrors"></param>
        /// <returns>Returns a dictionary containing the collection of parameters and values.</returns>
        public static Dictionary<string, string> ParseArgs(string[] Args, string strStart, bool throwErrors)
        {
            return ParseArgs(Args, strStart, @":", throwErrors);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Args"></param>
        /// <param name="strStart"></param>
        /// <param name="strDelim"></param>
        /// <param name="throwErrors"></param>
        /// <returns>Returns a dictionary containing the collection of parameters and values.</returns>
        public static Dictionary<string, string> ParseArgs(string[] Args, string strStart, string strDelim, bool throwErrors)
        {
            //	This function parses the command line arguments to find the correct type
            //	based on the syntax /strOption:[strReturnData]
            //	Although there are more efficent ways, other than a for-next loop,
            //	to search through a list, there will only be a limited number of 
            //	items so this has not been optimized. 

            Dictionary<string, string> data = new Dictionary<string, string>();

            if (Args == null)
                return data;

            string strKey, strValue;

            foreach (string strData in Args)
            {
                try
                {
                    ParseData(strData, out strKey, out strValue, strStart, strDelim);

                    if (!data.ContainsKey(strKey))
                    {
                        data.Add(strKey, strValue);
                    }
                    else
                    {
                        if (throwErrors)
                            throw new ArgumentException("Multiple keys found.", strKey);
                    }
                }
                catch
                {
                    if (throwErrors)
                    {
                        //Check the string format
                        throw new ArgumentException("Incorrect format", strData);
                    }
                }


            }

            return data;

        }

        private static void ParseData(string strData, out string strKey,
            out string strValue, string strStart, string strDelim)
        {
            //Ok, trim any space of the data
            strData = strData.Trim();
            //Does the string start with strStart, if not throw an error.
            if (!strData.StartsWith(strStart)) throw new ArgumentException();

            //Is the delimiter of 0 length
            if (strDelim.Length == 0)
            {
                //Just return the key
                strKey = strData.Substring(strStart.Length - 1);
                strValue = "";
            }
            else
            {
                //OK, get the position of the delimiter
                int intDelim = strData.IndexOf(strDelim, strStart.Length);

                if (intDelim == -1)
                {
                    strKey = strData.Substring(strStart.Length);
                    strValue = "";
                }
                else
                {
                    strKey = strData.Substring(strStart.Length, intDelim - strStart.Length);
                    strValue = strData.Substring(intDelim + 1);
                }
            }


        }
        #endregion

    }
}
