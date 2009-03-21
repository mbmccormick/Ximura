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
    /// This is the base class for fragment bodies. Body class differ from standard message fragments 
    /// because they implement the IXimuraMessageFragmentBody interface which facilitates the creation
    /// of the body meta-data headers.
    /// </summary>
    public class InternetMessageFragmentBody : MessageFragment//, IXimuraMessageFragmentBody
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public InternetMessageFragmentBody()
            : base()
        {
        }
        #endregion

        #region ContentType
        public virtual bool HasContentType
        {
            get { return false; }
        }

        public virtual string ContentType
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException("ContentType does not support set.");
            }
        }
        #endregion // ContentType

        #region ContentEncoding
        public virtual bool HasContentEncoding
        {
            get { return false; }
        }

        public virtual string ContentEncoding
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException("ContentEncoding does not support set.");
            }
        }
        #endregion // ContentEncoding

        #region ContentMD5
        public virtual bool HasContentMD5
        {
            get { return false; }
        }

        public virtual string ContentMD5
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException("ContentMD5 does not support set.");
            }
        }
        #endregion // ContentMD5

        #region ContentLanguage
        public virtual bool HasContentLanguage
        {
            get { return false; }
        }

        public virtual string ContentLanguage
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException("ContentLanguage does not support set.");
            }
        }
        #endregion // ContentLanguage

        #region ContentRange
        public virtual bool HasContentRange
        {
            get { return false; }
        }

        public virtual string ContentRange
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException("ContentRange does not support set.");
            }
        }
        #endregion // ContentRange

        #region ETag
        public virtual bool HasETag
        {
            get { return false; }
        }

        public virtual string ETag
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException("ETag does not support set.");
            }
        }

        #endregion

        #region Expires
        public virtual bool HasExpires
        {
            get { return false; }
        }

        public virtual string Expires
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotSupportedException("Expires does not support set.");
            }
        }

        #endregion

        #region LastModified
        public virtual bool HasLastModified
        {
            get { return false; }
        }

        public virtual string LastModified
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
            set
            {
                throw new NotSupportedException("LastModified does not support set.");
            }
        }

        #endregion
    }
}
