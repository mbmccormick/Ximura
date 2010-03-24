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

using Ximura.Data;
using CH = Ximura.Common;
#endregion
namespace Ximura.Communication
{
    public class MimeMessageFragment : MessageFragment<MimeMessageTerminator>, IXimuraMimeMessageInitialize
    {
        #region Declarations
        protected string mEncoding;
        #endregion // Declarations

        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MimeMessageFragment()
            : base()
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This reset override resets the deliminator and the body encoding.
        /// </summary>
        public override void Reset()
        {
            mEncoding = null;
            base.Reset();
        }
        #endregion // Reset()

        public virtual Stream BodyStream
        {
            get
            {
                if (CanWrite || Initializing || !BodyLength.HasValue)
                    return null;
                return new MemoryStream(mBuffer, 0, (int)BodyLength.Value-2, false);
            }
        }

        #region IXimuraMimeMessageInitialize Members

        public void Initialize(string boundary, string encoding)
        {
            mEncoding = encoding;
            Terminator.Initialize(boundary);
        }

        #endregion
    }
}
