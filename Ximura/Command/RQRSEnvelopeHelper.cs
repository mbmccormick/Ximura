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
using System.Data;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using RH = Ximura.Helper.Reflection;
#endregion // using
namespace Ximura.Framework
{
    /// <summary>
    /// The RQRSEnvelopeHelper is used to centralize pooling for the RQRSContract objects.
    /// </summary>
    public static class RQRSEnvelopeHelper
    {
        #region Declarations
        static IXimuraPoolManager sPoolManager;
        static Dictionary<Guid, Type> sContractIDandType;
        static Dictionary<Guid, Type> sContractCallbackIDandType;
        static Dictionary<Guid, string> sContractIDandName;
        static List<IXimuraPoolBuffer> sBufferList;
        static bool sStarted = false;

        static object syncObject = new object();
        #endregion

        #region Static Constructor
        /// <summary>
        /// This is the static constuctor for the class.
        /// </summary>
        static RQRSEnvelopeHelper()
        {

        }
        #endregion

        #region Start
        /// <summary>
        /// This method starts the pooling.
        /// </summary>
        public static void Start(IXimuraPoolManager poolManager)
        {
            if (sStarted)
                return;

            //Set the pool manager.
            sPoolManager = poolManager;

            lock (syncObject)
            {
                sContractIDandType = new Dictionary<Guid, Type>();
                sContractCallbackIDandType = new Dictionary<Guid, Type>();
                sContractIDandName = new Dictionary<Guid, string>();
                sBufferList = new List<IXimuraPoolBuffer>();
                sStarted = true;
            }
        }
        #endregion
        #region Stop
        /// <summary>
        /// This method stops the pooling and clears any remaining objects..
        /// </summary>
        public static void Stop()
        {
            if (sBufferList == null && sContractIDandType == null)
                return;

            if (sBufferList != null)
            {
                //Clear all the buffer references.
                foreach (IXimuraPoolBuffer pBuffer in sBufferList)
                    pBuffer.ResetBuffer();
                sBufferList.Clear();
            }

            //Get rid of the command references
            if (sContractIDandType != null)
            {
                sContractIDandType.Clear();
                sContractIDandType = null;
            }
            if (sContractCallbackIDandType != null)
            {
                sContractCallbackIDandType.Clear();
                sContractCallbackIDandType = null;
            }
        }
        #endregion

