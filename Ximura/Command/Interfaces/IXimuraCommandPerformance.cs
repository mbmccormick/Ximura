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
﻿#region using
using System;
using System.Collections;
using System.Configuration;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;

using Ximura.Server;


#endregion // using
namespace Ximura
{
    public interface IXimuraCommandPerformance : IXimuraPerformanceCounterCollection, IXimuraPerformance
    {
        void RequestStart(Guid ID);
        void RequestEnd(Guid ID);

        void RequestCallbackStart(Guid ID);
        void RequestCallbackEnd(Guid ID);


    }
}