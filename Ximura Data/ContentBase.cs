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

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Data
{
    public abstract class ContentBase: IXimuraContent
    {

        #region IXimuraContent Members

        public Guid IDType
        {
            get { throw new NotImplementedException(); }
        }

        public Guid IDContent
        {
            get;
            set;
        }

        public Guid IDVersion
        {
            get;
            set;
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
