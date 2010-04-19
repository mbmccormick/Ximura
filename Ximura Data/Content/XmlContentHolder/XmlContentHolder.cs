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
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Text;
using System.Xml.XPath;
using System.Runtime.Serialization.Formatters.Binary;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Data
{
    public class XmlContentHolder<T> : ContentHolder<T>, IXPathNavigable
        where T : class, IXPathNavigable
    {


        #region IXPathNavigable Members

        public XPathNavigator CreateNavigator()
        {
            return mData.CreateNavigator();
        }

        #endregion
    }
}
