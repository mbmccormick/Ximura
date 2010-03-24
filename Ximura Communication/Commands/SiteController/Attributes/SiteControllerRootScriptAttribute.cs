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
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This attribute is used to specify the default script for the Site Controller Command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SiteControllerRootScriptAttribute : Attribute
    {
        #region Declarations
        private string mItem;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is theprimary constructor for the attribute.
        /// </summary>
        /// <param name="item">The uri for the default script.</param>
        public SiteControllerRootScriptAttribute(string item)
        {
            mItem = item;
        }
        #endregion // Constructors

        public string RootScript
        {
            get
            {
                return mItem;
            }
        }
    }
}
