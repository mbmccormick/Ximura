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
using System.Threading;
using System.Timers;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

using System.Diagnostics;

using Ximura;
using Ximura.Helper;
using Ximura.Data;

using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Communication
{
    public class HTTPResponseHeaderFragment : ResponseHeaderFragment
    {
        #region Declarations
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public HTTPResponseHeaderFragment()
            : base()
        {
        }
        #endregion
        #region Reset()
        /// <summary>
        /// This is the reset method to set the content.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
        }
        #endregion // Reset()

        //public override string Protocol
        //{
        //    get
        //    {
        //        return base.Protocol;
        //    }
        //    set
        //    {
        //        base.Protocol = "HTTP/1.1";
        //    }
        //}

        
        #region FragmentCollectionComplete()
        /// <summary>
        /// This method is used to complete the header collection organization once the initialization phase has ended.
        /// </summary>
        protected override void EndInitCustom()
        {
            Protocol = "HTTP/1.1";

            base.EndInitCustom();
        }
        #endregion // FragmentCollectionComplete()
    }
}
