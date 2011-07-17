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
using System.Runtime.Serialization;
using System.IO;
using System.Text;

using Ximura;
#endregion // using
namespace Ximura
{
    /// <summary>
    /// This 
    /// </summary>
    public class CSVStreamWriter: TextWriter
    {
        private Encoding hmm;

        public CSVStreamWriter()
        {
            hmm = Encoding.UTF8;
        }

        public override IFormatProvider FormatProvider
        {
            get
            {
                return base.FormatProvider;
            }
        }



        public override Encoding Encoding
        {
            get { return hmm; }
        }
    }
}
