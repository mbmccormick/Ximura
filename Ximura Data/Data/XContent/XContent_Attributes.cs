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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using Ximura;
using Ximura.Data;
using Ximura.Framework;
using Ximura.Helper;
using AH = Ximura.Helper.AttributeHelper;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This is the base XML content class using the new .NET 3.5 Linq XML classes.
    /// </summary>
    public partial class XContent : Content
    {
        #region Declarations
        /// <summary>
        /// This is the Schema attribute.
        /// </summary>
        protected XimuraDataContentSchemaAttribute attrSchemaPrimary;
        /// <summary>
        /// This is the default data attribute.
        /// </summary>
        protected XimuraDataContentDefaultAttribute attrDefaultData;
        /// <summary>
        /// This is the collection of reference schemas for the object.
        /// </summary>
        protected XimuraDataContentSchemaReferenceAttribute[] attrsSchemaReference;
        #endregion // Declarations

        #region SetAttributes()
        /// <summary>
        /// This protected method sets the mapping attributes for the content.
        /// </summary>
        protected override void SetAttributes()
        {
            base.SetAttributes();

            attrDefaultData = AH.GetAttribute<XimuraDataContentDefaultAttribute>(GetType());

            attrSchemaPrimary = AH.GetAttribute<XimuraDataContentSchemaAttribute>(GetType());

            attrsSchemaReference = AH.GetAttributes<XimuraDataContentSchemaReferenceAttribute>(GetType());
        }
        #endregion
    }
}
