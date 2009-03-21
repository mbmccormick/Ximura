#region using
using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using Ximura;
using Ximura.Data;
using Ximura.Persistence;
using Ximura.Helper;
using Ximura.Server;
using Ximura.Command;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
#endregion
namespace Ximura.Communication
{
    /// <summary>
    /// This class is used to resolve include stylesheets reference in the base stylesheet.
    /// </summary>
    public class StylesheetPMXsltResolver : XmlResolver, IXimuraPoolableObject
    {
        #region Declarations
        public delegate Uri Resolver(CDSContext context, Uri baseUri, string relativeUri, System.Net.ICredentials Credentials);
        public delegate object EntityResolver(CDSContext context, Uri absoluteUri, string role, Type ofObjectToReturn, System.Net.ICredentials Credentials);

        ICredentials mCredentials = null;

        Resolver mResolver;
        EntityResolver mEntityResolver;

        private readonly Guid mTrackID = Guid.NewGuid();

        private CDSContext mContext;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The pool manager constructor. This should not be used.
        /// </summary>
        public StylesheetPMXsltResolver() : this(null, null) { }
        /// <summary>
        /// This class contains a redirection to the entity reolution method.
        /// </summary>
        /// <param name="resolver">The uri resolver.</param>
        /// <param name="entityResolver">The entity object resolver.</param>
        public StylesheetPMXsltResolver(Resolver resolver, EntityResolver entityResolver)
        {
            mResolver = resolver;
            mEntityResolver = entityResolver;
        }
        #endregion // Constructor

        #region ResolveUri(Uri baseUri, string relativeUri)
        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            return mResolver(mContext, baseUri, relativeUri, mCredentials);
        }
        #endregion // ResolveUri(Uri baseUri, string relativeUri)
        #region GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            return mEntityResolver(mContext, absoluteUri, role, ofObjectToReturn, mCredentials);
        }
        #endregion // GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        #region Credentials
        /// <summary>
        /// The network credentials of the resolver.
        /// </summary>
        public override System.Net.ICredentials Credentials
        {
            set { mCredentials = value; }
        }
        #endregion // Credentials
        #region CurrentContext
        /// <summary>
        /// This method sets the current context.
        /// </summary>
        public CDSContext CurrentContext
        {
            get { return mContext; }
            set { mContext = value; }
        }
        #endregion // CurrentContext
        #region IXimuraPoolableObject Members

        bool IXimuraPoolableObject.CanPool
        {
            get { return true; }
        }

        Guid IXimuraPoolableObject.TrackID
        {
            get { return mTrackID; }
        }

        void IXimuraPoolableObject.Reset()
        {
            mContext = null;
        }

        #endregion
        #region IDisposable Members/Finalize
        private bool disposed = false;
        /// <summary>
        /// The dispose method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// The overrided dispose method
        /// </summary>
        /// <param name="disposing">True if this is called by dispose, false if this
        /// is called by the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                mCredentials = null;
                mResolver = null;
                mEntityResolver = null;
                mContext = null;
            }
            disposed = true;
        }
        #endregion

        #region StylesheetXsltResolverCreator()
        /// <summary>
        /// This is the remote invocation method for the resolver Pool Manager
        /// </summary>
        /// <returns></returns>
        public static StylesheetPMXsltResolver StylesheetXsltResolverCreator<CONT>() where CONT : StylesheetBase
        {
            return new StylesheetPMXsltResolver(StylesheetResolver, StlyesheetEntityResolver<CONT>);
        }
        #endregion // StylesheetXsltResolverCreator()
        #region StylesheetResolver
        private static Uri StylesheetResolver(CDSContext context, Uri baseUri, string relativeUri, System.Net.ICredentials Credentials)
        {
            return new Uri(relativeUri, UriKind.Relative);
        }
        #endregion // StylesheetResolver(CDSContext context, Uri baseUri, string relativeUri, System.Net.ICredentials Credentials)
        #region StlyesheetEntityResolver
        private static object StlyesheetEntityResolver<CONT>(CDSContext context,
            Uri absoluteUri, string role, Type ofObjectToReturn, System.Net.ICredentials Credentials) where CONT : StylesheetBase
        {
            CONT data = null;
            try
            {
                string refValue = absoluteUri.OriginalString.Replace('/', '.');

                switch (context.CDSHelperDirect.Read<CONT>("name", refValue, out data))
                {
                    case CDSResponse.OK:
                        return new MemoryStream(data.ToArray());
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("{0} cannot be resolved", absoluteUri.OriginalString));
                }
            }
            finally
            {
                if (data != null && data.ObjectPoolCanReturn)
                    data.ObjectPoolReturn();
            }
        }
        #endregion // StlyesheetEntityResolver
    }
}