        #region Get
        /// <summary>
        /// This method get the appropriate EnvelopeContract based on the command ID in the address. By
        /// default the address is set to the EnvelopeContract, and the request id is set to new ID.
        /// </summary>
        /// <param name="address">The envelope address. 
        /// This will be set to the EnvelopeAddress property in the envelope.</param>
        /// <returns>Returns the EnvelopeContract.</returns>
        public static IXimuraRQRSEnvelope Get(EnvelopeAddress address)
        {
            return Get(address, true, Guid.NewGuid());
        }
        /// <summary>
        /// This method get the appropriate EnvelopeContract based on the command ID in the address. By
        /// default the address is set to the EnvelopeContract, and the request id is set to new ID.
        /// </summary>
        /// <param name="address">The envelope address. 
        /// This will be set to the EnvelopeAddress property in the envelope.</param>
        /// <returns>Returns the EnvelopeContract.</returns>
        public static IXimuraRQRSEnvelope GetCallback(EnvelopeAddress address)
        {
            return GetCallback(address, true, Guid.NewGuid());
        }
        /// <summary>
        /// This method get the appropriate EnvelopeContract based on the command ID in the address. By
        /// default the address is set to the EnvelopeContract.
        /// </summary>
        /// <param name="address">The envelope address. This will be set to the EnvelopeAddress property in the envelope.</param>
        /// <param name="requestID">The required request id. Set this to null if you do not want the ID set.</param>
        /// <returns>Returns the EnvelopeContract.</returns>
        public static IXimuraRQRSEnvelope Get(EnvelopeAddress address, Guid? requestID)
        {
            return Get(address, true, requestID);
        }
        /// <summary>
        /// This method get the appropriate EnvelopeContract based on the command ID in the address. By
        /// default the address is set to the EnvelopeContract.
        /// </summary>
        /// <param name="address">The envelope address. This will be set to the EnvelopeAddress property in the envelope.</param>
        /// <param name="requestID">The required request id. Set this to null if you do not want the ID set.</param>
        /// <returns>Returns the EnvelopeContract.</returns>
        public static IXimuraRQRSEnvelope GetCallback(EnvelopeAddress address, Guid? requestID)
        {
            return GetCallback(address, true, requestID);
        }
        /// <summary>
        /// This method get the appropriate EnvelopeContract based on the command ID in the address.
        /// </summary>
        /// <param name="address">The envelope address.</param>
        /// <param name="setAddress">If this parameter is set to true the addess will be set to the Envelope.</param>
        /// <param name="requestID">The required request id. Set this to null if you do not want the ID set.</param>
        /// <returns>Returns the EnvelopeContract.</returns>
        public static IXimuraRQRSEnvelope Get(EnvelopeAddress address, bool setAddress, Guid? requestID)
        {
            IXimuraRQRSEnvelope Env = Get(address.command);
            if (setAddress)
                Env.DestinationAddress = address;

            if (requestID.HasValue)
                Env.Request.ID = requestID.Value;

            return Env;
        }
        /// <summary>
        /// This method get the appropriate EnvelopeContract based on the command ID in the address.
        /// </summary>
        /// <param name="address">The envelope address.</param>
        /// <param name="setAddress">If this parameter is set to true the addess will be set to the Envelope.</param>
        /// <param name="requestID">The required request id. Set this to null if you do not want the ID set.</param>
        /// <returns>Returns the EnvelopeContract.</returns>
        public static IXimuraRQRSEnvelope GetCallback(EnvelopeAddress address, bool setAddress, Guid? requestID)
        {
            IXimuraRQRSEnvelope Env = GetCallback(address.command);
            if (setAddress)
                Env.DestinationAddress = address;

            if (requestID.HasValue)
                Env.Request.ID = requestID.Value;

            return Env;
        }
        /// <summary>
        /// This static method returns an EnvelopeContract from the pool for the specific
        /// command type.
        /// </summary>
        /// <param name="commandID">The command ID.</param>
        /// <returns>Returns a EnvelopeContract for the specific command ID. If the command
        /// is not registered for the application, this method will return null.</returns>
        public static IXimuraRQRSEnvelope Get(Guid commandID)
        {
            IXimuraPool pool = GetPoolManager(commandID);

            if (pool == null)
                return null;

            return pool.Get() as IXimuraRQRSEnvelope;
        }
        /// <summary>
        /// This static method returns an EnvelopeContract from the pool for the specific
        /// command type.
        /// </summary>
        /// <param name="commandID">The command ID.</param>
        /// <returns>Returns a EnvelopeContract for the specific command ID. If the command
        /// is not registered for the application, this method will return null.</returns>
        public static IXimuraRQRSEnvelope GetCallback(Guid commandID)
        {
            IXimuraPool pool = GetPoolManagerCallback(commandID);

            if (pool == null)
                return null;

            return pool.Get() as IXimuraRQRSEnvelope;
        }
        /// <summary>
        /// This static method returns an EnvelopeContract from the pool for the specific
        /// object type.
        /// </summary>
        /// <param name="objectType">The envelope type.</param>
        /// <returns>Returns a EnvelopeContract for the specific command ID. If the command
        /// is not registered for the application, this method will return null.</returns>
        public static IXimuraRQRSEnvelope Get(Type objectType)
        {
            IXimuraPool pool = GetPoolManagerInternal(objectType, false);

            if (pool == null)
                return null;

            return pool.Get() as IXimuraRQRSEnvelope;
        }

        /// <summary>
        /// This static method returns an EnvelopeContract from the pool for the specific
        /// object type.
        /// </summary>
        /// <returns>Returns a EnvelopeContract for the specific command ID. If the command
        /// is not registered for the application, this method will return null.</returns>
        public static RQRSContract<RQ, RS> Get<RQ, RS>()
            where RS : RQRSFolder, new()
            where RQ : RQRSFolder, new()
        {
            IXimuraPool<RQRSContract<RQ, RS>> pool =
                GetPoolManagerInternal<RQ, RS>(false);

            if (pool == null)
                return null;

            return pool.Get();
        }
        #endregion

