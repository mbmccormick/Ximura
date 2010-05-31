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
    /// <summary>
    /// This persistence agent manages the BinaryTest object.
    /// </summary>
    [CDSStateActionPermit(CDSAction.Construct)]
    [CDSStateActionPermit(CDSAction.Create)]
    [CDSStateActionPermit(CDSAction.Read)]
    [CDSStateActionPermit(CDSAction.Update)]
    [CDSStateActionPermit(CDSAction.Delete)]
    [CDSStateActionPermit(CDSAction.VersionCheck)]
    [CDSStateActionPermit(CDSAction.ResolveReference)]
    public class BinaryTestPersistenceAgent : PersistenceManagerCDSState<BinaryTest, BinaryTest, SQLDBPMCDSConfiguration>
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public BinaryTestPersistenceAgent() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public BinaryTestPersistenceAgent(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region Construct(CDSContext context)
        /// <summary>
        /// Constructs the BinaryTest object and adds it to the persistence context.
        /// </summary>
        /// <param name="context">The persistence context.</param>
        /// <returns>Returns true to indicate a successful operation.</returns>
        protected override bool Construct(CDSContext context)
        {
            context.Response.Data = new BinaryTest();
            context.Response.Status = "200";

            return true;
        }
        #endregion 


        
    }
}
