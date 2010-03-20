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
namespace Ximura.Data
{
    public class HeaderFragment : HeaderFragment<MessageTerminatorCRLFNoFolding>
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public HeaderFragment()
            : base()
        {
        }
        #endregion
    }


    public class HeaderFragment<TERM> : MessageCRLFFragment<TERM>, IXimuraHeaderFragment
        where TERM: MessageTerminatorCRLFFolding
    {
        #region Declarations
        /// <summary>
        /// This property contains the field name.
        /// </summary>
        protected string mField;
        /// <summary>
        /// This property contains the field data.
        /// </summary>
        protected string mFieldData;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public HeaderFragment()
            : base()
        {
        }
        #endregion
        #region Reset()
        /// <summary>
        /// This method resets the data.
        /// </summary>
        public override void Reset()
        {
            mField = null;
            mFieldData = null;
            base.Reset();
        }
        #endregion // Reset()

        #region Field
        /// <summary>
        /// This property contains the field name.
        /// </summary>
        public virtual string Field
        {
            get
            {
                if (!Initializing)
                    MessagePartsBuild();

                return mField;
            }
            set
            {
                if (!Initializing)
                    throw new NotSupportedException("The Field cannot be set when the fragment is not initializing.");
                mField = value.TrimStart(new char[] { ' ' }).TrimEnd(new char[] { ' ', ':' });
            }
        }
        #endregion
        #region FieldData
        /// <summary>
        /// This property contains the field data.
        /// </summary>
        public virtual string FieldData
        {
            get
            {
                if (!Initializing)
                    MessagePartsBuild();

                return mFieldData;
            }
            set
            {
                if (!Initializing)
                    throw new NotSupportedException("The FieldData cannot be set when the fragment is not initializing.");
                mFieldData = value.TrimStart(new char[] { ':', ' ' }).TrimEnd(new char[] { '\r', '\n' });
            }
        }
        #endregion

        #region MessagePartsBuild()
        /// <summary>
        /// This method breaks the DataString in to its constituent parts.
        /// </summary>
        /// <param name="force"></param>
        protected virtual void MessagePartsBuild()
        {
            if (mField != null && mFieldData != null)
                return;

            string data = DataString;
            int colonPos = data.IndexOf(':');
            if (colonPos == -1)
            {
                mField = null;
                mFieldData = null;
                return;
            }

            mField = data.Substring(0, colonPos).Trim();
            mFieldData = data.Substring(colonPos).TrimStart(new char[] { ':', ' ' }).TrimEnd(new char[] { '\r', '\n' });
        }
        #endregion

        #region EndInitCustom()
        /// <summary>
        /// This override sets the DataString at the end of the message initialization.
        /// </summary>
        protected override void EndInitCustom()
        {
            DataString = mField + ": " + mFieldData + "\r\n";
            mField = null;
            mFieldData = null;
            base.EndInitCustom();
        }
        #endregion // EndInitCustom()
    }

}
