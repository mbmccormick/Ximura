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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;

using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Xml;

using Ximura;
using Ximura.Helper;
using CH=Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
#endregion // using
namespace Ximura.Server
{
	/// <summary>
	/// AppServerCommandExtender is used to start commands in the order specified within a command container.
	/// </summary>
	//[MetadataExtenderItem(typeof(AppServerCommandMetadataContainer))]
    [ProvideProperty("Priority", typeof(IXimuraCommandBase))]
	[ToolboxBitmap(typeof(XimuraResourcePlaceholder),"Ximura.Resources.ModelFolder2.bmp")]
	public class CommandMetadataExtender : MetadataExtender<IXimuraCommandBase, CommandMetadataContainer>
	{
		#region Declarations
        private SortedList<CommandMetadataContainer, IXimuraCommandBase> sList = null;
		#endregion // Declarations
		#region Constructors
		/// <summary>
		/// The default constructor.
		/// </summary>
		public CommandMetadataExtender():this(null){}
		/// <summary>
		/// The component model constructor.
		/// </summary>
		/// <param name="container">The container.</param>
		public CommandMetadataExtender(IContainer container):base(container)
		{
            sList = new SortedList<CommandMetadataContainer, IXimuraCommandBase>(new CommandPriorityExtender());	
		}
		#endregion

		#region GetPriority(IXimuraCommandBase item) 
		/// <summary>
		/// This property is used to return the start priority.
		/// </summary>
		/// <param name="item">The command object.</param>
		/// <returns>The command start priority.</returns>
		[DefaultValue(0), Category("Command Priority")]
		[Description("This is the command start priority. Setting a value of 0 indicates that the priority is unimportant.")]
        public int GetPriority(IXimuraCommandBase item) 
		{
			return ((CommandMetadataContainer)ItemGet(item)).Priority;
		}
		#endregion // GetPriority(IXimuraCommandBase item) 
		#region SetPriority(IXimuraCommandBase item, int value) 
		/// <summary>
		/// This property is used to set the command start priority.
		/// </summary>
		/// <param name="item">The command object.</param>
		/// <param name="value">The command start priority.</param>
        public void SetPriority(IXimuraCommandBase item, int value) 
		{
			CommandMetadataContainer mdItem;
			lock (this)
			{
				mdItem = ItemGet(item) as CommandMetadataContainer;
				mdItem.Priority=value;
			}
			RecalculatePriorities(mdItem);
		}
		#endregion // SetPriority(IXimuraCommandBase item, int value) 
		#region CommandHasPriority(object service)
		/// <summary>
		/// This method determines whether the command has a priority set.
		/// </summary>
		/// <param name="service">The command service</param>
		/// <returns>returns true if the command has a priority set.</returns>
		public bool CommandHasPriority(object service)
		{
			if (service == null)
				return false;

            IXimuraCommandBase item = service as IXimuraCommandBase;
			if (item == null || !sList.ContainsValue(item))
				return false;

            CommandMetadataContainer mdCont = this.mMetaData[item];

			return mdCont==null?false:mdCont.Priority>0;
		}
		#endregion // CommandHasPriority(object service)

        #region InternalStart/InternalStop
        /// <summary>
        /// This override does not implement any logic.
        /// </summary>
        protected override void InternalStart()
        {
            //base.InternalStart();
        }
        /// <summary>
        /// This override does not implement any logic.
        /// </summary>
        protected override void InternalStop()
        {
            //base.InternalStop();
        }
        #endregion // InternalStart/InternalStop

		#region StartCommandsInOrder()
		/// <summary>
		/// This method starts the commands in the order specified.
		/// </summary>
		public void StartCommandsInOrder()
		{
			foreach (CommandMetadataContainer item in sList.Keys)
			{
                try
                {
                    IXimuraCommandBase command = item.Command;
                    command.Start();
                }
                catch (Exception ex)
                {
                    string message = item.Command.CommandName + "-> Start error: " + ex.ToString();
                    throw new XimuraComponentServiceException(message, ex);
                }
			}
		}
		#endregion // StartCommandsInOrder()
        #region StopCommandsInReverseOrder()
        /// <summary>
        /// This method starts the commands in the order specified.
        /// </summary>
        public void StopCommandsInReverseOrder()
        {
            Stack<IXimuraCommandBase> closeStack = new Stack<IXimuraCommandBase>();

            foreach (CommandMetadataContainer item in sList.Keys)
            {
                IXimuraCommandBase command = item.Command;
                closeStack.Push(command);
            }

            while (closeStack.Count>0)
                closeStack.Pop().Stop();
        }
        #endregion // StopCommandsInReverseOrder()

        #region CommandsNotify(Type notificationType, object notification)
        /// <summary>
        /// This method notifies commands of specific system events in the same order that they were started.
        /// </summary>
        /// <param name="notificationType">The notification type.</param>
        /// <param name="notification">The notification object.</param>
        public void CommandsNotify(Type notificationType, object notification)
        {
            try
            {
                sList.Keys.ForEach(c =>
                {
                    try
                    {
                        if (c.Command.SupportsNotifications) 
                            c.Command.Notify(notificationType, notification);
                    }
                    catch (Exception ex)
                    {

                    }
                });
            }
            catch (Exception ex)
            {

            }
        }
        #endregion // CommandsNotify(Type notificationType, object notification)