        #region ResolveType/ResolveTypeCallback
        /// <summary>
        /// This method returns the envelope request type for registered commands.
        /// </summary>
        /// <param name="commandID">The command ID.</param>
        /// <returns>Returns the type, or null if the command cannot be found.</returns>
        public static Type ResolveType(Guid commandID)
        {
            if (sContractIDandType == null || !sContractIDandType.ContainsKey(commandID))
            {
                return null;
            }

            return sContractIDandType[commandID];
        }
        /// <summary>
        /// This method resolves the envelope callback type for registered commands.
        /// </summary>
        /// <param name="commandID">The command ID.</param>
        /// <returns>Returns the type, or null if the command cannot be found.</returns>
        public static Type ResolveTypeCallback(Guid commandID)
        {
            if (sContractCallbackIDandType == null || !sContractCallbackIDandType.ContainsKey(commandID))
            {
                return null;
            }

            return sContractCallbackIDandType[commandID];
        }
        #endregion // ResolveType
        #region GetPoolManager/GetPoolManagerCallback
        /// <summary>
        /// This method returns the pool manager based on the command ID type.
        /// </summary>
        /// <param name="commandID">The command ID to resolve.</param>
        /// <returns>Returns the pool, or null if the pool cannot be resolved.</returns>
        public static IXimuraPool GetPoolManager(Guid commandID)
        {
            if (commandID == Guid.Empty)
                return null;

            return GetPoolManager(ResolveType(commandID));
        }
        /// <summary>
        /// This method returns the callback pool manager based on the command ID type.
        /// </summary>
        /// <param name="commandID">The command ID to resolve.</param>
        /// <returns>Returns the pool, or null if the pool cannot be resolved.</returns>
        public static IXimuraPool GetPoolManagerCallback(Guid commandID)
        {
            if (commandID == Guid.Empty)
                return null;

            return GetPoolManager(ResolveTypeCallback(commandID));
        }
        /// <summary>
        /// This method returns the pool manager based on the object type of the envelope.
        /// </summary>
        /// <param name="objectType">The envelope object type.</param>
        /// <returns>Returns a pool manager object or null if the type is null or cannot be resolved..</returns>
        public static IXimuraPool GetPoolManager(Type objectType)
        {
            if (objectType == null)
                return null;

            return GetPoolManagerInternal(objectType, true);
        }
        #endregion

        #region GetPoolManagerInternal
        /// <summary>
        /// This method returns a unbuffered pool manager for the type specified.
        /// </summary>
        /// <param name="objectType">The envelope type required.</param>
        /// <param name="buffered">This parameter specifies whether the pool should be wrapped
        /// around a buffered wrapper to disable the Clear() method.</param>
        /// <returns></returns>
        private static IXimuraPool 
            GetPoolManagerInternal(Type objectType, bool buffered)
        {
            if (!RH.ValidateInterface(objectType, typeof(IXimuraRQRSEnvelope)))
                throw new ArgumentException("envelopeType must inherit from RQRSContract<>", "envelopeType");

            IXimuraPool pool = sPoolManager.GetPoolManager(objectType, buffered);

            lock (syncObject)
            {
                if (buffered && pool != null && pool is IXimuraPoolBuffer)
                    sBufferList.Add((IXimuraPoolBuffer)pool);
            }

            return pool;
        }

        /// <summary>
        /// This method returns a unbuffered pool manager for the type specified.
        /// </summary>
        /// <param name="objectType">The envelope type required.</param>
        /// <param name="buffered">This parameter specifies whether the pool should be wrapped
        /// around a buffered wrapper to disable the Clear() method.</param>
        /// <returns></returns>
        private static IXimuraPool<RQRSContract<RQ, RS>> 
            GetPoolManagerInternal<RQ, RS>(bool buffered)
            where RQ : RQRSFolder, new()
            where RS : RQRSFolder, new()
        {
            IXimuraPool<RQRSContract<RQ, RS>> pool =
                sPoolManager.GetPoolManager(typeof(RQRSContract<RQ, RS>), buffered)
                    as IXimuraPool<RQRSContract<RQ, RS>>;

            lock (syncObject)
            {
                if (buffered && pool != null && pool is IXimuraPoolBuffer)
                    sBufferList.Add((IXimuraPoolBuffer)pool);
            }

            return pool;
        }
        #endregion

        #region RegisterCommand
        /// <summary>
        /// This method registers the command in the object pool. If the object pool type does not exist the command
        /// creates a new pool.
        /// </summary>
        /// <param name="commandID">The command ID to register.</param>
        /// <param name="commandName">This is the name of the command which is used by the performance counters.</param>
        /// <param name="objectType">The envelope type to register.</param>
        /// <param name="objectCallbackType">The envelope callback type to register.</param>
        public static void RegisterCommand(Guid commandID, string commandName,
            Type objectType, Type objectCallbackType)
        {
            if (!sStarted)
                throw new ArgumentNullException("The manager has not started.");

            if (!RH.ValidateInterface(objectType, typeof(IXimuraRQRSEnvelope)))
                throw new ArgumentException("envelopeType must inherit from RQRSContract<>", "envelopeType");
            if (!RH.ValidateInterface(objectCallbackType, typeof(IXimuraRQRSEnvelope)))
                throw new ArgumentException("objectCallbackType must inherit from RQRSContract<>", "objectCallbackType");

            lock (syncObject)
            {
                //Add the command ID and the contract type to the collection.
                if (!sContractIDandType.ContainsKey(commandID))
                    sContractIDandType.Add(commandID, objectType);

                if (!sContractCallbackIDandType.ContainsKey(commandID))
                    sContractCallbackIDandType.Add(commandID, objectCallbackType);

                if (!sContractIDandName.ContainsKey(commandID))
                    sContractIDandName.Add(commandID, commandName);
            }
        }
        #endregion
    }
}
