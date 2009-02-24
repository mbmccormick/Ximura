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
namespace Ximura.Persistence
{
    /// <summary>
    /// This attribute is used to define unique refernce properties for the entity
    /// that can be used to retrieve it from the CDS store..
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class CDSReferenceAttribute : System.Attribute
    {
        #region Private Field Declaration

        private string mName;

        #endregion

        #region Class Constructors
        /// <summary>
        /// The XimuraAppCommandIDAttribute is used to uniquely identify the Ximura command.
        /// </summary>
        /// <param name="ID">A string representation of a GUID in the form 6364755B-97B9-4799-B8BC-3D98EB786C92</param>
        public CDSReferenceAttribute(string type)
        {
            mName = type;
        }
        #endregion

        #region Property Fields
        /// <summary>
        /// The name of the module. This is used to retrieve app settings.
        /// </summary>
        public string Name
        {
            get { return mName; }
        }
        #endregion
    }
}
