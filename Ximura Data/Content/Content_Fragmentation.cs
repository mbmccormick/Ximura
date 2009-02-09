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
using System.Drawing;
using System.Text;

using Ximura;
using Ximura.Data;
using Ximura.Data.Serialization;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
#endregion // using
namespace Ximura.Data
{
    public abstract partial class Content
    {
        #region Fragmentation Methods
        /// <summary>
        /// This property identifies whether the content is a fragment type.
        /// </summary>
        /// <returns>Returns true if the object is a fragment, or supports fragmentation.</returns>
        public virtual bool IsFragment()
        {
            return attrContentFragment != null;
        }
        /// <summary>
        /// This is the fragment base type. If this property is supported the persistence manager 
        /// will store this property in the serialization blob. Should the original type be unavailable 
        /// the content will be deserialized using this type.
        /// </summary>
        /// <returns>Return the base content type if supported.</returns>
        public virtual Type FragmentBaseType()
        {
            return null;
        }
        /// <summary>
        /// This method identifies whether the content supports the base type content.
        /// </summary>
        /// <returns>Returns true if this property is supported.</returns>
        public virtual bool SupportsFragmentBaseType()
        {
            return false;
        }
        /// <summary>
        /// This boolean property identifies whether a fragment can convert to a primary entity.
        /// </summary>
        /// <returns>Returns true if the entity can convert.</returns>
        public virtual bool CanConvertToPrimaryEntity()
        {
            return false;
        }
        /// <summary>
        /// This method converts the fragment entity to the primary entity.
        /// </summary>
        public virtual void ConvertFragmentToPrimaryEntity()
        {
            if (!CanConvertToPrimaryEntity())
                throw new NotImplementedException("ConvertFragmentToPrimaryEntity is not implemented");

            throw new NotImplementedException("ConvertFragmentToPrimaryEntity is not implemented");
        }
        /// <summary>
        /// This method is implemented by the fragment content and will be used to merge any
        /// changes back in to the root content.
        /// </summary>
        /// <param name="baseContent">The base content to merge in to.</param>
        public virtual void MergeContent(IXimuraContent baseContent)
        {
            throw new NotImplementedException("MergeContent is not implemented.");
        }
        /// <summary>
        /// This method is implemented by the fragment content and will be used to convert any
        /// changes back in to the root content.
        /// </summary>
        public virtual void AddBaseContent()
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// This method is implemented by the fragment content and will be used to update any
        /// changes back in to the root content.
        /// </summary>
        /// <param name="baseContent">The base content to merge in to.</param>
        public virtual void UpdateBaseContent(IXimuraContent baseContent)
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// This method is implemented by the fragment content and will be used to
        /// delete the root content.
        /// </summary>
        /// <param name="baseContent">The base content to merge in to.</param>
        public virtual void DeleteBaseContent(IXimuraContent baseContent)
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// this method tells whether the fragment id is a by-reference id or not
        /// </summary>
        /// <returns>Returns turs if the fragment id is by-reference</returns>
        public virtual bool FragmentIDIsByReference()
        {
            return false;
        }
        /// <summary>
        /// this method returns the fragment reference type
        /// </summary>
        /// <returns>fragment reference type</returns>
        public virtual string FragmentReferenceType()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// this method set the fragment reference type 
        /// </summary>
        /// <param name="referenceType"></param>
        public virtual void SetFragmentReferenceType(string referenceType)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// this method returns the fragment reference id
        /// </summary>
        /// <returns>fragment reference id</returns>
        public virtual string FragmentReferenceID()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// this method set the fragment reference id 
        /// </summary>
        /// <param name="referenceID"></param>
        public virtual void SetFragmentReferenceID(string referenceID)
        {
            throw new NotImplementedException();
        }
        #endregion // Fragmentation Methods

    }
}
