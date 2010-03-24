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
using System.IO;
using System.Text;

using Ximura;

#endregion // using
namespace Ximura.Collections.Data
{
    public struct VertexReference : IEquatable<VertexReference>, IXimuraBinarySerializable
    {
        public string Type;
        public string SubType;
        public byte[] Reference;

        public VertexReference(string Type, string SubType, byte[] Reference)
        {
            this.Type = Type;
            this.SubType = SubType;
            this.Reference = Reference;
        }

        #region IBinarySerializable Members

        public void Read(BinaryReader r)
        {
            Type = ReadString(r);

            SubType = ReadString(r); 

            if (!r.ReadBoolean())
            {
                Reference = null;
                return;
            }

            int length = r.ReadInt32();
            Reference = r.ReadBytes(length);
        }

        public static string ReadString(BinaryReader r)
        {
            if (!r.ReadBoolean())
                return null;
            int len = r.ReadInt32();
            byte[] blob = r.ReadBytes(len);
            return Encoding.UTF8.GetString(blob);
        }

        public void Write(BinaryWriter w)
        {
            WriteString(w,Type);

            WriteString(w, SubType);

            w.Write(Reference == null);
            if (Reference != null)
            {
                w.Write(Reference.Length);
                w.Write(Reference);
            }
        }

        public static void WriteString(BinaryWriter w, string data)
        {
            w.Write(data != null);

            if (data == null)
                return;

            byte[] blob = Encoding.UTF8.GetBytes(data);
            w.Write(blob.Length);
            if (blob.Length > 0)
                w.Write(blob);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("[{0}/{1}={2}]", Type, SubType, Convert.ToBase64String(Reference));
        }

        #region IEquatable<VertexReference> Members

        public bool Equals(VertexReference other)
        {
            return this == other; 
        }

        public static bool operator ==(VertexReference a, VertexReference b)
        {
            if (a.Type != b.Type)
                return false;
            if (a.SubType != b.SubType)
                return false;
            if (a.Reference != b.Reference)
                return false;

            return true;
        }

        public static bool operator !=(VertexReference a, VertexReference b)
        {
            return !(a == b);
        }

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is VertexReference))
            {
                return false;
            }

            return Equals((Vertex)o);
        }

        #endregion

    }
}
