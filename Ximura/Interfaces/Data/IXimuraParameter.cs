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
    /// The IXimuraParameter interface is used by the parameter element. 
    /// The interface is defined from IDictionary and is based on a the Composite design pattern.
    /// </summary>
    public interface IXimuraParameter : IDictionary
    {
        /// <summary>
        /// This is the root key of the parameter.
        /// </summary>
        string Key { get;set;}
        /// <summary>
        /// This is the root value of the paramter. A parameter can contain any type of object.
        /// </summary>
        object Value { get;set;}
        /// <summary>
        /// This is the default enumerator for the parameter and will return the value of the 
        /// sub-paramters based on the key supplied.
        /// </summary>
        object this[string Key] { get;set;}
        /// <summary>
        /// This method will add a new parameter based on the key and the value.
        /// </summary>
        /// <param name="key">The key of the new parameter</param>
        /// <param name="Value">The value of the new parameter</param>
        void Add(string key, object Value);
        /// <summary>
        /// This method will append an existing parameter to the parameter collection.
        /// </summary>
        /// <param name="key">The key you wish to use for this parameter in the collection</param>
        /// <param name="Value">The parameter.</param>
        void Add(string key, IXimuraParameter Value);
        /// <summary>
        /// This method checks whether a key exists in the parameter collection.
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>Returns true if the key exist in the folder collection</returns>
        bool Contains(string key);
        /// <summary>
        /// This method checks whether a parameter exists within the parameter collection.
        /// </summary>
        /// <param name="childParameter">The child parameter.</param>
        /// <returns>Returns true if the parameter exists</returns>
        bool Contains(IXimuraParameter childParameter);
        /// <summary>
        /// This method removes a parameter from the collection based on the key passed.
        /// </summary>
        /// <param name="key">The key of the parameter to remove</param>
        void Remove(string key);
        /// <summary>
        /// This method removes a parameter from the collection based on the parameter passed.
        /// </summary>
        /// <param name="childParameter">The child parameter to be removed.</param>
        void Remove(IXimuraParameter childParameter);
        /// <summary>
        /// This method creates a new parameter and adds it to the collection.
        /// </summary>
        /// <param name="Key">The parameter key.</param>
        /// <param name="Value">The parameter value.</param>
        /// <returns>This method returns the newly created parameter.</returns>
        IXimuraParameter AddParameter(string Key, object Value);
        /// <summary>
        /// This method sets the parameter as the root parameter within a collection.
        /// </summary>
        void SetAsRootParameter();
        /// <summary>
        /// This method checks whether the parameter is the root parameter within the collection.
        /// </summary>
        /// <returns>Returns true if the parameter is the root parameter.</returns>
        bool IsRootParameter();
        /// <summary>
        /// This method checks whether the parameter has any children.
        /// </summary>
        /// <returns>Returns true if the parameter has children.</returns>
        bool HasChildParameters();

        /// <summary>
        /// This helper method retieves a parameter value from the collection.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>Returns the valus of the parameter.</returns>
        object ParameterValue(string key);
    }
}
