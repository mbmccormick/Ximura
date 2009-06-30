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
using Ximura.Collections;
using Ximura.Collections.Data;
#endregion // using
namespace Ximura.Collections
{
    public abstract partial class StructBasedVertexArray<T>
    {
#if (DEBUG)
        private string DebugLockedItem
        {
            get
            {
                return LockedItems(mSlots);
            }
        }

        private string LockedItems(LockableWrapper<CollectionVertexStruct<T>>[][] data)
        {
            StringBuilder sb = new StringBuilder();
            //data.Where(s => s!=null)
            //    .ForIndex((i, s) => { if (s.v) sb.AppendLine(i.ToString()); });
            return sb.ToString();
        }
#endif
    }
}
