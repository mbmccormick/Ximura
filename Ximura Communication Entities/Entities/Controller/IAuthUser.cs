#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Net.Mail;
using System.ComponentModel;
using System.Collections.Generic;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public interface IAuthUser
    {
        string UserName { get; }
        string RealmDomain { get; }
    }
}
