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
using System.Runtime.Serialization;
using Ximura;
using RH = Ximura.Reflection;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This attribute can be used to specify a specific formatter 
    /// for a content class serialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class XimuraContentSerializationAttribute : Attribute
    {
        #region Declarations
        private string mFormatter;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor for the class.
        /// </summary>
        /// <param name="serializer">The serializer formatter.</param>
        public XimuraContentSerializationAttribute(string formatter)
        {
            mFormatter = formatter;
        }
        #endregion // Constructor

        #region FormatterName
        /// <summary>
        /// This is the name of the formatter.
        /// </summary>
        public string FormatterName
        {
            get { return mFormatter; }
        }
        #endregion // FormatterName
        #region Formatter
        /// <summary>
        /// This is the formatter for the specific attribute. This formatter
        /// is taken from the flywight threadpool for formatters.
        /// </summary>
        public IXimuraFormatter Formatter
        {
            get
            {
                return RH.CreateObjectFromType(mFormatter) as IXimuraFormatter;
            }
        }
        #endregion // Formatter
    }
}