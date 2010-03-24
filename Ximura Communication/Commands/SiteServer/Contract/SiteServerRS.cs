#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net.Mail;
using System.Diagnostics;
using System.ComponentModel;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
using AH = Ximura.Helper.AttributeHelper;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the default response folder for the Site Server command.
    /// </summary>
    public class SiteServerRS : RQRSFolder
    {
		#region Constructors
		/// <summary>
		/// This is the default constuctor.
		/// </summary>
		public SiteServerRS():base()
		{
		}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The base container.</param>
		public SiteServerRS(System.ComponentModel.IContainer container): base(container)
		{
		}

		/// <summary>
		/// This is the deserialization constructor
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
        public SiteServerRS(SerializationInfo info, StreamingContext context):base(info,context)
		{
		}
		#endregion

    }
}
