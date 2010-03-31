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
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using System.Security.Cryptography;
using System.Globalization;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Data;
using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
#endregion // using
namespace Ximura.Data
{
    /// <summary>
    /// This staic class allows for dynamic updates to the persistence service.
    /// </summary>
    public static partial class CDSHelper
    {
        #region Initialize
        /// <summary>
        /// This method initializes the entity from the persistence store.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="svc">The persistence service.</param>
        /// <returns>Returns the entity.</returns>
        public static CDSResponse CDSConstruct<E>(this IXimuraSessionRQ SessionRQ, out E data) where E : Content
        {
            return SessionRQ.CDSExecute<E>(CDSData.Get(CDSAction.Construct), out data);

        }
        #endregion



        //        #region Update
        //        /// <summary>
        //        /// 
        //        /// </summary>
        //        /// <typeparam name="E"></typeparam>
        //        /// <param name="svc"></param>
        //        /// <param name="entity"></param>
        //        /// <returns></returns>
        //        public static E Update<E>(this IPersistenceService svc, E entity) where E : Entity
        //        {
        //            return svc.Update<E>(entity, (Guid?)null, (Guid?)null);
        //        }

        //        /// <summary>
        //        /// 
        //        /// </summary>
        //        /// <typeparam name="E"></typeparam>
        //        /// <param name="svc"></param>
        //        /// <param name="entity"></param>
        //        /// <param name="lockNew"></param>
        //        /// <param name="lockCurrent"></param>
        //        /// <returns></returns>
        //        public static E Update<E>(this IPersistenceService svc, E entity,
        //            Guid? lockNew, Guid? lockCurrent) where E : Entity
        //        {
        //            Guid oldVersion = entity.IDVersion;
        //            entity.IDVersion = Guid.NewGuid();

        //            PersistenceContext pc = PersistenceContext.Update<PersistenceContext>(
        //                entity, oldVersion
        //                , lockNew.HasValue ? new EntityLockID(lockNew.Value) : (EntityLockID?)null
        //                , lockCurrent.HasValue ? new EntityLockID(lockCurrent.Value) : (EntityLockID?)null
        //                );

        //            svc.Execute(pc);

        //            return (E)pc.ResponseData;
        //        }
        //        /// <summary>
        //        /// 
        //        /// </summary>
        //        /// <typeparam name="E"></typeparam>
        //        /// <param name="svc"></param>
        //        /// <param name="entity"></param>
        //        /// <param name="lockNew"></param>
        //        /// <param name="lockCurrent"></param>
        //        /// <returns></returns>
        //        public static E Update<E>(this IPersistenceService svc, E entity, EntityLockID? lockNew, EntityLockID? lockCurrent) where E : Entity
        //        {
        //            Guid oldVersion = entity.IDVersion;
        //            entity.IDVersion = Guid.NewGuid();

        //            PersistenceContext pc = PersistenceContext.Update<PersistenceContext>(
        //                entity, oldVersion, lockNew, lockCurrent);
        //            svc.Execute(pc);

        //            return (E)pc.ResponseData;
        //        }
        //        #endregion // Update

        //        #region Lock
        //        public static bool Lock<E>(this IPersistenceService svc, Guid IDContent, Guid newLock) where E : Entity
        //        {
        //            PersistenceContext pc = PersistenceContext.Lock(typeof(E), IDContent, newLock, (Guid?)null);
        //            svc.Execute(pc);

        //            return pc.ResponseStatus == 201;
        //        }
        //        public static bool Lock(this IPersistenceService svc, Type entityType, Guid IDContent, Guid newLock)
        //        {
        //            PersistenceContext pc = PersistenceContext.Lock(entityType, IDContent, newLock, (Guid?)null);
        //            svc.Execute(pc);

        //            return pc.ResponseStatus == 201;
        //        }



        //        public static bool Lock<E>(this IPersistenceService svc, Guid IDContent, Guid newLock, Guid? oldLock) where E : Entity
        //        {
        //            PersistenceContext pc = PersistenceContext.Lock(typeof(E), IDContent, newLock, oldLock);
        //            svc.Execute(pc);

        //            return pc.ResponseStatus == 201;
        //        }

        //        public static bool Lock(this IPersistenceService svc, Type entityType, Guid IDContent, Guid newLock, Guid? oldLock)
        //        {
        //            PersistenceContext pc = PersistenceContext.Lock(entityType, IDContent, newLock, oldLock);
        //            svc.Execute(pc);

        //            return pc.ResponseStatus == 201;
        //        }


