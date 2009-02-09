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
using System.Reflection;
using System.IO;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This attribute is used to define the default content for DataContent.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class XimuraDataContentDefaultAttribute : XimuraSchemaAttribute
    {
        #region Declarations
        private bool mAutoLoad;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="resPath">The resource path of the default data for the DataContent.</param>
        public XimuraDataContentDefaultAttribute(string resPath)
            : this(resPath, false)
        {
        }
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="resPath">The resource path of the default data for the DataContent.</param>
        /// <param name="autoLoad">This property specifies if the data should load the default data automatically
        /// when the object is first created.</param>
        public XimuraDataContentDefaultAttribute(string resPath, bool autoLoad)
            : base(resPath)
        {
            mAutoLoad = autoLoad;
        }
        #endregion // Constructor


        #region AutoLoad
        /// <summary>
        /// This property informs the content on whether it should autoload the default content when the 
        /// object is first created.
        /// </summary>
        public bool AutoLoad
        {
            get { return mAutoLoad; }
        }
        #endregion // AutoLoad



    }
}
