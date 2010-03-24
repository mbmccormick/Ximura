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
using System.Text;
using System.IO;
using System.Security;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

using Ximura;
using Ximura.Framework;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This struct sets the appropriate bits.
    /// </summary>
    public struct BitMaskInt32
    {
        int bitPosition;
        int bitMask, bitMaskNeg;

        public BitMaskInt32(int bitPosition)
        {
            if (bitPosition < 0 || bitPosition >= 23)
                throw new ArgumentOutOfRangeException("", "Bit position is out of range. Valid values are 0-22");

            this.bitPosition = bitPosition;
            bitMask = 1 << bitPosition;
            bitMaskNeg = 0x7FFFFFFF ^ bitMask;
        }

        public int BitPosition { get { return bitPosition; } }

        public bool BitCheck(int value)
        {
            return (value & bitMask) > 0;
        }

        public int BitSet(int data)
        {
            return data | bitMask;
        }

        public int BitUnset(int data)
        {
            return data & bitMaskNeg;
        }


    }
}
