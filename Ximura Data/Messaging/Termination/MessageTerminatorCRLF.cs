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
    #region Old code
    //#region TerminatorCRLF
    ///// <summary>
    ///// This is the default terminator for the MessageCRLFFragment message class.
    ///// </summary>
    //public class MessageTerminatorCRLFFolding : MessageTerminatorBase
    //{
    //    #region Declarations
    //    private readonly byte[] mConstTerminator = new byte[] { 13, 10 };
    //    #endregion // Declarations
    //    #region Constructor
    //    /// <summary>
    //    /// The default constructor
    //    /// </summary>
    //    public MessageTerminatorCRLFFolding()
    //        : base()
    //    {
    //    }
    //    #endregion

    //    #region IsTerminator
    //    /// <summary>
    //    /// This property is set to true whenever the byte array has reached predetermined termination characteristics.
    //    /// These characteristics will differ depending on the termination type. 
    //    /// </summary>
    //    public override bool IsTerminator
    //    {
    //        get { return Matched && Length == 2; }
    //    }
    //    #endregion // IsTerminator

    //    #region ValidateBoundaryPartCondition
    //    /// <summary>
    //    /// This method validates the boundary part conditions for CRLF folding. Specifically, 
    //    /// it rejects the boundary folding when the part is a single CRLF set of characters.
    //    /// </summary>
    //    /// <param name="buffer">The buffer.</param>
    //    /// <param name="offset">The offset.</param>
    //    /// <param name="count">The count.</param>
    //    /// <param name="carryOver">The carry over bytes.</param>
    //    /// <param name="bytesRead">The bytes read.</param>
    //    /// <returns>Returns true if the folding is acceptable.</returns>
    //    protected override bool ValidateBoundaryPartCondition(byte[] buffer, int offset, int count, int carryOver, int bytesRead)
    //    {
    //        if (!AllowFolding)
    //            return false;

    //        if (bytesRead == 2)
    //            return !(buffer[offset]==13 && buffer[offset+1]==10);

    //        return true;
    //    }
    //    #endregion

    //    #region Reset()
    //    /// <summary>
    //    /// This method resets the terminator.
    //    /// </summary>
    //    public override void Reset()
    //    {
    //        base.Reset();
    //        Initialized = true;
    //    }
    //    #endregion // Reset()
    //    #region AllowFolding
    //    /// <summary>
    //    /// This property specifies whether the message allows folding, that is a CRLF followed by a TAB or SPC character
    //    /// is not a termination. Otherwise the terminator will signal a match on CRLF.
    //    /// </summary>
    //    public virtual bool AllowFolding
    //    {
    //        get { return true; }
    //    }
    //    #endregion // AllowFolding

    //    #region TerminatorHolderGetCurrent()
    //    /// <summary>
    //    /// This override gets the current termination holder.
    //    /// </summary>
    //    /// <returns>Returns a TerminationHolder structure.</returns>
    //    protected override TerminatorHolder? TerminatorHolderGetCurrent()
    //    {
    //        if (CurrentSection == -1)
    //        {
    //            CurrentSection = 0;
    //            return TerminatorHolder.CRLF;
    //        }

    //        if (AllowFolding && ((CurrentSection % 2) == 1))
    //            return TerminatorHolder.LWSPEx;

    //        return TerminatorHolder.CRLF;
    //    }
    //    #endregion
    //    #region TerminatorHolderGetNext()
    //    /// <summary>
    //    /// This override returns the next termination holder. If folding is set
    //    /// this will alternate between CRLF and LWSP. If folding is not set, this will return null after the
    //    /// original CRLF.
    //    /// </summary>
    //    /// <returns>Returns a TerminationHolder structure.</returns>
    //    protected override TerminatorHolder? TerminatorHolderGetNext()
    //    {
    //        if (!AllowFolding && CurrentSection == 0)
    //            return null;

    //        CurrentSection += 1;

    //        if (CurrentSection == 1)
    //            return TerminatorHolder.LWSPEx;

    //        return null;
    //    }
    //    #endregion
    //}
    //#endregion

    //#region TerminatorCRLFNoFolding
    ///// <summary>
    ///// This is the default terminator for the MessageCRLFFragment message class.
    ///// </summary>
    //public class MessageTerminatorCRLFNoFolding : MessageTerminatorCRLFFolding
    //{
    //    #region Constructor
    //    /// <summary>
    //    /// The default constructor
    //    /// </summary>
    //    public MessageTerminatorCRLFNoFolding()
    //        : base()
    //    {
    //    }
    //    #endregion

    //    #region AllowFolding
    //    /// <summary>
    //    /// This property specifies whether the message allows folding, that is a CRLF followed by a TAB or SPC character
    //    /// is not a termination. Otherwise the terminator will signal a match on CRLF.
    //    /// </summary>
    //    public override bool AllowFolding
    //    {
    //        get { return false; }
    //    }
    //    #endregion // AllowFolding

    //}
    //#endregion
    #endregion // Old code


    #region TerminatorCRLF
    /// <summary>
    /// This is the default terminator for the MessageCRLFFragment message class.
    /// </summary>
    public class MessageTerminatorCRLFFolding : MessageGenericTerminatorBase<CRLFMatchCollectionState>
    {
        #region Declarations
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageTerminatorCRLFFolding()
            : base()
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This method resets the terminator.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Initialized = true;
            mState = new CRLFMatchCollectionState(AllowFolding);
#if (DEBUG)
            mState.DebugTrace = true;
#endif
        }
        #endregion // Reset()

        #region AllowFolding
        /// <summary>
        /// This property specifies whether the message allows folding, that is a CRLF followed by a TAB or SPC character
        /// is not a termination. Otherwise the terminator will signal a match on CRLF.
        /// </summary>
        public virtual bool AllowFolding
        {
            get { return true; }
        }
        #endregion // AllowFolding


    }
    #endregion

    #region TerminatorCRLFNoFolding
    /// <summary>
    /// This is the default terminator for the MessageCRLFFragment message class.
    /// </summary>
    public class MessageTerminatorCRLFNoFolding : MessageTerminatorCRLFFolding
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MessageTerminatorCRLFNoFolding()
            : base()
        {
        }
        #endregion

        #region AllowFolding
        /// <summary>
        /// This property specifies whether the message allows folding, that is a CRLF followed by a TAB or SPC character
        /// is not a termination. Otherwise the terminator will signal a match on CRLF.
        /// </summary>
        public override bool AllowFolding
        {
            get { return false; }
        }
        #endregion // AllowFolding

    }
    #endregion
}
