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
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Data
{
    /// <summary>
    /// This fragment is used for messages whose primary storage is the file system.
    /// </summary>
    public class FileBasedStorageMessageFragment : MessageFragment
    {
        #region Declarations
        /// <summary>
        /// This is the file storage stream for reading and writing.
        /// </summary>
        protected Stream fileStorageStream = null;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public FileBasedStorageMessageFragment()
            : base()
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This method closes the stream if it is still open.
        /// </summary>
        public override void Reset()
        {
            if (fileStorageStream != null)
            {
                fileStorageStream.Close();
                fileStorageStream = null;
            }
            base.Reset();
        }
        #endregion // Reset()

        #region Load(int maxSize, Stream inStream)
        /// <summary>
        /// This override is to allow the base stream to be set.
        /// </summary>
        /// <param name="maxSize"></param>
        /// <param name="inStream"></param>
        public void Load(int maxSize, Stream inStream)
        {
            fileStorageStream = inStream;
            Load(maxSize);
        }
        #endregion // Load(int maxSize, Stream inStream)

    }
}
