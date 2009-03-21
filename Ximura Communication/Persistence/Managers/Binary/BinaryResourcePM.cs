#region using
using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using Ximura;
using Ximura.Data;
using Ximura.Persistence;
using Ximura.Helper;
using Ximura.Server;
using Ximura.Command;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
#endregion
namespace Ximura.Communication
{
    /// <summary>
    /// This persistence manager handles BinaryContent embedded within the assembly.
    /// </summary>
    public class BinaryResourcePM : ResourceBinaryPersistenceManager<BinaryContent, BinaryContent, CommandConfiguration>
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public BinaryResourcePM() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public BinaryResourcePM(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region BaseResourceString
        /// <summary>
        /// This string returns the path to the resource folder in the assembly.
        /// </summary>
        protected override string BaseResourceString
        {
            get
            {
                throw new NotImplementedException("BaseResourceString is not implemented.");
            }
        }
        #endregion // BaseResourceString

        #region BaseResourceLocationResolve(Type type, Guid tid, KeyValuePair<string, string> key)
        /// <summary>
        /// This method resolves the resource loacation for the requested BinaryContent type.
        /// </summary>
        /// <param name="type">The request type.</param>
        /// <param name="tid">The request type id.</param>
        /// <param name="key">The reftype/refValue key value pair.</param>
        /// <returns>Returns the resource Uri the resolves the location of the binary content.</returns>
        protected override Uri BaseResourceLocationResolve(Type type, Guid tid, KeyValuePair<string, string> key)
        {
            string baseVal;

            switch (tid.ToString().ToUpperInvariant())
            {
                case "7806DA64-20DA-447E-BECD-F8DF6E7D9E76": //Image
                    baseVal = "Images";
                    break;
                case "B1BB9AE5-950F-4F08-B748-54B1CB5D7F40": //Script
                    baseVal = "JS";
                    break;
                case "9FCDE441-C438-46E3-86F7-16C8A20752E7": //CSS
                    baseVal = "CSS";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new Uri(string.Format(BaseResourceString, baseVal, key.Value));

        }
        #endregion // BaseResourceLocationResolve(Type type, Guid tid, KeyValuePair<string, string> key)
    }
}
