#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using AH = Ximura.Helper.AttributeHelper;
using Ximura.Server;
using Ximura.Command;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The HTTP SiteServer response.
    /// </summary>
    public class HTTPSiteServerRS : SiteServerRS
    {
		#region Constructors
		/// <summary>
		/// This is the default constuctor.
		/// </summary>
		public HTTPSiteServerRS():base()
		{
		}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The base container.</param>
		public HTTPSiteServerRS(System.ComponentModel.IContainer container): base(container)
		{
		}

		/// <summary>
		/// This is the deserialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
        public HTTPSiteServerRS(SerializationInfo info, StreamingContext context)
            : base(info, context)
		{
		}
		#endregion

    }
}
