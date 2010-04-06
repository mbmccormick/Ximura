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
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Text;

using Ximura;
using Ximura.Data;
using Ximura.Framework;
using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Data
{
    public partial class PersistenceManagerCDSState<CONT, DCONT, CONF>
    {
        #region CDSAttributesRetrieve(IXimuraContent data, TypeConverter conv)
        /// <summary>
        /// This method retrieves a set of CDSAttribute from a class.
        /// </summary>
        /// <param name="data">The class to examine.</param>
        /// <param name="conv">A converter function to turn the value in to a string.</param>
        /// <returns>Returns an enumerable collection of attributes and values.</returns>
        protected virtual IEnumerable<KeyValuePair<CDSAttributeAttribute, string>> CDSAttributesRetrieve(IXimuraContent data, TypeConverter conv)
        {
            List<KeyValuePair<PropertyInfo, CDSAttributeAttribute>> attrList =
                AH.GetPropertyAttributes<CDSAttributeAttribute>(data.GetType());

            foreach (KeyValuePair<PropertyInfo, CDSAttributeAttribute> reference in attrList)
            {
                PropertyInfo pi = reference.Key;

                if (pi.PropertyType == typeof(string))
                {
                    yield return new KeyValuePair<CDSAttributeAttribute, string>(
                        reference.Value, pi.GetValue(data, null) as string);
                }
                else if (pi.PropertyType == typeof(IEnumerable<string>))
                {
                    IEnumerable<string> enumerator = pi.GetValue(data, null) as IEnumerable<string>;
                    foreach (string value in enumerator)
                        yield return new KeyValuePair<CDSAttributeAttribute, string>(
                            reference.Value, value);
                }
                else if (conv != null && conv.CanConvertFrom(pi.PropertyType))
                {
                    yield return new KeyValuePair<CDSAttributeAttribute, string>(
                        reference.Value, conv.ConvertToString(pi.GetValue(data, null)));
                }
            }
        }
        #endregion
        #region CDSReferencesRetrieve(IXimuraContent data, TypeConverter conv)
        /// <summary>
        /// This method retrieves a set of reference attributes from a class.
        /// </summary>
        /// <param name="data">The class to examine.</param>
        /// <param name="conv">A converter function to turn the value in to a string.</param>
        /// <returns>Returns an enumerable collection of attributes and values.</returns>
        protected virtual IEnumerable<KeyValuePair<CDSReferenceAttribute, string>> CDSReferencesRetrieve(IXimuraContent data, TypeConverter conv)
        {
            List<KeyValuePair<PropertyInfo, CDSReferenceAttribute>> attrList =
                AH.GetPropertyAttributes<CDSReferenceAttribute>(data.GetType());

            foreach (KeyValuePair<PropertyInfo, CDSReferenceAttribute> reference in attrList)
            {
                PropertyInfo pi = reference.Key;

                if (pi.PropertyType != typeof(string) &&
                    (conv == null || !conv.CanConvertFrom(pi.PropertyType)))
                    continue;

                string value;

                if (pi.PropertyType == typeof(string))
                    value = pi.GetValue(data, null) as string;
                else
                    value = conv.ConvertToString(pi.GetValue(data, null));

                yield return new KeyValuePair<CDSReferenceAttribute, string>(reference.Value, value);
            }
        }
        #endregion 
    }
}
