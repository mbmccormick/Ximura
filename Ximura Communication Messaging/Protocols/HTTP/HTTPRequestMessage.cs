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
using System.Threading;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
#endregion // using
namespace Ximura.Communication
{
    public class HTTPRequestMessage: InternetMessage
    {
        #region Declarations
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public HTTPRequestMessage()
            : base()
        {
        }
        #endregion

        #region FragmentInitialType
        /// <summary>
        /// This method returns the initial fragment type for the class.
        /// </summary>
        protected override Type FragmentHeaderInitialType
        {
            get
            {
                return typeof(HTTPRequestHeaderFragment);
            }
        }
        #endregion // FragmentInitialType

#if (DEBUG)
        public override int Write(byte[] buffer, int offset, int count)
        {
            return base.Write(buffer, offset, count);
        }
#endif

        #region Host
        /// <summary>
        /// This is the header host.
        /// </summary>
        public string Host
        {
            get
            {
                return HeaderSingle("host");
            }
        }
        #endregion // Host

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

            if (FragmentCurrent.IsTerminator)
            {
                long? bodyLength = BodyLength;

                if (!bodyLength.HasValue)
                    throw new ArgumentOutOfRangeException();

                ContentType cType = ContentType;

                switch (cType.MediaType)
                {
                    case "multipart/form-data":
                        MultipartFormDataMimeMessageBody messageBody =
                            PoolGetObject(typeof(MultipartFormDataMimeMessageBody))
                                as MultipartFormDataMimeMessageBody;

                        ////Set the maximum length of the fragment.
                        messageBody.Initialize(cType.Parameter("boundary"), null);

                        messageBody.Load(bodyLength.Value);
                        FragmentAddInternal(messageBody);

                        mBody = messageBody;
                        break;
                    case "application/x-www-form-urlencoded":
                        mBody = FragmentSetNext(typeof(WWWFormUrlEncodedBodyFragment), bodyLength.Value);
                        break;
                    case "":
                    default:
                        mBody = FragmentSetNext(typeof(InternetMessageFragmentBody), bodyLength.Value);
                        break;
                }

                return mBody;
            }

            return FragmentSetNext(FragmentHeaderType);
        }
        #endregion // FragmentSetNext()


        #region ContentType
        /// <summary>
        /// This property returns the body length of the message.
        /// </summary>
        public virtual ContentType ContentType
        {
            get
            {
                if (mHeaderCollection.ContainsKey("content-type"))
                {
                    int[] ids = mHeaderCollection["content-type"];
                    HeaderFragment header = mMessageParts[ids[0]] as HeaderFragment;
                    return new ContentType(header.FieldData);
                }

                return null;
            }
        }
        #endregion // BodyLength
    }


}
