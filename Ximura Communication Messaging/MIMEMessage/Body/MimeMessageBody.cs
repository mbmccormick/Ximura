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
    /// <summary>
    /// This is the base Mime Body message class.
    /// </summary>
    public class MimeBodyMessage : Message, IXimuraMimeMessageInitialize
    {
        #region Declarations
        /// <summary>
        /// This is the mime boundary used in the deliminator.
        /// </summary>
        protected string mBoundary;
        /// <summary>
        /// This is the encoding type passed in the load constructor.
        /// </summary>
        protected string mEncoding;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public MimeBodyMessage()
            : base()
        {
        }
        #endregion

        #region Reset()
        /// <summary>
        /// This is the pooling reset method.
        /// </summary>
        public override void Reset()
        {
            mBoundary = null;
            mEncoding = null;
            base.Reset();
        }
        #endregion // Reset()

        #region Initialize(string deliminator, string encoding)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deliminator"></param>
        /// <param name="encoding"></param>
        public virtual void Initialize(string boundary, string encoding)
        {
            mEncoding = encoding;
            mBoundary = boundary;
        }
        #endregion // Initialize(string deliminator, string encoding)

        #region FragmentSetNext(Type fragmentType, int maxLength)
        /// <summary>
        /// This method returns a new fragment object for the type specified.
        /// </summary>
        /// <param name="fragmentType">The fragment type required.</param>
        /// <param name="maxLength">The maximum permitted length for the fragment.</param>
        protected override IXimuraMessage FragmentSetNext(Type fragmentType, long maxLength)
        {
            IXimuraMessage fragment = this.PoolGetObject(fragmentType) as IXimuraMessage;
            //Set the maximum length of the fragment.
            if (fragment is IXimuraMimeMessageInitialize)
                ((IXimuraMimeMessageInitialize)fragment).Initialize(mBoundary, mEncoding);

            fragment.Load(maxLength);
            FragmentAddInternal(fragment);
            return fragment;
        }
        #endregion


        public IEnumerable<MimeMessage> DataParts
        {
            get
            {
                foreach (IXimuraMessage message in mMessageParts.Values)
                {
                    if (message is MimeMessage)
                        yield return (MimeMessage)message;
                }
            }
        }

    }
}
