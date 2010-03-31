#region using
using System;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// This class is used for hosting an application within a class.
    /// </summary>
    /// <typeparam name="APP">The app server type.</typeparam>
    public class AppServerContainer<APP> //: IXimuraComponentService, IComponent
        where APP : class, IXimuraAppServer
    {
    }
}
