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
using System.Threading;
using System.Timers;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

using System.Diagnostics;

using Ximura;
using Ximura.Helper;
using CH=Ximura.Helper.Common;
using Ximura.Data;

using Ximura.Server;
using Ximura.Command;
#endregion // using
namespace Ximura.Communication
{
    public class WWWFormUrlEncodedBodyFragment : InternetMessageFragmentBody
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public WWWFormUrlEncodedBodyFragment()
            : base()
        {
        }
        #endregion

        #region ExtractDataDictionary()
        /// <summary>
        /// This method extracts the URL encoded body data as a string dictionary.
        /// </summary>
        /// <returns>Returns a dictionary containing the data.</returns>
        public virtual Dictionary<string, string> ExtractDataDictionary()
        {
            if (this.Length == 0)
                return new Dictionary<string, string>();

            string UrlData = Encoding.ASCII.GetString(mBuffer, 0, (int)Length);

            Dictionary<string, string> data = 
                CH.SplitOnCharsUnique<string, string>(
                  UrlData
                , CH.ConvPassthru
                , delegate(string input)
                {
                    if (input == null)
                        return null;
                    return Uri.UnescapeDataString(input);
                }, new char[] { '&' }, new char[] { '=' });

            return data;
        }
        #endregion // ExtractDataUnique()
        #region ExtractDataList()
        /// <summary>
        /// This method extracts the incoming data as a keyvaluepair. This method allows multiple parameters
        /// of the same key value to be extracted from the data.
        /// </summary>
        /// <returns></returns>
        public virtual List<KeyValuePair<string, string>> ExtractDataList()
        {
            if (this.Length == 0)
                return new List<KeyValuePair<string, string>>();

            string UrlData = Encoding.ASCII.GetString(mBuffer, 0, (int)Length);

            List<KeyValuePair<string, string>> data = 
                CH.SplitOnChars<string, string>(UrlData, CH.ConvPassthru,
                delegate(string input)
                {
                    if (input == null)
                        return null;
                    return Uri.UnescapeDataString(input);
                }, new char[] { '&' }, new char[] { '=' });

            return data;
        }
        #endregion // ExtractDataList()
    }
}
