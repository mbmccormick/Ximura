#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// For more details see http://ximura.org
//
// Contributors:
//     Paul Stancer - initial implementation
// *******************************************************************************
#endregion
#region using
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Text;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    public interface ICollectionState
    {
        bool AllowMultipleEntries { get; }
        bool AllowNullValues { get; }

        int ContainScanMissThreshold { get; set; }
        bool ContainScanUnlocked { get; }
        void ContainScanUnlockedMiss();

        void CountDecrement();
        int CountIncrement(int value);
        int CountIncrement();

        bool DefaultTAdd();
        void DefaultTClear();
        bool DefaultTContains();
        int DefaultTCount { get; }
        bool DefaultTDelete();

        bool IsFixedSize { get; }

        int InitialCapacity { get; }
        int Count { get; }

        int Version { get; }
        bool VersionCompare(int version);
    }
}
