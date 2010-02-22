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
using System.Data;

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;
using System.Net;

using Ximura;
using Ximura.Data;
using Ximura.Server;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Data
{
    public partial class XmlContentBase
    {
        #region short
        protected short XmlMappingGetToInt16(string xPath)
        {
            return XmlMappingGet<short>(xPath, delegate(string input) { return short.Parse(input); });
        }

        protected short? XmlMappingGetToInt16Nullable(string xPath)
        {
            return XmlMappingGet<short?>(xPath, delegate(string input)
            {
                return input == null ? (short?)null : short.Parse(input);
            }
            );
        }

        protected bool XmlMappingSet(string xPath, short input)
        {
            return XmlMappingSet<short>(xPath, input);
        }
        #endregion
        #region int
        protected int XmlMappingGetToInt32(string xPath)
        {
            return XmlMappingGet<int>(xPath, delegate(string input) { return int.Parse(input); });
        }

        protected int? XmlMappingGetToInt32Nullable(string xPath)
        {
            return XmlMappingGet<int?>(xPath, delegate(string input)
            { return input == null ? (int?)null : int.Parse(input); }
            );
        }

        protected bool XmlMappingSet(string xPath, int input)
        {
            return XmlMappingSet<int>(xPath, input);
        }
        #endregion // int
        #region long
        protected bool XmlMappingSet(string xPath, long input)
        {
            return XmlMappingSet<long>(xPath, input);
        }

        protected long XmlMappingGetToInt64(string xPath)
        {
            return XmlMappingGet<long>(xPath, delegate(string input) { return long.Parse(input); });
        }

        protected long? XmlMappingGetToInt64Nullable(string xPath)
        {
            return XmlMappingGet<long?>(xPath, delegate(string input)
            { return input == null ? (long?)null : long.Parse(input); }
            );
        }
        #endregion // long
        #region Guid?
        protected Guid? XmlMappingGetToGuidNullable(string xPath)
        {
            return XmlMappingGet<Guid?>(xPath,
                delegate(string input)
                {
                    if (input == null || input == "")
                        return null;
                    return new Guid(input);
                }
            );
        }

        protected bool XmlMappingSet(string xPath, Guid? input)
        {
            return XmlMappingSet<Guid?>(xPath, input, delegate(Guid? data)
            {
                return data.HasValue ? data.ToString().ToUpperInvariant() : "";
            });
        }    

        #endregion // Guid?
        #region String
        protected string XmlMappingGetToString(string xPath)
        {
            return XmlMappingGet<string>(xPath, delegate(string input) { return input; });
        }

        protected bool XmlMappingSet(string xPath, string input)
        {
            return XmlMappingSet<string>(xPath, input, delegate(string inputData) { return inputData; });
        }
        #endregion // String
        #region bool
        protected bool XmlMappingGetToBool(string xPath)
        {
            
            return XmlMappingGet<bool>(xPath, delegate(string input) { return bool.Parse(input); });
        }

        protected bool XmlMappingSet(string xPath, bool input)
        {
            return XmlMappingSet<bool>(xPath, input, delegate(bool inputData) { return inputData ? "true" : "false"; });
        }
        #endregion // bool
        #region byte[]
        protected byte[] XmlMappingGetToByteArray(string xPath)
        {
            return XmlMappingGet<byte[]>(xPath, delegate(string input) { return Convert.FromBase64String(input); });
        }

        protected bool XmlMappingSet(string xPath, byte[] input)
        {
            return XmlMappingSet<string>(xPath, Convert.ToBase64String(input));
        }
        #endregion // byte[]
        #region DateTime?
        protected DateTime? XmlMappingGetToDateTimeNullable(string xPath)
        {
            return XmlMappingGet<DateTime?>(xPath, delegate(string data) { return (data == "" ? (DateTime?)null : DateTime.Parse(data)); });
        }

        protected bool XmlMappingSet(string xPath, DateTime? input)
        {
            return XmlMappingSet<DateTime?>(xPath, input, delegate(DateTime? data) 
                {
                    return data.HasValue ? CH.ConvertToISO8601DateString(data.Value) : ""; 
                });
        }        
        #endregion // DateTime?
        #region Uri
        protected Uri XmlMappingGetToUri(string xPath)
        {
            return XmlMappingGet<Uri>(xPath,
                delegate(string input)
                {
                    if (input == null || input == "")
                        return null;
                    return new Uri(input);
                }
            );
        }

        protected bool XmlMappingSet(string xPath, Uri input)
        {
            return XmlMappingSet<Uri>(xPath, input, delegate(Uri inputData) { return inputData.ToString(); });
        }

        #endregion // Uri
        #region IPEndPoint
        protected IPEndPoint XmlMappingGetToIPEndPoint(string xPath)
        {
            return XmlMappingGet<IPEndPoint>(xPath,
                delegate(string input)
                {
                    if (input == null || input == "")
                        return null;
                    string[] items = input.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    return new IPEndPoint(IPAddress.Parse(items[0]), int.Parse(items[1]));
                }
            );
        }

        protected bool XmlMappingSet(string xPath, IPEndPoint input)
        {
            return XmlMappingSet<IPEndPoint>(xPath, input, delegate(IPEndPoint inputData) { return inputData.ToString(); });
        }        
        
        #endregion // IPEndPoint

        #region XmlMappingGet
        protected T XmlMappingGet<T>(string xPath, Converter<string, T> convert)
        {
            XmlNode node = XmlDataDoc.SelectSingleNode(xPath, NSM);

            if (node == null)
                return default(T);

            return XmlMappingGet<T>(node, convert);
        }

        protected T XmlMappingGet<T>(XmlNode node, Converter<string, T> convert)
        {
            string input = node.InnerText;
            return convert(input);
        }
        #endregion // XmlMappingGet
        #region XmlMappingSet
        protected bool XmlMappingSet<T>(string xPath, T input)
        {
            XmlNode node = XmlDataDoc.SelectSingleNode(xPath, NSM);
            return XmlMappingSet<T>(node, input, delegate(T inputData) { return inputData.ToString(); });
        }

        protected bool XmlMappingSet<T>(XmlNode node, T input)
        {
            return XmlMappingSet<T>(node, input, delegate(T inputData) { return inputData.ToString(); });
        }

        protected bool XmlMappingSet<T>(string xPath, T input, Converter<T, string> convert)
        {
            XmlNode toSet = this.XmlDataDoc.SelectSingleNode(xPath, NSM);
            return XmlMappingSet<T>(toSet, input, convert);
        }

        protected bool XmlMappingSet<T>(XmlNode toSet, T input, Converter<T, string> convert)
        {
            if (toSet == null)
                return false;
            toSet.InnerText = convert(input);
            return true;
        }
        #endregion // XmlMappingSet
    }
}
