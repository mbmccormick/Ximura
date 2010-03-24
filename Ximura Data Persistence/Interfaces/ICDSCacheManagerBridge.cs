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
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
using Ximura.Framework;


using Ximura.Framework;
#endregion // using
namespace Ximura.Data
{
    public interface ICDSCacheManagerBridge
    {
        bool BridgeRegister(ICDSCacheManager cacheManager);
        bool BridgeUnregister(ICDSCacheManager cacheManager);
        bool EntityTypeSupportsCaching(Type entityType);

        bool PollRegister(ICDSCacheManager cacheManager, int timeInSecs);
        bool PollUnregister(ICDSCacheManager cacheManager);

        XimuraContentCachePolicyAttribute CacheAttribute(Type entityType);
    }
}
