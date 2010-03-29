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
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using System.Security.Cryptography;
using System.Globalization;
using System.Runtime.Serialization;

using Ximura;

using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
using Ximura.Data;
using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    ///// <summary>
    ///// This class is used to reference the CDS directly within the CDS itself. It allows Persistence Managers to call other Persistence Managers
    ///// from the same request without the need to call the dispatcher.
    ///// </summary>
    //public class CDSHelperDirect : CDSHelper
    //{
    //    #region Constructors
    //    public CDSHelperDirect()
    //        : this(null)
    //    {
    //    }
    //    /// <summary>
    //    /// This is the main constructor.
    //    /// </summary>
    //    /// <param name="session">The session object to wrap.</param>
    //    public CDSHelperDirect(IXimuraSessionRQ session)
    //        : base(session)
    //    {
    //    }
    //    #endregion // Constructors

    //    #region ResolveReference/ResolveReference<T>
    //    public CDSResponse ResolveReference<T>(string refType, string refValue, out Guid? cid, out Guid? vid) where T : Content
    //    {
    //        return TranslateResponseCode(Execute<T>(CDSData.Get(CDSAction.ResolveReference, refType, refValue), out cid, out vid));
    //    }

    //    public CDSResponse ResolveReference(Type objectType, string refType, string refValue, out Guid? cid, out Guid? vid)
    //    {
    //        return TranslateResponseCode(Execute(objectType, CDSData.Get(CDSAction.ResolveReference, refType, refValue), out cid, out vid));
    //    }

    //    #endregion // VersionCheck

    //}
}
