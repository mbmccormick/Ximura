﻿#region Copyright
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
using System.Text;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Data
{
    public abstract partial class Content : IXimuraContent, IXimuraContentEntityPersistence, IXimuraContentSupportsDirty
    {
        #region Declarations
        /// <summary>
        /// This is the protected field used to record the dirty state of the Leaf.
        /// </summary>
        protected bool mDirty = false;
        /// <summary>
        /// This is the type name used for serialization.
        /// </summary>
        protected string mEntityType = "";
        /// <summary>
        /// This is the entity subtype string.
        /// </summary>
        protected string mEntitySubType = "";
        #endregion // Declarations

        #region Dirty
        /// <summary>
        /// This boolean property indicates whether the object has been changed.
        /// </summary>
        [Browsable(false), DefaultValue(false)]
        public virtual bool Dirty
        {
            get { return mDirty; }
            set
            {
                //This value is needed to stop cascading firing
                //bool old_Dirty = m_Dirty;
                mDirty = value;

                //Ok, we need to inform any subscribing objects that we have gone dirty
                //if (value && !old_Dirty && mDirtySet != null)
                //    foreach (DataGoneDirty gd in mDirtySet.GetInvocationList())
                //    {
                //        //Invoke the delegate syncronously, else strange things can happen
                //        //in the multithreaded world.
                //        gd(this);
                //    }
            }
        }
        #endregion // Dirty
        #region IsDirty()
        /// <summary>
        /// This is a public function that indicated whether the object
        /// internal data has been changed since it was created or last saved.
        /// </summary>
        /// <returns>A boolean value - true indicates the object has been changed.</returns>
        public virtual bool IsDirty()
        {
            return mDirty;
        }
        #endregion // IsDirty()

        #region EntityType
        /// <summary>
        /// This is the entity type used for searching.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string EntityType
        {
            get
            {
                if (mEntityType == null)
                {
                    XimuraContentTypeIDAttribute attrContentTypeID = 
                        AH.GetAttribute<XimuraContentTypeIDAttribute>(GetType());

                    if (attrContentTypeID == null)
                        mEntityType = "";
                    else
                        mEntityType = attrContentTypeID.Description;
                }

                return mEntityType;
            }
        }
        #endregion // EntityType
        #region EntitySubtype
        /// <summary>
        /// This is the entity subtype used for searching
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string EntitySubtype
        {
            get
            {
                return mEntitySubType;
            }
        }
        #endregion // EntitySubtype
        #region EntityAQN
        /// <summary>
        /// This is the assembly qualified name used for seaching. This
        /// may differ from the actual name as it will not have the specific
        /// version number to enable consistency across multiple version.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string EntityAQN
        {
            get
            {
                return GetType().FullName;
            }
        }
        #endregion // EntityAQN
        #region EntityName
        /// <summary>
        /// This is the entity name used for search display.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string EntityName
        {
            get
            {
                return "";
            }
        }
        #endregion // EntityName
        #region EntityDescription
        /// <summary>
        /// This is the entity description used for search display.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string EntityDescription
        {
            get
            {
                return "";
            }
        }
        #endregion // EntityDescription
    }
}
