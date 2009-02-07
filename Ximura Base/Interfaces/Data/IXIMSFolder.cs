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
using System.Collections;

using Ximura;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// IXimuraFolder is the facade interface that extends the content class and allows it to 
    /// include additional content and parameters.
    /// </summary>
    public interface IXimuraFolder : IXimuraParameter
    {
        /// <summary>
        /// This property returns a collection of Content keys.
        /// </summary>
        ICollection ContentKeys { get;}
        /// <summary>
        /// This property returns a collection of Content values.
        /// </summary>
        ICollection ContentValues { get;}
        /// <summary>
        /// This property returns the count of the folder content objects.
        /// </summary>
        /// <returns>The number of Content object.</returns>
        int ContentCount();

        /// <summary>
        /// This property returns a collection of Parameters keys.
        /// </summary>
        ICollection ParameterKeys { get;}
        /// <summary>
        /// This property returns a collection of Parameters values.
        /// </summary>
        ICollection ParameterValues { get;}
        /// <summary>
        /// This property returns the count of the child parameter object.
        /// </summary>
        /// <returns>The number of child parameter objects.</returns>
        int ParameterCount();

        /// <summary>
        /// This method add a content object to the Folder collection.
        /// </summary>
        /// <param name="Key">The content key.</param>
        /// <param name="theContent">The content object to add.</param>
        void Add(string Key, IXimuraContent theContent);
        /// <summary>
        /// This method checks whether the Folder contains this Content object.
        /// </summary>
        /// <param name="theContent">The content object to check</param>
        /// <returns>Returns true is the content object is in the Folder</returns>
        bool Contains(IXimuraContent theContent);
        /// <summary>
        /// This method removes a content object from the Folder collection.
        /// </summary>
        /// <param name="theContent">The content object to remove.</param>
        void Remove(IXimuraContent theContent);
    }

}
