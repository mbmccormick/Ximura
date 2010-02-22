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
using System.Data;

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;

using Ximura;
using Ximura.Data;
using Ximura.Server;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Data
{
    //public partial class DataContent: IListSource
    //{
    //    #region GetList()
    //    /// <summary>
    //    /// This method gets the IList object for databinding
    //    /// </summary>
    //    /// <returns></returns>
    //    public virtual IList GetList()
    //    {
    //        if (this.LinkType == DataContentLinkType.Link && (!mSatelliteEntity ?? true))
    //        {
    //            LoadFromParentEntityServer();
    //        }
    //        //DataViewManager hello = new DataViewManager();
    //        //DataViewManager is the internal class used to expose the tables to the 
    //        //binding architecture.
    //        if (!mSatelliteEntity ?? false)
    //        {
    //            IXimuraDataEntityServer parentDataServer =
    //                GetService(typeof(IXimuraDataEntityServer)) as IXimuraDataEntityServer;

    //            if (parentDataServer != null)
    //            {
    //                LoadFromParentEntityServer(parentDataServer);
    //            }
    //        }

    //        IListSource source = mDataContentSet as IListSource;
    //        if (source == null)
    //            return null;
    //        else
    //        {
    //            IList list = source.GetList();
    //            DataViewManager newList = list as DataViewManager;

    //            return newList as IList;
    //        }
    //    }
    //    #endregion // GetList()

    //    #region ContainsListCollection
    //    /// <summary>
    //    /// This property identifies whether the object supports the IList
    //    /// interface for databinding
    //    /// </summary>
    //    [Browsable(false)]
    //    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    //    public virtual bool ContainsListCollection
    //    {
    //        get
    //        {
    //            //				IListSource source = mDataContentSet as IListSource;
    //            //				if (source == null)
    //            //					return false;
    //            //				else
    //            return true;
    //        }
    //    }
    //    #endregion // ContainsListCollection
    //}
}
