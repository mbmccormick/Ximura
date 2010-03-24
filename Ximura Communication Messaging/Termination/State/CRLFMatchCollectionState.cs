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
﻿#region using
using System;
using System.Linq;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;

using Ximura;

using CH = Ximura.Common;
#endregion
namespace Ximura.Communication
{
    /// <summary>
    /// This class matches on CRLF
    /// </summary>
    public class CRLFMatchCollectionState : MatchCollectionState<byte, byte>
    {
        #region Declarations
        private bool mAllowFolding;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public CRLFMatchCollectionState()
            : this(false)
        {
        }
        /// <summary>
        /// This is the extended constructor.
        /// </summary>
        /// <param name="boundary"></param>
        public CRLFMatchCollectionState(bool AllowFolding)
        {
            mAllowFolding = AllowFolding;
        }
        #endregion // Constructors

        #region GetEnumerator()
        /// <summary>
        /// This method returns a new MimeMatchCollection enumerator.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator<MatchTerminator<byte, byte>> GetEnumerator()
        {
            return new CRLFFoldingMatchCollection(mAllowFolding);
        }
        #endregion // GetEnumerator()

        #region Class --> MimeMatchCollection
        /// <summary>
        /// The class contains the match terminators.
        /// </summary>
        public class CRLFFoldingMatchCollection : MatchCollection<byte, byte>
        {
            #region Static constructor and declarations
            private static readonly CRLFTerminatorWithReset mTerminatorCRLF;
            private static readonly MatchExceptionTerminator<byte, byte> mTerminatorWHTSPException;

            static CRLFFoldingMatchCollection()
            {
                mTerminatorCRLF = new CRLFTerminatorWithReset();
                mTerminatorWHTSPException = new MatchExceptionTerminator<byte, byte>(
                    new byte[] { 9, 32 });
            }
            #endregion // Static constructor and declarations
            #region Declarations
            private bool disposed = false;
            private bool mAllowFolding;
            #endregion // Declarations
            #region Internal Constructor
            /// <summary>
            /// This constructor initializes the collection with the boundary.
            /// </summary>
            /// <param name="boundary"></param>
            internal CRLFFoldingMatchCollection(bool AllowFolding)
                : base(null)
            {
                mAllowFolding = AllowFolding;
            }
            #endregion // Constrcutor
            #region Dispose(bool disposing)
            /// <summary>
            /// This method disposes the collection.
            /// </summary>
            /// <param name="disposing"></param>
            public override void Dispose(bool disposing)
            {
                if (!disposed && disposing)
                {
                    base.Dispose(disposing);
                }
                disposed = true;
            }
            #endregion // Dispose(bool disposing)

            #region this[int index]
            /// <summary>
            /// This method returns the specified item for the collection. You should override this indexer.
            /// </summary>
            /// <param name="index">The position index.</param>
            /// <returns></returns>
            public override MatchTerminator<byte, byte> this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: // "CRLF"(required)
                            return mTerminatorCRLF;
                        case 1: // whitespace exception
                            return mTerminatorWHTSPException;
                        default:
                            throw new ArgumentOutOfRangeException("");
                    }
                }
            }
            #endregion // this[int index]
            #region Count
            /// <summary>
            /// This property returns the number of items in the collection. You should override this property.
            /// </summary>
            public override int Count
            {
                get { return mAllowFolding ? 2 : 1; }
            }
            #endregion // Count
        }
        #endregion // Class --> MimeMatchCollection

        #region Class --> CRLFTerminatorWithReset
        /// <summary>
        /// This termination class specifically matches on CRLF and resets the match buffer when
        /// a termination character has been found.
        /// </summary>
        public class CRLFTerminatorWithReset : MatchSequenceTerminator<byte, byte>
        {
            #region Constructor
            public CRLFTerminatorWithReset()
                : base(new byte[] { 13, 10 }, true, null, (x, y, z, l) => ((l+ x.Length) == 2 && z == (byte)0x0a))
            {

            }
            #endregion // Constructor

            protected override MatchTerminatorStatus Validate(byte item, MatchTerminatorResult currentResult)
            {
                bool result = item.Equals(CurrentTerminator.Current);

                if (!result)
                    return MatchTerminatorStatus.Fail;

                try
                {
                    return CurrentTerminator.MoveNext() ?
                        MatchTerminatorStatus.SuccessPartial :
                        (currentResult.Length == 1 ? MatchTerminatorStatus.SuccessReset : MatchTerminatorStatus.Success);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion // CRLFTerminatorWithReset
    }
}
