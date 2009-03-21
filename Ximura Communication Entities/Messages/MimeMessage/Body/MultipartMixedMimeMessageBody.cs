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
using System.Text;
using System.Collections.Generic;
using Ximura;
using Ximura.Helper;
using Ximura.Data;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Communication
{
    public class MultipartMixedMimeMessageBody : MimeBodyMessage
    {
        #region Declarations
        /// <summary>
        /// This is the main body fragment.
        /// </summary>
        protected PreambleMimeMessageBodyFragment mPreambleBody;
        /// <summary>
        /// This is the main body fragment.
        /// </summary>
        protected MessageFragment mEpilogueBody;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MultipartMixedMimeMessageBody()
            : base()
        {
        }
        #endregion

        #region FragmentInitialType
        /// <summary>
        /// This is the fragment initial type.
        /// </summary>
        protected override Type FragmentHeaderInitialType
        {
            get
            {
                return typeof(PreambleMimeMessageBodyFragment);
            }
        }
        #endregion // FragmentInitialType
        #region FragmentFinalType
        /// <summary>
        /// This is the final type for the Mime message
        /// </summary>
        protected virtual Type FragmentFinalType
        {
            get
            {
                return typeof(MessageFragment);
            }
        }
        #endregion // FragmentFinalType

        #region FragmentSetNext()
        /// <summary>
        /// This method sets the next fragment in the message.
        /// </summary>
        protected override IXimuraMessage FragmentSetNext()
        {
            if (FragmentFirst == null)
            {
                return FragmentSetNext(FragmentHeaderInitialType);
            }

            if ((FragmentCurrent is MimeMessage || FragmentCurrent is MimeMessageFragment)
                && !FragmentCurrent.IsTerminator)
            {
                return FragmentSetNext(typeof(MimeMessage));
            }

            return FragmentSetNext(FragmentFinalType);
        }
        #endregion // FragmentSetNext()
    }
}
