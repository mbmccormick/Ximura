#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2011 Paul Stancer.
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
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
#endregion
namespace Ximura
{
    /// <summary>
    /// This fragment holds the individual line items from the CSV file.
    /// </summary>
    public class CSVBaseFragment : MessageFragment<MessageTerminatorCSV>
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public CSVBaseFragment()
            : base()
        {
        }
        #endregion

        public override int Write(byte[] buffer, int offset, int count)
        {
            return base.Write(buffer, offset, count);
        }


        /// <summary>
        /// This enumeration returns the items in the CSV line.
        /// </summary>
        public virtual IEnumerable<string> Items
        {
            get
            {
                yield break;
            }
        }
    }
}
