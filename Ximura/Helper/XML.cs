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
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// The XML Helper library is used to provide a set of reusable methods for manipulation XML Document information.
    /// </summary>
    public static class XMLHelper
    {
        #region XmlMappingGet
        public static T XmlMappingGet<T>(XmlDocument XmlDataDoc, XmlNamespaceManager NSM, string xPath, Converter<string, T> convert)
        {
            XmlNode node = XmlDataDoc.SelectSingleNode(xPath, NSM);
            return XmlMappingGet<T>(node, convert);
        }

        public static T XmlMappingGet<T>(XmlNode node, Converter<string, T> convert)
        {
            string input = node.InnerText;
            return convert(input);
        }

        public static string XmlMappingGetToString(XmlDocument XmlDataDoc, XmlNamespaceManager NSM, string xPath)
        {
            return XmlMappingGet<string>(XmlDataDoc, NSM, xPath, delegate(string input) { return input; });
        }

        public static bool XmlMappingGetToBool(XmlDocument XmlDataDoc, XmlNamespaceManager NSM, string xPath)
        {
            return XmlMappingGet<bool>(XmlDataDoc, NSM, xPath, delegate(string input) { return bool.Parse(input); });
        }

        public static int XmlMappingGetToInt32(XmlDocument XmlDataDoc, XmlNamespaceManager NSM, string xPath)
        {
            return XmlMappingGet<int>(XmlDataDoc, NSM, xPath, delegate(string input) { return int.Parse(input); });
        }


        public static Uri XmlMappingGetToUri(XmlDocument XmlDataDoc, XmlNamespaceManager NSM, string xPath)
        {
            return XmlMappingGet<Uri>(XmlDataDoc, NSM, xPath,
                delegate(string input)
                {
                    if (input == null || input == "")
                        return null;
                    return new Uri(input);
                }
            );
        }

        public static int? XmlMappingGetToInt32Nullable(XmlDocument XmlDataDoc, XmlNamespaceManager NSM, string xPath)
        {
            return XmlMappingGet<int?>(XmlDataDoc, NSM, xPath, delegate(string input)
            { return input == null ? (int?)null : int.Parse(input); }
            );
        }
        #endregion // XmlMappingGet

        #region XmlMappingSet
        public static bool XmlMappingSet<T>(XmlDocument XmlDataDoc, XmlNamespaceManager NSM, string xPath, T input)
        {
            XmlNode node = XmlDataDoc.SelectSingleNode(xPath, NSM);
            return XmlMappingSet<T>(node, input, delegate(T inputData) { return inputData.ToString(); });
        }

        public static bool XmlMappingSet<T>(XmlNode node, T input)
        {
            return XmlMappingSet<T>(node, input, delegate(T inputData) { return inputData.ToString(); });
        }

        public static bool XmlMappingSet<T>(XmlDocument XmlDataDoc, XmlNamespaceManager NSM, string xPath, T input, Converter<T, string> convert)
        {
            XmlNode toSet = XmlDataDoc.SelectSingleNode(xPath, NSM);
            return XmlMappingSet<T>(toSet, input, convert);
        }

        public static bool XmlMappingSet<T>(XmlNode toSet, T input, Converter<T, string> convert)
        {
            if (toSet == null)
                return false;
            toSet.InnerText = convert(input);
            return true;
        }


        public static bool XmlMappingSet(XmlDocument XmlDataDoc, XmlNamespaceManager NSM, string xPath, Uri input)
        {
            return XmlMappingSet<Uri>(XmlDataDoc, NSM, xPath, input, delegate(Uri inputData) { return inputData.ToString(); });
        }


        public static bool XmlMappingSet(XmlDocument XmlDataDoc, XmlNamespaceManager NSM, string xPath, string input)
        {
            return XmlMappingSet<string>(XmlDataDoc, NSM, xPath, input, delegate(string inputData) { return inputData; });
        }

        public static bool XmlMappingSet(XmlDocument XmlDataDoc, XmlNamespaceManager NSM, string xPath, bool input)
        {
            return XmlMappingSet<bool>(XmlDataDoc, NSM, xPath, input, delegate(bool inputData) { return inputData ? "true" : "false"; });
        }
        #endregion // XmlMappingSet
    }
}
