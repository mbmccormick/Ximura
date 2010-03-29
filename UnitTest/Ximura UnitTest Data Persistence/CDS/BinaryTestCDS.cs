#region using
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ximura;
using Ximura.Data;
using Ximura.Framework;
#endregion
namespace Ximura.UnitTest.Data
{
    /// <summary>
    /// This content data store is for testing.
    /// </summary>
    public class BinaryTestCDS: ContentDataStore
    {
        #region Constructors
		/// <summary>
		/// Empty constructor used during the design mode.
		/// </summary>
		public BinaryTestCDS():this((IContainer)null){}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
        public BinaryTestCDS(IContainer container)
            : base(container)
		{

		}
		#endregion
    }
}
