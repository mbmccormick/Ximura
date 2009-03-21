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
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
using AH = Ximura.Helper.AttributeHelper;
#endregion // using
namespace Ximura.Communication
{
    public class SiteServerPerformance : FSMCommandPerformance
    {
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Content object.
		/// </summary>
        public SiteServerPerformance()
        {
        }
		#endregion

    }
}
