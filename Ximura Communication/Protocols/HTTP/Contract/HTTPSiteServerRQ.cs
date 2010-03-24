#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using AH = Ximura.AttributeHelper;
using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The HTTP SiteServer request.
    /// </summary>
    public class HTTPSiteServerRQ : SiteServerRQ
    {
		#region Constructors
		/// <summary>
		/// This is the default constuctor.
		/// </summary>
		public HTTPSiteServerRQ():base()
		{
		}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The base container.</param>
		public HTTPSiteServerRQ(System.ComponentModel.IContainer container): base(container)
		{
		}

		/// <summary>
		/// This is the deserialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
        public HTTPSiteServerRQ(SerializationInfo info, StreamingContext context)
            : base(info, context)
		{
		}
		#endregion
    }
}
