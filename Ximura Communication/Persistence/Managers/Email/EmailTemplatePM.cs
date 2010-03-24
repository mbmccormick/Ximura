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
using Ximura.Data;

using Ximura.Framework;
using Ximura.Framework;
using CH = Ximura.Common;
using RH = Ximura.Reflection;
#endregion
namespace Ximura.Communication
{
    [CDSStateActionPermit(CDSStateAction.Create)]
    [CDSStateActionPermit(CDSStateAction.Update)]
    [CDSStateActionPermit(CDSStateAction.Read)]
    [CDSStateActionPermit(CDSStateAction.VersionCheck)]
    [CDSStateActionPermit(CDSStateAction.ResolveReference)]
    [CDSStateActionPermit(CDSStateAction.Browse)]
    public class EmailTemplatePM : SQLDBPersistenceManager<EmailTemplate, EmailTemplate>
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public EmailTemplatePM() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public EmailTemplatePM(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors


        protected override bool Create(CDSContext context)
        {
            return base.Create(context);
        }

        protected override bool Read(CDSContext context)
        {
            return base.Read(context);
        }

        protected override bool Update(CDSContext context)
        {
            return base.Update(context);
        }

        protected override bool Delete(CDSContext context)
        {
            return base.Delete(context);
        }

        protected override bool ResolveReference(CDSContext context)
        {
            return base.ResolveReference(context);
        }

        protected override bool VersionCheck(CDSContext context)
        {
            return base.VersionCheck(context);
        }
    }
}
