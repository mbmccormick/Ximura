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
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections;
using System.Text;

using Ximura;
using Ximura.Helper;
using Ximura.Data;
#endregion
namespace Ximura.Data
{

    public interface IXimuraMessageStreamAsync : IXimuraMessageStream
    {
        //
        // Summary:
        //     Waits for the pending asynchronous read to complete.
        //
        // Parameters:
        //   asyncResult:
        //     The reference to the pending asynchronous request to finish.
        //
        // Returns:
        //     The number of bytes read from the stream, between zero (0) and the number
        //     of bytes you requested. Streams return zero (0) only at the end of the stream,
        //     otherwise, they should block until at least one byte is available.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     asyncResult did not originate from a System.IO.Stream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)
        //     method on the current stream.
        //
        //   System.ArgumentNullException:
        //     asyncResult is null.
        int EndRead(IAsyncResult asyncResult);
        //
        // Summary:
        //     Ends an asynchronous write operation.
        //
        // Parameters:
        //   asyncResult:
        //     A reference to the outstanding asynchronous I/O request.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     asyncResult is null.
        //
        //   System.ArgumentException:
        //     asyncResult did not originate from a System.IO.Stream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)
        //     method on the current stream.
        void EndWrite(IAsyncResult asyncResult);

        // Summary:
        //     Begins an asynchronous read operation.
        //
        // Parameters:
        //   offset:
        //     The byte offset in buffer at which to begin writing data read from the stream.
        //
        //   count:
        //     The maximum number of bytes to read.
        //
        //   buffer:
        //     The buffer to read the data into.
        //
        //   callback:
        //     An optional asynchronous callback, to be called when the read is complete.
        //
        //   state:
        //     A user-provided object that distinguishes this particular asynchronous read
        //     request from other requests.
        //
        // Returns:
        //     An System.IAsyncResult that represents the asynchronous read, which could
        //     still be pending.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     Attempted an asynchronous read past the end of the stream, or a disk error
        //     occurs.
        //
        //   System.NotSupportedException:
        //     The current Stream implementation does not support the read operation.
        //
        //   System.ArgumentException:
        //     One or more of the arguments is invalid.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state);
        //
        // Summary:
        //     Begins an asynchronous write operation.
        //
        // Parameters:
        //   offset:
        //     The byte offset in buffer from which to begin writing.
        //
        //   count:
        //     The maximum number of bytes to write.
        //
        //   buffer:
        //     The buffer to write data from.
        //
        //   callback:
        //     An optional asynchronous callback, to be called when the write is complete.
        //
        //   state:
        //     A user-provided object that distinguishes this particular asynchronous write
        //     request from other requests.
        //
        // Returns:
        //     An IAsyncResult that represents the asynchronous write, which could still
        //     be pending.
        //
        // Exceptions:
        //   System.NotSupportedException:
        //     The current Stream implementation does not support the write operation.
        //
        //   System.IO.IOException:
        //     Attempted an asynchronous write past the end of the stream, or a disk error
        //     occurs.
        //
        //   System.ArgumentException:
        //     One or more of the arguments is invalid.
        //
        //   System.ObjectDisposedException:
        //     Methods were called after the stream was closed.
        IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state);
    }
}
