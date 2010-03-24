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

#endregion // using
namespace Ximura.Collections.Data
{
    public interface IVertexWindow<T>
    {
        void ItemInsert();
        void ItemSetNext();

        T Value { get; }

        bool MoveUp();
        void ItemRemoveAndUnlock();

        int ScanAndLock();
        bool ScanProcess();
        bool ScanItemMatch{get;}

        void Snip();
        void Unlock();

        int HashID { get; }

        bool NextIsSentinel { get; }
        bool CurrIsTerminator { get; }

        T NextData { get; }
    }
}
