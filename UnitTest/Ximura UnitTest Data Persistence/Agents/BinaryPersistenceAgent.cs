#region using
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ximura;
using Ximura.Data;
#endregion
namespace Ximura.UnitTest.Data
{
    [CDSStateActionPermit(CDSAction.Construct)]
    [CDSStateActionPermit(CDSAction.Create)]
    [CDSStateActionPermit(CDSAction.Read)]
    [CDSStateActionPermit(CDSAction.Update)]
    [CDSStateActionPermit(CDSAction.Delete)]
    [CDSStateActionPermit(CDSAction.VersionCheck)]
    [CDSStateActionPermit(CDSAction.ResolveReference)]
    public class BinaryPersistenceAgent : PersistenceManagerCDSState<BinaryTest, BinaryTest, SQLDBPMCDSConfiguration>
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public BinaryPersistenceAgent() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public BinaryPersistenceAgent(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        protected override bool Construct(CDSContext context)
        {
            context.Response.Data = new BinaryTest();
            context.Response.Status = "200";

            return true;
        }

        
    }
}
