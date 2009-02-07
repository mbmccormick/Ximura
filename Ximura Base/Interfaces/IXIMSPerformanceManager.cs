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
using System.Collections.Generic;
using System.Text; 
#endregion
namespace XIMS.Applications
{
    /// <summary>
    /// This interface should be used by all services providing performance services
    /// </summary>
    public interface IXIMSPerformanceManager
    {
        void PerformanceCounterCollectionRegister(IXIMSPerformanceCounterCollection collection);

        void PerformanceCounterCollectionUnregister(IXIMSPerformanceCounterCollection collection);
    }
}
