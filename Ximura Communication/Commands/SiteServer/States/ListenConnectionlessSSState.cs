﻿#region using
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

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Communication
{
    public class ListenConnectionlessSSState<RQ, RS>: ListenBaseSiteServerState<RQ, RS>
        where RQ : SiteServerRQ, new()
        where RS : SiteServerRS, new()
    {
        #region Constructors
		/// <summary>
		/// This is the default constructor.
		/// </summary>
		public ListenConnectionlessSSState():this(null){}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The container</param>
        public ListenConnectionlessSSState(IContainer container)
            : base(container)
		{
		}
		#endregion // Constructors
    }
}
