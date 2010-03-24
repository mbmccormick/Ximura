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
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
#endregion // using
namespace Ximura.Data.SQL
{
    #region EntityExceptionBase
    /// <summary>
    /// This abstract exception is used to catch errors and throw the correct error code.
    /// </summary>
    public abstract class EntityExceptionBase : Exception
    {
        public EntityExceptionBase():base()
        {
        }

        public EntityExceptionBase(string message):base(message)
        {
        }

        public EntityExceptionBase(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public abstract int ErrorResponse { get;}
    }
    #endregion // EntityExceptionBase

    public class BlobParseException : EntityExceptionBase
    {

        public BlobParseException():base()
        {

        }

        public BlobParseException(string message):base(message)
        {

        }

        public BlobParseException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public override int ErrorResponse
        {
            get { return 500; }
        }
    }

    public class EntityCreateException : EntityExceptionBase
    {
        private int mResponse = 500;

        public EntityCreateException(int response)
            : base()
        {
            mResponse = response;
        }

        public EntityCreateException(string message)
            : base(message)
        {

        }

        public EntityCreateException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public override int ErrorResponse
        {
            get { return mResponse; }
        }
    }

    public class InvalidAQNException: EntityExceptionBase
    {
        private int mResponse = 500;

        public InvalidAQNException(int response)
            : base()
        {
            mResponse = response;
        }

        public InvalidAQNException(string message)
            : base(message)
        {

        }

        public InvalidAQNException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public override int ErrorResponse
        {
            get { return mResponse; }
        }
    }

}
