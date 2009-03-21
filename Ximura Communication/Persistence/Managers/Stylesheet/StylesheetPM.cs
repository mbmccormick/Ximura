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
    [CDSStateActionPermit(CDSStateAction.Create)]
    [CDSStateActionPermit(CDSStateAction.Read)]
    [CDSStateActionPermit(CDSStateAction.Update)]
    [CDSStateActionPermit(CDSStateAction.Delete)]
    [CDSStateActionPermit(CDSStateAction.VersionCheck)]
    [CDSStateActionPermit(CDSStateAction.ResolveReference)]
    public class StylesheetPM<CONT, DCONT> : SQLDBPersistenceManager<CONT, DCONT>
        where CONT : DCONT
        where DCONT : StylesheetBase
    {
        #region Declarations
        protected Pool<StylesheetPMXsltResolver> mResolverPool;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public StylesheetPM() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public StylesheetPM(IContainer container)
            : base(container)
        {
            mResolverPool = new Pool<StylesheetPMXsltResolver>(StylesheetPMXsltResolver.StylesheetXsltResolverCreator<CONT>);
        }
        #endregion // Constructors

        #region Read(CDSContext context)
        /// <summary>
        /// This override compiles the stylesheet before it leaves the persistence manager,
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
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
        #region ResolveReference(CDSContext context)
        /// <summary>
        /// This method resolves the stylesheet reference
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool ResolveReference(CDSContext context)
        {
            //we set the cacheable property to false as the stylesheet is compliled and is not cacheable.
            context.ContentIsCacheable = false;
            return base.ResolveReference(context);
        }
        #endregion // ResolveReference(CDSContext context)
    }
}
