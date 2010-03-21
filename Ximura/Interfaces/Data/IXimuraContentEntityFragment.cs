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
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This interface is used to identify the fragmentation options for the entity.
    /// </summary>
    public interface IXimuraContentEntityFragment
    {
        /// <summary>
        /// This property identifies whether the content is a fragment type.
        /// </summary>
        /// <returns>Returns true if the object is a fragment, or supports fragmentation.</returns>
        bool IsFragment();
        /// <summary>
        /// This is the fragment base type. If this property is supported the persistence manager 
        /// will store this property in the serialization blob. Should the original type be unavailable 
        /// the content will be deserialized using this type.
        /// </summary>
        /// <returns>Return the base content type if supported.</returns>
        Type FragmentBaseType();
        /// <summary>
        /// This method identifies whether the content supports the base type content.
        /// </summary>
        /// <returns>Returns true if this property is supported.</returns>
        bool SupportsFragmentBaseType();
        /// <summary>
        /// This boolean property identifies whether a fragment can convert to a primary entity.
        /// </summary>
        /// <returns>Returns true if the entity can convert.</returns>
        bool CanConvertToPrimaryEntity();
        /// <summary>
        /// This method converts the fragment entity to the primary entity.
        /// </summary>
        void ConvertFragmentToPrimaryEntity();
        /// <summary>
        /// This method is implemented by the fragment content and will be used to merge any
        /// changes back in to the root content.
        /// </summary>
        /// <param name="baseContent">The base content to merge in to.</param>
        void MergeContent(IXimuraContent baseContent);
        /// <summary>
        /// This method is implemented by the fragment content and will be used to convert any
        /// changes back in to the root content.
        /// </summary>
        void AddBaseContent();
        /// <summary>
        /// This method is implemented by the fragment content and will be used to update any
        /// changes back in to the root content.
        /// </summary>
        /// <param name="baseContent">The base content to merge in to.</param>
        void UpdateBaseContent(IXimuraContent baseContent);
        /// <summary>
        /// This method is implemented by the fragment content and will be used to
        /// delete the root content.
        /// </summary>
        /// <param name="baseContent">The base content to merge in to.</param>
        void DeleteBaseContent(IXimuraContent baseContent);
        /// <summary>
        /// this method tells whether the fragment id is a by-reference id or not
        /// </summary>
        /// <returns>Returns turs if the fragment id is by-reference</returns>
        bool FragmentIDIsByReference();
        /// <summary>
        /// this method returns the fragment reference type
        /// </summary>
        /// <returns>fragment reference type</returns>
        string FragmentReferenceType();
        /// <summary>
        /// this method set the fragment reference type 
        /// </summary>
        /// <param name="referenceType"></param>
        void SetFragmentReferenceType(string referenceType);
        /// <summary>
        /// this method returns the fragment reference id
        /// </summary>
        /// <returns>fragment reference id</returns>
        string FragmentReferenceID();
        /// <summary>
        /// this method set the fragment reference id 
        /// </summary>
        /// <param name="referenceID"></param>
        void SetFragmentReferenceID(string referenceID);
    }
}