        //        public static bool Lock<E>(this IPersistenceService svc, Guid IDContent, EntityLockID newLock) where E : Entity
        //        {
        //            PersistenceContext pc = PersistenceContext.Lock(typeof(E), IDContent, newLock, (Guid?)null);
        //            svc.Execute(pc);

        //            return pc.ResponseStatus == 201;
        //        }

        //        public static bool Lock(this IPersistenceService svc, Type entityType, Guid IDContent, EntityLockID newLock)
        //        {
        //            PersistenceContext pc = PersistenceContext.Lock(entityType, IDContent, newLock, (Guid?)null);
        //            svc.Execute(pc);

        //            return pc.ResponseStatus == 201;
        //        }


        //        public static bool Lock<E>(this IPersistenceService svc, Guid IDContent, EntityLockID newLock, Guid? oldLock) where E : Entity
        //        {
        //            PersistenceContext pc = PersistenceContext.Lock(typeof(E), IDContent, newLock, oldLock);
        //            svc.Execute(pc);

        //            return pc.ResponseStatus == 201;
        //        }


        //        public static bool Lock(this IPersistenceService svc, Type entityType, Guid IDContent, EntityLockID newLock, Guid? oldLock)
        //        {
        //            PersistenceContext pc = PersistenceContext.Lock(entityType, IDContent, newLock, oldLock);
        //            svc.Execute(pc);

        //            return pc.ResponseStatus == 201;
        //        }

        //        #endregion

        //        #region Unlock

        //        public static bool Unlock<E>(this IPersistenceService svc, Guid IDContent, Guid currentLockID) where E : Entity
        //        {
        //            PersistenceContext pc = PersistenceContext.Unlock(typeof(E), IDContent, currentLockID);
        //            svc.Execute(pc);

        //            return pc.ResponseStatus == 201;
        //        }


        //        public static bool Unlock(this IPersistenceService svc, Type entityType, Guid IDContent, Guid currentLockID)
        //        {
        //            PersistenceContext pc = PersistenceContext.Unlock(entityType, IDContent, currentLockID);
        //            svc.Execute(pc);

        //            return pc.ResponseStatus == 201;
        //        }

        //        #endregion

        //        #region Promote<E>(this IPersistenceService svc, E entity, Guid userID)
        //        /// <summary>
        //        /// This method promotes the entity.
        //        /// </summary>
        //        /// <typeparam name="E">The entity type. The entity should implement ISupportsIssue</typeparam>
        //        /// <param name="svc">The persistence service.</param>
        //        /// <param name="entity">The entity to promote.</param>
        //        /// <param name="userID">The ID of the user performing the action.</param>
        //        /// <returns>Returns the status code from the perssitence layer.</returns>
        //        public static int Promote<E>(this IPersistenceService svc, E entity, Guid userID) where E : ISupportsIssue
        //        {
        //            PersistenceContext pc = PersistenceContext.Promote(entity, userID);
        //            svc.Execute(pc);

        //            return pc.ResponseStatus;
        //        }
        //        #endregion // Promote<E>(this IPersistenceService svc, E entity, Guid userID)

        //        #region Demote<E>(this IPersistenceService svc, E entity, Guid userID)
        //        /// <summary>
        //        /// This method demoted the entity.
        //        /// </summary>
        //        /// <typeparam name="E">The entity type. The entity should implement ISupportsIssue</typeparam>
        //        /// <param name="svc">The persistence service.</param>
        //        /// <param name="entity">The entity to demote.</param>
        //        /// <param name="userID">The ID of the user performing the action.</param>
        //        /// <returns>Returns the status code from the perssitence layer.</returns>
        //        public static int Demote<E>(this IPersistenceService svc, E entity, Guid userID) where E : ISupportsIssue
        //        {
        //            PersistenceContext pc = PersistenceContext.Demote(entity, userID);
        //            svc.Execute(pc);

        //            return pc.ResponseStatus;
        //        }
        //        #endregion // Demote<E>(this IPersistenceService svc, E entity, Guid userID)


        //        public static E Browse<E>(this IPersistenceService svc) where E : Entity, new()
        //        {
        //            E sr = new E();
        //            return svc.Browse<E>(sr);
        //        }

        //        public static E Browse<E>(this IPersistenceService svc, E searchResult) where E : Entity, new()
        //        {
        //            PersistenceContext pc = PersistenceContext.Browse(searchResult);
        //            svc.Execute(pc);

        //            return (E)pc.ResponseData;
        //        }

    }
}