        #region ItemGetNew
        /// <summary>
		/// This overriden method creates the new meta data container.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
        protected override CommandMetadataContainer ItemGetNew(IXimuraCommandBase key)
        {
            return new CommandMetadataContainer(key as IXimuraCommandBase);
        }
		#endregion // getNewItem
		#region RecalculatePriorities
		private void RecalculatePriorities(CommandMetadataContainer mdItem)
		{
			lock (sList)
			{
				if (sList.ContainsValue(mdItem.Command))
					sList.Remove(mdItem);

				sList.Add(mdItem,mdItem.Command);
			}
		}
		#endregion // RecalculatePriorities

		#region CommandPriorityExtender
		/// <summary>
		/// This class is used to set the command priority.
		/// </summary>
		public class CommandPriorityExtender : IComparer<CommandMetadataContainer>
		{
			#region Declarations
			int nullBubbleOrder;
			#endregion // Declarations
			#region Constructors
			/// <summary>
			/// This is the default constructor. Objects without a priority will go top. 
			/// </summary>
			internal CommandPriorityExtender():this(true){}
			/// <summary>
			/// This is the comparer.
			/// </summary>
			/// <param name="nullGoesBottom">Set this to true if you wish objects 
			/// without a priority to be started last.</param>
			internal CommandPriorityExtender(bool nullGoesBottom)
			{
				nullBubbleOrder = nullGoesBottom?-1:1;
			}
			#endregion // Constructors

            #region IComparer Members old
            ///// <summary>
            ///// This object compares two AppServerCommandMetadataContainer objects. The highest
            ///// priority will bubble to the top of the sorted list.
            ///// Null objects will also be set to the highest priority.
            ///// </summary>
            ///// <param name="x">The first object.</param>
            ///// <param name="y">The second object.</param>
            ///// <returns>Returns a positive integer if the first object is greater than the second.</returns>
            //public int Compare(object x, object y)
            //{
            //    AppServerCommandMetadataContainer aX = x as AppServerCommandMetadataContainer;
            //    AppServerCommandMetadataContainer aY = y as AppServerCommandMetadataContainer;

            //    if (aX == null && aY == null)
            //        return 0;
            //    if (aX == null || aX.Priority == 0)
            //        return nullBubbleOrder;
            //    if (aY == null || aY.Priority == 0)
            //        return nullBubbleOrder * -1;
            //    if (aY.Priority == aX.Priority)
            //        return nullBubbleOrder;

            //    return (aY.Priority-aX.Priority);
            //}
            #endregion

            #region IComparer<AppServerCommandMetadataContainer> Members
            /// <summary>
            /// This object compares two AppServerCommandMetadataContainer objects. The highest
            /// priority will bubble to the top of the sorted list.
            /// Null objects will also be set to the highest priority.
            /// </summary>
            /// <param name="aX">The first object.</param>
            /// <param name="aY">The second object.</param>
            /// <returns>Returns a positive integer if the first object is greater than the second.</returns>
            public int Compare(CommandMetadataContainer aX, CommandMetadataContainer aY)
            {
                if (aX == null && aY == null)
                    return 0;
                if (aX == null || aX.Priority == 0)
                    return nullBubbleOrder;
                if (aY == null || aY.Priority == 0)
                    return nullBubbleOrder * -1;
                if (aY.Priority == aX.Priority)
                    return nullBubbleOrder;

                return (aY.Priority - aX.Priority);
            }

            #endregion
        }
		#endregion // CommandPriorityExtender
    }

    #region CommandMetadataContainer
    /// <summary>
	/// This is the command meta data container.
	/// </summary>
	public class CommandMetadataContainer
	{
		#region Declarations
        private IXimuraCommandBase mCommand;
		private Hashtable extendedProperties = null;

		/// <summary>
		/// This is the enabled status.
		/// </summary>
		public int Priority = 0;
		#endregion // Declarations
		#region Constructors
		/// <summary>
		/// This is the primary constructor.
		/// </summary>
		public CommandMetadataContainer():this(null){}
		/// <summary>
		/// This is the primary constructor.
		/// </summary>
		/// <param name="theCommand">The command object.</param>
        public CommandMetadataContainer(IXimuraCommandBase theCommand)
		{
			mCommand = theCommand;
		}
		#endregion // Constructors

		#region Command
		/// <summary>
		/// This is the internal command stored in the metadata container.
		/// </summary>
        public IXimuraCommandBase Command
		{
			get{return mCommand;}
		}
		#endregion // Command

		#region Extended Properties
		/// <summary>
		/// This is the public accessor for the extended properties.
		/// </summary>
		public object this[object key]
		{
			get
			{
				if (extendedProperties == null ||
					!extendedProperties.ContainsKey(key))
					return null;

				return extendedProperties[key];
			}
			set
			{
				if (extendedProperties == null)
					extendedProperties = new Hashtable();

				extendedProperties[key]=value;
			}		
		}
		#endregion // Extended Properties
	}
	#endregion
}
