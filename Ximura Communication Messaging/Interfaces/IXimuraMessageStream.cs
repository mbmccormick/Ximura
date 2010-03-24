﻿#region Copyright
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
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections;
using System.Text;

using Ximura;

using Ximura.Data;
#endregion
namespace Ximura.Communication
{
    /// <summary>
    /// This interface is used to interface between the protocol and the message
    /// </summary>
    public interface IXimuraMessageStream : IXimuraMessageStreamLoad, IXimuraMessage
    {
        //
        // Summary:
        //     When overridden in a derived class, gets a value indicating whether the current
        //     stream supports seeking.
        //
        // Returns:
        //     true if the stream supports seeking; otherwise, false.
        bool CanSeek { get; }
        //
        // Summary:
        //     Gets a value that determines whether the current stream can time out.
        //
        // Returns:
        //     A value that determines whether the current stream can time out.
        bool CanTimeout { get; }

        //
        // Summary:
        //     Gets or sets a value that determines how long the stream will attempt to
        //     read before timing out.
        //
        // Returns:
        //     A value that determines how long the stream will attempt to read before timing
        //     out.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.IO.Stream.ReadTimeout method always throws an System.InvalidOperationException.
        int ReadTimeout { get; set; }
        //
        // Summary:
        //     Gets or sets a value that determines how long the stream will attempt to
        //     write before timing out.
        //
        // Returns:
        //     A value that determines how long the stream will attempt to write before
        //     timing out.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The System.IO.Stream.WriteTimeout method always throws an System.InvalidOperationException.
        int WriteTimeout { get; set; }


        //
        // Summary:
        //     Closes the current stream and releases any resources (such as sockets and
        //     file handles) associated with the current stream.
        void Close();


        //
        // Summary:
        //     When overridden in a derived class, clears all buffers for this stream and
        //     causes any buffered data to be written to the underlying device.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        void Flush();

        //
        // Summary:
        //     When overridden in a derived class, sets the position within the current
        //     stream.
        //
        // Parameters:
        //   offset:
        //     A byte offset relative to the origin parameter.
        //
        //   origin:
        //     A value of type System.IO.SeekOrigin indicating the reference point used
        //     to obtain the new position.
        //
        // Returns:
        //     The new position within the current stream.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.NotSupportedException:
        //     The stream does not support seeking, such as if the stream is constructed
        //     from a pipe or console output.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        long Seek(long offset, SeekOrigin origin);
        //
        // Summary:
        //     When overridden in a derived class, sets the length of the current stream.
        //
        // Parameters:
        //   value:
        //     The desired length of the current stream in bytes.
        //
        // Exceptions:
        //   System.NotSupportedException:
        //     The stream does not support both writing and seeking, such as if the stream
        //     is constructed from a pipe or console output.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        void SetLength(long value);
    }
}
