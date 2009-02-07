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
#endregion // using
namespace XIMS.Helper.Collections
{
	/// <summary>
	/// This event will be fired when the collection has changed.
	/// </summary>
	public delegate void CollectionChanged();

	/// <summary>
	/// This event will be fired when the collection will change.
	/// </summary>
	public delegate void CollectionChange(int index, object value);

	/// <summary>
	/// CollectionWithEvents is a class derived from CollectionBase that has the
	/// additional properties that allow events to be raised when the collection
	/// changes.
	/// </summary>
	public class CollectionWithEvents : CollectionBase
	{
		// Collection change events
		public event CollectionChanged Clearing;
		public event CollectionChanged Cleared;
		public event CollectionChanged Changed;
		public event CollectionChange Inserting;
		public event CollectionChange Inserted;
		public event CollectionChange Removing;
		public event CollectionChange Removed;
	
		protected void EventHelper(CollectionChanged theEvent)
		{
			if (theEvent != null)
			{
				theEvent();
				EventHelper(Changed);
			}
		}
		protected void EventHelper(CollectionChange theEvent,int index, object value)
		{
			if (theEvent != null)
			{
				theEvent(index, value);
				EventHelper(Changed);
			}
		}

		// Overrides for generating events
		protected override void OnClear()
		{
			// Any attached event handlers?
			EventHelper(Clearing);
		}	

		protected override void OnClearComplete()
		{
			// Any attached event handlers?
			EventHelper(Cleared);
		}	

		protected override void OnInsert(int index, object value)
		{
			// Any attached event handlers?
			EventHelper(Inserting,index, value);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			// Any attached event handlers?
			EventHelper(Inserted,index, value);
		}

		protected override void OnRemove(int index, object value)
		{
			// Any attached event handlers?
			EventHelper(Removing,index, value);
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			// Any attached event handlers?
			EventHelper(Removed,index, value);
		}

		protected int IndexOf(object value)
		{
			// Find the 0 based index of the requested entry
			return base.List.IndexOf(value);
		}
	}
}
