#region using
using System;
using System.Data;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Net;
using System.Net.Mail;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public enum AtomLinkRelation
    {
        Alternate,
        Start,
        Next,
        Previous,
        ServiceEdit,
        ServicePost,
        ServiceFeed
    }
}
