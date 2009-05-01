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
using System.Text;
using System.IO;
using System.Security;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

using Ximura;
using Ximura.Server;
#endregion // using
namespace Ximura.Helper
{
    public sealed class Option<T> : IEquatable<Option<T>>
    {
        private readonly T value;
        private static readonly Option<T> none = new Option<T>();

        private Option() { }
        private Option(T value)
        {
            this.value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Option<T>)
                return Equals((Option<T>)obj);

            return false;
        }
        public bool Equals(Option<T> other)
        {
            if (IsNone)
                return other.IsNone;

            return EqualityComparer<T>.Default.Equals(value, other.value);
        }
        public override int GetHashCode()
        {
            if (IsNone)
                return 0;

            return EqualityComparer<T>.Default.GetHashCode(value);
        }

        public bool IsNone
        {
            get { return this == none; }
        }
        public bool IsSome
        {
            get { return !IsNone; }
        }

        public T Value
        {
            get
            {
                if (IsSome)
                    return value;

                throw new InvalidOperationException();
            }
        }

        public static Option<T> None
        {
            get { return none; }
        }

        public static Option<T> Some(T value)
        {
            return new Option<T>(value);
        }
    }

    public static class Option
    {
        public static Option<T> Some<T>(T value)
        {
            return Option<T>.Some(value);
        }
    }
}
