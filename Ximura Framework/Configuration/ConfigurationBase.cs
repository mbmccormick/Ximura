﻿#region Copyright
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
using System.Linq;
using System.Drawing;
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

using Ximura;
using Ximura.Data;


using CH = Ximura.Common;
using RH = Ximura.Reflection;
using Ximura.Framework;

#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This class is the base class for the configuration functionality.
    /// </summary>
    public class ConfigurationBase : XimuraCore
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public ConfigurationBase() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public ConfigurationBase(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion
    }
}
