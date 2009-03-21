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
    /// This Persistence Manager manages CDS interaction for the Stylesheet and ModelTemplate types. There is additional 
    /// functionality in this class because of the need for the stylesheet to resolve embedded references to other stylesheets during 
    /// the compile process.
    /// </summary>
    /// <typeparam name="CONT">The Stylesheet content type.</typeparam>
    /// <typeparam name="DCONT">The StylesheetBase content type.</typeparam>
    public class StylesheetResourcePM<CONT, DCONT> : ResourceBinaryPersistenceManager<CONT, DCONT, CommandConfiguration>
        where CONT : DCONT
        where DCONT : StylesheetBase
    {
        #region Declarations
        private Pool<StylesheetPMXsltResolver> mResolverPool;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public StylesheetResourcePM() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public StylesheetResourcePM(IContainer container)
            : base(container)
        {
            mResolverPool = new Pool<StylesheetPMXsltResolver>(StylesheetPMXsltResolver.StylesheetXsltResolverCreator<CONT>);
        }
        #endregion // Constructors

        #region Read(CDSContext context)
        /// <summary>
        /// This override reads the stylesheet and compiles the data.
        /// </summary>
        /// <param name="context">the current context.</param>
        /// <returns>Returns true.</returns>
        protected override bool Read(CDSContext context)
        {
            bool response = base.Read(context);

            if (response && context.Response.Status == CH.HTTPCodes.OK_200)
            {
                StylesheetPMXsltResolver xsltResolver = null;
                try
                {
                    xsltResolver = mResolverPool.Get();
                    xsltResolver.CurrentContext = context;
                    ((CONT)context.Response.Data).Compile(xsltResolver);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (xsltResolver != null)
                        mResolverPool.Return(xsltResolver);
                }
            }

            return response;
        }
        #endregion // Read(CDSContext context)

        #region PrepareData(CDSContext context, KeyValuePair<string, string> key, Uri resourceUri, CONT data)
        /// <summary>
        /// This method prepares the stylesheet and sets the name and the content ID and VersionID.
        /// </summary>
        /// <param name="context">The current context.</param>
        /// <param name="key">The request refType/refValue key pair.</param>
        /// <param name="resourceUri">The request resource Uri.</param>
        /// <param name="data">The stylesheet to adjust.</param>
        protected override void PrepareData(CDSContext context, KeyValuePair<string, string> key, Uri resourceUri, CONT data)
        {
            base.PrepareData(context, key, resourceUri, data);

            FileInfo fi = new FileInfo(key.Value);
            if (fi.Name.ToLowerInvariant().StartsWith("include."))
            {
                data.FileName = @"Include/" + fi.Name.Substring(8);
            }
            else
                data.FileName = fi.Name;
        }
        #endregion // PrepareData(CDSContext context, KeyValuePair<string, string> key, Uri resourceUri, CONT data)
    }
}
