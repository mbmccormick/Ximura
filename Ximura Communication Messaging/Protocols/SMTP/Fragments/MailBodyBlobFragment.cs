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
using System.Threading;
using System.Text;
using System.Timers;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

using System.Diagnostics;

using Ximura;

using Ximura.Data;

using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Communication
{
    public class MailBodyBlobFragment : MessageFragment, IXimuraMessageFileStreamLoad
    {
        #region Declarations
        protected Stream fileStream;
        /// <summary>
        /// This is the terminator for a SMTP mail message, namely CRLF.CRLF
        /// </summary>
        protected readonly byte[] mSMTPMailTerminator = new byte[] { 13, 10, 46, 13, 10 };
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MailBodyBlobFragment()
            : base()
        {
        }
        #endregion
        #region Reset()
        /// <summary>
        /// This method resets the fragment so that it can be reused.
        /// </summary>
        public override void Reset()
        {
            //Note we only set the stream to null. We don't close it as it is not owned by the fragment
            //and may be used by other fragments.
            fileStream = null;
            base.Reset();
        }
        #endregion // Reset()

        #region TerminatorAllowFolding
        /// <summary>
        /// This method specifies whether the message allows folding, i.e. does not
        /// terminate when the CRLF is followed by either a space or a tab character.
        /// The default is not to allow folding.
        /// </summary>
        protected virtual bool TerminatorAllowFolding { get { return false; } }
        #endregion // TerminatorAllowFolding
        #region TerminationType
        /// <summary>
        /// This is the fragment termination type.
        /// </summary>
        protected override FragmentTerminationType TerminationType{get{return FragmentTerminationType.Terminator;}}
        #endregion // TerminationType


        #region WriteProcessByteLength (byte[] buffer, int offset, int count)
        /// <summary>
        /// This override writes the message bytes to the data stream instead of keeping it in a memory buffer.
        /// </summary>
        /// <param name="buffer">The byte buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>Returns the number of bytes written from the buffer.</returns>
        protected override int WriteProcessByteLength(byte[] buffer, int offset, int count)
        {
            fileStream.Write(buffer, offset, count);

            return count;
        }
        #endregion // WriteProcessByteLength (byte[] buffer, int offset, int count)

        #region Load(long maxSize, Stream writeStream)
        /// <summary>
        /// This method initializes the fragment with the maximum size and the output stream.
        /// </summary>
        /// <param name="maxSize">The maximum size for the message.</param>
        /// <param name="writeStream">The output stream to write to.</param>
        public virtual void Load(long maxSize, Stream writeStream)
        {
            fileStream = writeStream;
            Load(maxSize);
        }
        #endregion // Load(long maxSize, Stream writeStream)
    }
}
