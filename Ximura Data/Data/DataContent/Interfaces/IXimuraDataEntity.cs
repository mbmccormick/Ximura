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
using System.ComponentModel;
using System.Collections;
using System.Runtime.Serialization;
using System.Data;

using Ximura;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This interface is retired and is currently only used by old data
    /// entities.
    /// </summary>
    public interface IXimuraDataEntity : IXimuraContent, ISupportInitialize
    {
        /// <summary>
        /// This property returns true if the content is a satelite entity.
        /// </summary>
        bool SatelliteEntity { get;}
        /// <summary>
        /// This method returns the internal dataset for the Data Entity
        /// </summary>
        DataSet GetDataSet { get;}        
        
        ///// <summary> 
        ///// This method loads the DataContent from a DataSet.
        ///// </summary>
        ///// <param name="data">The DataSet this content should hold.</param>
        //void EntityLoad(DataSet data);
        ///// <summary>
        ///// This method is used to create a new entity.
        ///// </summary>
        //bool EntityCreateNew();
        ///// <summary>
        ///// This method is used to create a new entity.
        ///// </summary>
        ///// <param name="force">if this property is set to true the entity will discard any changes if set to true.</param>
        ///// <returns>Returns true if a new entity was created.</returns>
        //bool EntityCreateNew(bool force);

    }

}
