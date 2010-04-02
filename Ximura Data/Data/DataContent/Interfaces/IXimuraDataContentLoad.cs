#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2010 Paul Stancer.
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
    public interface IXimuraDataContentLoad
    {
        /// <summary> 
        /// This method loads the DataContent from a DataSet.
        /// </summary>
        /// <param name="data">The DataSet this content should hold.</param>
        void Load(DataSet data);

        /// <summary>
        /// This method is used to create a new entity.
        /// </summary>
        bool Load();
        /// <summary>
        /// This method is used to create a new entity.
        /// </summary>
        /// <param name="force">if this property is set to true the entity will discard any changes if set to true.</param>
        /// <returns>Returns true if a new entity was created.</returns>
        bool Load(bool force);
    }
}
