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
using System.Collections.Generic;
using System.Text;
#endregion
namespace Ximura
{
    /// <summary>
    /// This interface should be used by all services providing performance services
    /// </summary>
    public interface IXimuraPerformanceManager
    {
        /// <summary>
        /// This method registers a performance counter will the application.
        /// </summary>
        /// <param name="collection">The counter collection to register.</param>
        void PerformanceCounterCollectionRegister(IXimuraPerformanceCounterCollection collection);
        /// <summary>
        /// This method removes a performance counter collection from the application.
        /// </summary>
        /// <param name="collection"></param>
        void PerformanceCounterCollectionUnregister(IXimuraPerformanceCounterCollection collection);
    }
}
