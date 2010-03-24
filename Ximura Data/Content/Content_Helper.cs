#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// Contributors:
//     Paul Stancer - initial implementation
// *******************************************************************************
#endregion
#region using
using System;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Diagnostics;

using System.Text;

using Ximura;
using Ximura.Data;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Data
{
    public abstract partial class Content
    {
        #region Static Declarations
        /// <summary>
        /// This random generator is used to assist export the content with a unique id.
        /// </summary>
        private static Random rndUse = new Random();
        #endregion // Declarations

        #region DumpXML
        /// <summary>
        /// This method dumps xml to the current user's desktop folder.
        /// </summary>
        public virtual string DumpXML()
        {
            string id = this.IDContent.ToString() + "_" + this.IDVersion.ToString() + "_" + rndUse.Next(100).ToString();

            return DumpXML(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), id + ".xml"));
        }
        /// <summary>
        /// This method is used in debug to dump the internal contents of the xml object to a file
        /// </summary>
        /// <param name="filename">The filename to save the file to.</param>
        public virtual string DumpXML(string filename)
        {
            File.WriteAllBytes(filename, ContentBody);
            //Debug.WriteLine(filename);
            return filename;
        }
        #endregion // DumpXML()
    }
}
