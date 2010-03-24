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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using Ximura;
using Ximura.Framework;
using CH = Ximura.Helper.Common;
using RH = Ximura.Helper.Reflection;

#endregion // using
namespace Ximura
{
	/// <summary>
	/// MetadataExtender is the base class for all extenders.
	/// </summary>
	public class MetadataExtender<T,C>: XimuraComponentService, IExtenderProvider, ISupportInitialize
        where C : new()
        where T : class
	{
		#region Declarations
		/// <summary>
		/// This property indicates whether the collection is locked. A locked
		/// collection will not create data record automatically.
		/// </summary>
		protected bool mLocked = false;
		/// <summary>
		/// This is the hashtable for the extended properties.
		/// </summary>
        protected Dictionary<T, C> mMetaData = new Dictionary<T, C>();
		/// <summary>
		/// This is the initialization counter.
		/// </summary>
		protected int mInitializationCount=0;
		/// <summary>
		/// This is the attribute for the class that identifies the internal
		/// content class.
		/// </summary>
		//protected MetadataExtenderItemAttribute attrItem = null;

		#endregion
		#region Constructors
		/// <summary>
		/// This is the empty constructor.
		/// </summary>
		public MetadataExtender():this(null){}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
		/// <param name="container">The container.</param>
		public MetadataExtender(IContainer container):base(container)
		{
			//SetItemAttribute();
		}
		#endregion

		#region CanExtend
		/// <summary>
		/// This method is used to check whether the object can be extended.
		/// </summary>
		/// <param name="extendee">The object to extend</param>
		/// <returns>returns true if the object can be extended.</returns>
        public virtual bool CanExtend(object extendee)
        {
            if (extendee == null)
                return false;

            T extendeeT = null;

            if (extendee is T)
                try
                {
                    extendeeT = (T)extendee;
                }catch { }

            if (extendeeT == null)
                return false;

            if (!mMetaData.ContainsKey(extendeeT))
                ItemGet(extendeeT);

            return true;
        }
		#endregion

		#region ISupportInitialize Members
		/// <summary>
		/// This method is called at the beginning of the initialization phase.
		/// </summary>
		public void BeginInit()
		{
			lock (this)
			{
				Locked = ++mInitializationCount<=0;
			}			
		}

		/// <summary>
		/// This method is called at the end of the initialization phase.
		/// </summary>
		public void EndInit()
		{
			lock (this)
			{
				Locked = --mInitializationCount<=0;
			}
		}

		#region Locked
		/// <summary>
		/// This property identifies whether the meta data container is locked.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Locked
		{
			get{return mLocked && (this.Site==null?true:!this.Site.DesignMode);}
			set{mLocked=value;}
		}
		#endregion // Locked

		#endregion

		#region ContentCollection
		/// <summary>
		/// This is the content collections.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual IEnumerable<T> ContentCollection()
		{
            foreach (T key in mMetaData.Keys)
                yield return key;
            //get
            //{
            //    //return new ArrayList(metaData.Keys) as ICollection;
            //}
		}
		#endregion // ContentCollection

        #region ItemAdd
        /// <summary>
		/// This methods adds an item to the collection.
		/// </summary>
		/// <param name="key">The item to add.</param>
		public virtual void ItemAdd(T key)
		{
			lock (this)
			{
				bool mOldLocked = Locked;
				Locked=false;
				if (!mMetaData.ContainsKey(key))
					ItemGet(key);
				Locked=mOldLocked;
			}
		}
		#endregion // AddItem
        #region ItemRemove
        /// <summary>
		/// This method removes an item from the collection.
		/// </summary>
		/// <param name="key">The object.</param>
		public virtual void ItemRemove(T key)
		{
			lock (this)
			{
				if (!mMetaData.ContainsKey(key))
					mMetaData.Remove(key);
			}
		}
		#endregion

        #region ItemGet
        /// <summary>
		/// This method gets an object from the collection.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual C ItemGet(T key)
		{
			lock (this)
			{
				if (mMetaData.ContainsKey(key))
					return mMetaData[key];
				else
				{
					C item = ItemGetNew(key);

					if (!Locked)
						mMetaData.Add(key, item);
					else
						XimuraAppTrace.WriteLine("Cannot add to metdata collection.");

					return item;
				}
			}		
		}
		#endregion // getItem
        #region ItemGetNew() / ItemGetNew(T key)
        /// <summary>
		/// This method creates a new object based on the type specified in the 
		/// MetadataExtenderItem type attribute.
		/// </summary>
		/// <param name="key">The object to create the meta data container for, or null if 
		/// called by getNewItem().</param>
		/// <returns>The new item.</returns>
		/// <remarks>You should override this method if you wish to pass the creation
		/// object in to the meta data container.</remarks>
		protected virtual C ItemGetNew(T key)
		{
            return ItemGetNew();
		}
		/// <summary>
		/// This method creates a new object based on the type specified in the 
		/// MetadataExtenderItem type attribute.
		/// </summary>
		/// <returns>The new item.</returns>
		protected virtual C ItemGetNew()
		{
            C newItem = (C)RH.CreateObjectFromType(typeof(C));

            return newItem;
        }
		#endregion // getNewItem
	}
}