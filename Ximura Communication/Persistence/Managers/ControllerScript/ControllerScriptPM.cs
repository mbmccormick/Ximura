#region using
using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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
    public class ControllerScriptPM :
        SQLDBPersistenceManager<ControllerScript, ControllerScript>
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ControllerScriptPM() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ControllerScriptPM(IContainer container)
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
    }
}
