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

using Ximura;

using CH = Ximura.Common;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// The XimuraContentReference attribute uniquely identifies a content. This is useful
    /// as it allows the Ximura to identify different content implementations within the system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class XimuraContentReferenceAttribute : Attribute
    {
        #region Declarations
        private string mReferenceType;
        private string mReferenceField;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// The attribute constructor.
        /// </summary>
        /// <param name="ReferenceType">The Reference Type string.</param>
        /// <param name="ReferenceField">The Reference Field string.</param>
        public XimuraContentReferenceAttribute(string ReferenceType, string ReferenceField)
        {
            mReferenceType = ReferenceType;
            mReferenceField = ReferenceField;
        }
        #endregion // Constructors

        #region ReferenceType
        /// <summary>
        /// This is the Reference Type of the content.
        /// </summary>
        public string ReferenceType
        {
            get
            {
                return mReferenceType;
            }
        }
        #endregion // ReferenceType
        #region ReferenceField
        /// <summary>
        /// This is the Reference Field use for reference which field reflecting the Reference Value.
        /// </summary>
        public string ReferenceField
        {
            get
            {
                return mReferenceField;
            }
        }
        #endregion // ReferenceField
    }
}