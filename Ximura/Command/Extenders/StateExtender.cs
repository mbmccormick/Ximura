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
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Drawing;
using System.Diagnostics;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using Ximura.Server;
using Ximura.Command;

#endregion // using
namespace Ximura.Command
{
	/// <summary>
	/// StateExtender contains the collection of states for the finite state machine.
	/// </summary>
	//[MetadataExtenderItem(typeof(StateMetadataContainer))]
    [ProvideProperty("StateID", "Ximura.IXimuraFSMState, Ximura")]
    [ProvideProperty("Enabled", "Ximura.IXimuraFSMState, Ximura")]
    [ProvideProperty("NextStateID", "Ximura.IXimuraFSMState, Ximura")]
    public class StateExtender<ST> :
        MetadataExtender<ST, StateMetadataContainer<ST>>, IXimuraFSMStateMetadataExtender<ST>, IXimuraStateExtender<ST>
        where ST : class, IXimuraFSMState
    {
		#region Declarations
        private IXimuraFSMExtenderBridge<ST> mExBase = null;
		private Dictionary<string, ST> mStateCollection= new Dictionary<string, ST>();
		private string mInitialState = null;
		#endregion
		#region Constructors
		/// <summary>
		/// The default constructor.
		/// </summary>
		public StateExtender():this(null){}
		/// <summary>
		/// The component model constructor.
		/// </summary>
		/// <param name="container">The container.</param>
		public StateExtender(IContainer container):base(container)
		{
		}
		#endregion

        #region ServicesProvide()
        /// <summary>
        /// This overriden method registers the ContextExtender with the Extender Bridge
        /// </summary>
        protected override void ServicesProvide()
        {
            base.ServicesProvide();
            if (ExtenderBridge != null)
                ExtenderBridge.StateEx = this as IXimuraFSMStateMetadataExtender<ST>;
        }
        #endregion // ServicesProvide()
        #region ServicesRemove()
        /// <summary>
        /// This overriden method unregisters the ContextExtender with the Extender Bridge if it is already registered.
        /// </summary>
        protected override void ServicesRemove()
        {
            if (ExtenderBridge != null && ExtenderBridge.StateEx == this)
                ExtenderBridge.StateEx = null;
            base.ServicesRemove();
        }
        #endregion // ServicesRemove()

 		#region StateID
		/// <summary>
		/// This property is used to return the state ID.
		/// </summary>
		/// <param name="state">The state.</param>
		/// <returns>The ID of the state.</returns>
		[DefaultValue(""),Category("State Metadata")]
        public string GetStateID(IXimuraFSMState state) 
		{
            return getItem((ST)state).StateID;
		}
		/// <summary>
		/// This property is used to set the state id
		/// </summary>
		/// <param name="state">The state object.</param>
		/// <param name="value">The state ID.</param>
        public void SetStateID(IXimuraFSMState state, string value) 
		{
            getItem((ST)state).StateID = value;
			if (mStateCollection.ContainsKey(value))
                mStateCollection[value] = (ST)state;
			else
                mStateCollection.Add(value, (ST)state);
            ((ST)state).Identifier = value;
		}
		#endregion			
		#region Enabled
		/// <summary>
		/// This property is used to return the enabled state.
		/// </summary>
		/// <param name="state">The state.</param>
		/// <returns>The enabled boolean value of the state.</returns>
		[DefaultValue(false),Category("State Metadata")]
        public bool GetEnabled(IXimuraFSMState state) 
		{
			return getItem((ST)state).Enabled;
		}
		/// <summary>
		/// This property is used to set the enabled state.
		/// </summary>
		/// <param name="state">The state object.</param>
		/// <param name="value">The state enabled value.</param>
        public void SetEnabled(IXimuraFSMState state, bool value) 
		{
            getItem((ST)state).Enabled = value;
		}
		#endregion			
        #region NextStateID
        /// <summary>
        /// This property is used to return the state ID.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The ID of the state.</returns>
        [DefaultValue(""), Category("State Metadata")]
        public string GetNextStateID(IXimuraFSMState state)
        {
            return getItem((ST)state).StateIDNext;
        }
        /// <summary>
        /// This property is used to set the state id
        /// </summary>
        /// <param name="state">The state object.</param>
        /// <param name="value">The state ID.</param>
        public void SetNextStateID(IXimuraFSMState state, string value)
        {
            getItem((ST)state).StateIDNext = value;
        }
        #endregion	

        #region GetStateIDList
        /// <summary>
        /// This method returns a collection of the available state ids in the collection.
        /// </summary>
        /// <returns>Returns a string array containing a string is for each of the states.</returns>
        public string[] GetStateIDList()
        {
            string[] ids = new string[this.mStateCollection.Keys.Count];

            this.mStateCollection.Keys.CopyTo(ids, 0);

            return ids;
        }
        #endregion // GetStateIDList

		#region Content Items
		/// <summary>
		/// This method returns the object based on its type.
		/// </summary>
		/// <param name="dataContentType">The type to find.</param>
		/// <returns></returns>
		public ST GetItemByType(Type dataContentType)
		{
			foreach(StateMetadataContainer<ST> item in mMetaData.Values)
			{
				if (item.State.GetType() == dataContentType)
					return item.State;
			}

			return null;
		}

		private StateMetadataContainer<ST> getItem(ST key)
		{
			lock (this)
			{
				if (mMetaData.ContainsKey(key))
                    return mMetaData[key] as StateMetadataContainer<ST>;
				else
				{
                    StateMetadataContainer<ST> item = new StateMetadataContainer<ST>(key);
					if (!Locked)
						mMetaData.Add(key,item);
					else
						XimuraAppTrace.WriteLine("Cannot add to metdata collection.");

					return item;
				}
			}
		}
		#endregion // Item

		#region GetState
		/// <summary>
		/// This method returns the state for the name passed to the connection.
		/// </summary>
		/// <param name="State">The state ID to return.</param>
		/// <returns>Null if the state cannot be found, or the state 
		/// object requested.</returns>
		public ST GetState(string State)
		{
			if (State!=null && mStateCollection.ContainsKey(State))
				return mStateCollection[State];

			return null;
		}
		#endregion // GetState

		#region GetInitialState
		/// <summary>
		/// This is the initial state for all new connections.
		/// </summary>
		/// <returns>The initial state for the protocol.</returns>
		public ST GetInitialState()
		{
			return GetState (InitialState);
		}
		#endregion // GetInitialState

		#region GetStateName
		/// <summary>
		/// This method returns the string identifier for the specified state.
		/// </summary>
		/// <param name="CurrentState">The state to identify.</param>
		/// <returns>Returns a string identifying the state.</returns>
		public virtual string GetStateName(ST CurrentState)
		{
			if (CurrentState == null)
				return null;

			return CurrentState.Identifier;
		}
		#endregion // GetStateName

		#region InitialState
		/// <summary>
		/// This is the ID of the initial state.
		/// </summary>
		[DefaultValue(null),Description("This is the ID of the initial state.")]
		public string InitialState
		{
			get{return mInitialState;}
			set{mInitialState=value;}
		}
		#endregion // InitialState

        #region ExtenderBridge
        /// <summary>
        /// This read only property is the service bridge object from
        /// the protocol to the extenders.
        /// </summary>
        protected IXimuraFSMExtenderBridge<ST> ExtenderBridge
        {
            get
            {
                if (mExBase != null)
                    return mExBase;

                mExBase = GetService(
                    typeof(IXimuraFSMExtenderBridge<ST>)) as IXimuraFSMExtenderBridge<ST>;

                return mExBase;
            }
        }
        #endregion // ProtocolBridge

        #region IXimuraFSMStateExtender<ST> Members

        public virtual bool HasNextState(string State)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public virtual ST GetNextState(string State)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

        #region InternalStart()/InternalStop()
        /// <summary>
        /// This method starts the states registered in the collection.
        /// </summary>
        protected override void InternalStart()
        {
            try
            {
                mStateCollection
                    .Select(s => s.Value)
                    .Where(s => s is IXimuraService)
                    .Cast<IXimuraService>()
                    .ForEach(s => s.Start());

            }
            catch (Exception ex)
            {

                Stop();
                throw ex;
            }
        }
        /// <summary>
        /// This method stops the states registered in the collection.
        /// </summary>
        protected override void InternalStop()
        {
            mStateCollection
                .Select(s => s.Value)
                .Where(s => s is IXimuraService)
                .Cast<IXimuraService>()
                .Where(s => s.ServiceStatus == XimuraServiceStatus.Started)
                .ForEach(s => s.Stop());
        }
        #endregion // InternalStart/InternalStop
    }
}