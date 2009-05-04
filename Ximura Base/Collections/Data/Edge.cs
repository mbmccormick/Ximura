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
    /// The edge structure records the one-way relationship between the vertexes.
    /// </summary>
    public struct Edge : IEquatable<Edge>, IXimuraBinarySerializable
    {
        #region Static methods
        public static readonly Edge Null;

        static Edge()
        {
            Null = new Edge((ulong)0);
        }
        #endregion // Static methods

        #region Declarations
        internal ulong data;
        #endregion // Declarations

        #region Constructor
        public Edge(ulong value)
        {
            data = value;
        }

        public Edge(Vertex forward, Vertex back)
        {
            if (forward.data == back.data)
                throw new ArgumentException("The vertexes are identical.");

            data = ((ulong)forward.data) << 32 | (ulong)back.data;
        }

        public Edge(uint forward, uint back)
        {
            if (forward == back)
                throw new ArgumentException("The vertexes are identical.");

            data = ((ulong)forward) << 32 | (ulong)back;
        }

        #endregion // Constructor

        #region IEquatable<Edge> Members

        public bool Equals(Edge other)
        {
            return data == other.data;
        }

        public static bool operator ==(Edge a, Edge b)
        {
            return a.data == b.data;

        }

        public static bool operator !=(Edge a, Edge b)
        {
            return !(a == b);
        }

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is Edge))
            {
                return false;
            }

            return Equals((Edge)o);
        }

        #endregion

        public static bool operator > (Edge a, Edge b)
        {
            if (a == b)
                return false;

            return a.ForwardInternal == b.BackInternal && a.BackInternal != b.ForwardInternal;
        }

        /// <summary>
        /// The less than operator returns true if the two edges are joined on the 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator < (Edge a, Edge b)
        {
            if (a == b)
                return false;

            return a.BackInternal == b.ForwardInternal && a.ForwardInternal != b.BackInternal;
        }
        /// <summary>
        /// The not operator reverses the direction of the edge.
        /// </summary>
        /// <param name="a">The edge to reverse.</param>
        /// <returns>returns an edge structure.</returns>
        public static Edge operator ! (Edge a)
        {
            return a.Reverse;
        }

        public static implicit operator ulong(Edge e)
        {
            return e.data;
        }


        public ulong Value
        {
            get
            {
                return data;
            }
        }

        public Edge Reverse
        {
            get
            {
                return new Edge(data << 32 | data >> 32);
            }
        }

        internal uint BackInternal { get { return (uint)data; } }
        internal uint ForwardInternal { get { return (uint)(data>>32); } }

        public Vertex Forward
        {
            get
            {
                return new Vertex((uint)(data >> 32));
            }
        }

        public Vertex Back
        {
            get
            {
                return new Vertex((uint)data);
            }
        }

        public bool Contains(Vertex v)
        {
            return ForwardInternal == v.data || BackInternal == v.data;
        }

        //public override unsafe int GetHashCode()
        //{
        //    uint hashCode = (uint)data;
        //    hashCode ^= (uint)(data >> 32);

        //    return *(((int*)hashCode));
        //}

        public override string ToString()
        {
            return string.Format("E={0}<{1}", ForwardInternal, BackInternal);
        }

        #region IBinarySerializable Members

        public void Read(System.IO.BinaryReader r)
        {
            data = r.ReadUInt64();
        }

        public void Write(System.IO.BinaryWriter w)
        {
            w.Write(data);
        }

        #endregion
    }
}
