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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    /// <summary>
    /// A vertex is the fundemental unit for building a relational graph. 
    /// The Vertex structure is built around an unsigned integer, but has additonal properties.
    /// </summary>
    public struct Vertex : IEquatable<Vertex>, IXimuraBinarySerializable
    {
        #region Static methods
        public static readonly Vertex Null;

        static Vertex()
        {
            Null = new Vertex((uint)0);
        }
        #endregion // Static methods

        #region Declarations
        internal uint data;
        #endregion // Declarations
        #region Constructors
        public Vertex(uint value)
        {
            data = value; 
        }

        public Vertex(byte[] blob, int offset)
        {
            data = BitConverter.ToUInt32(blob, offset);
        }
        #endregion // Constructors

        #region IEquatable<Edge> Members

        public bool Equals(Vertex other)
        {
            return data==other.data;
        }

        public static bool operator ==(Vertex a, Vertex b)
        {
            return a.data == b.data;
        }

        public static bool operator !=(Vertex a, Vertex b)
        {
            return !(a == b);
        }

        public static Edge operator + (Vertex a, Vertex b)
        {
            return new Edge(a, b);
        }

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is Vertex))
            {
                return false;
            }

            return Equals((Vertex)o);
        }

        #endregion

        #region IsNull
        /// <summary>
        /// The null vertex has a value of 0.
        /// </summary>
        public bool IsNull
        {
            get
            {
                return this == Null;
            }
        }
        #endregion // IsNull
        #region Value
        /// <summary>
        /// The unsigned integer value of the vertex.
        /// </summary>
        public uint Value
        {
            get
            {
                return data;
            }
        }
        #endregion // Value

        #region ToString()
        /// <summary>
        /// This override converts the vertex to a readable string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("V={0}", data);
        }
        #endregion // ToString()

        #region IXimuraBinarySerializable Members

        void IXimuraBinarySerializable.Read(System.IO.BinaryReader r)
        {
            data = r.ReadUInt32();
        }

        void IXimuraBinarySerializable.Write(System.IO.BinaryWriter w)
        {
            w.Write(data);
        }

        #endregion
    }
}
