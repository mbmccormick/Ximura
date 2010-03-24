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
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This attribute is used to define search properties. Attributes do not need to be unique.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class CDSAttributeAttribute : System.Attribute
    {
        #region Private Field Declaration

        private string mName, mDescription, mLanguage;

        #endregion

        #region Class Constructors
        /// <summary>
        /// The XimuraAppCommandIDAttribute is used to uniquely identify the Ximura command.
        /// </summary>
        /// <param name="ID">A string representation of a GUID in the form 6364755B-97B9-4799-B8BC-3D98EB786C92</param>
        public CDSAttributeAttribute(string type) : this(type, "", "") { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID">A string representation of a GUID in the form 6364755B-97B9-4799-B8BC-3D98EB786C92</param>
        /// <param name="name">The friendly name of the command. 
        /// This will be used in the config file when parsing command set up paramters.</param>
        public CDSAttributeAttribute(string type, string subtype) : this(type, subtype, "") { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID">A string representation of a GUID in the form 6364755B-97B9-4799-B8BC-3D98EB786C92</param>
        /// <param name="name">The friendly name of the command. 
        /// This will be used in the config file when parsing command set up paramters.</param>
        /// <param name="description">A human readable description explaining what the command does.</param>
        public CDSAttributeAttribute(string type, string subtype, string language)
        {
            mName = type;
            mDescription = subtype;
            mLanguage = language;
        }
        #endregion

        #region Property Fields
        /// <summary>
        /// The Guid of the module
        /// </summary>
        public string Language
        {
            get { return mLanguage; }
        }
        /// <summary>
        /// The name of the module. This is used to retrieve app settings.
        /// </summary>
        public string Name
        {
            get { return mName; }
        }
        /// <summary>
        /// The friendly description of the module.
        /// </summary>
        public string Description
        {
            get { return mDescription; }
        }
        #endregion
    }
}
