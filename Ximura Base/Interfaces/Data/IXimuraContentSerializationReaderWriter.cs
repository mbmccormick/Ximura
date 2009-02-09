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
using System.IO;
#endregion // using
namespace Ximura.Data.Serialization
{
    public interface IXimuraContentSerializationReaderWriter
    {
        Stream BaseStream { get; set; }
        void Close();
        void Flush();
        byte[] ReadBlob(bool verifyLength);
        byte[] ReadBlob(int MaxLength);
        byte[] ReadBlob();
        byte[] ReadBlob(bool verifyLength, int MaxLength);
        byte[] ReadBlob(Stream inStream, bool verifyLength, int MaxLength);
        byte[] ReadBlob(Stream inStream, bool verifyLength);
        byte[] ReadBlob(Stream inStream);
        bool ReadBool(Stream inStream);
        bool ReadBool();
        byte ReadByte(Stream inStream);
        byte ReadByte();
        byte[] ReadCompressedBlob();
        byte[] ReadCompressedBlob(Stream inStream, bool verifyLength, int MaxLength);
        byte[] ReadCompressedBlob(Stream inStream, bool verifyLength);
        byte[] ReadCompressedBlob(int MaxLength);
        byte[] ReadCompressedBlob(Stream inStream);
        byte[] ReadCompressedBlob(bool verifyLength, int MaxLength);
        byte[] ReadCompressedBlob(bool verifyLength);
        DateTime ReadDateTime(Stream inStream);
        DateTime ReadDateTime();
        Guid ReadGuid(Stream inStream);
        Guid ReadGuid();
        int ReadInt();
        int ReadInt(Stream inStream);
        long ReadLong();
        long ReadLong(Stream inStream);
        short ReadShort(Stream inStream);
        short ReadShort();
        string ReadString(Stream inStream, int MaxLength);
        string ReadString(Stream inStream);
        string ReadString(int MaxLength);
        string ReadString();
        uint ReadUint();
        uint ReadUint(Stream inStream);
        void Write(Stream outStream, string value);
        void Write(string value);
        void Write(Stream outStream, DateTime value);
        void Write(DateTime value);
        void Write(uint value);
        void Write(Stream outStream, uint value);
        void Write(Stream outStream, bool value);
        void Write(byte value);
        void Write(Stream outStream, byte value);
        void Write(Guid value);
        void Write(Stream outStream, Guid value);
        void Write(bool value);
        void Write(Stream outStream, int value);
        void Write(long value);
        void Write(Stream outStream, long value);
        void Write(short value);
        void Write(Stream outStream, short value);
        void Write(int value);
        void WriteBlob(byte[] value);
        void WriteBlob(Stream outStream, byte[] value);
        void WriteCompressedBlob(byte[] value);
        void WriteCompressedBlob(Stream outStream, byte[] value);
    }
}
