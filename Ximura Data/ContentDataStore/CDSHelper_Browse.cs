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
{    /// <summary>
    /// This static class allows for dynamic updates to the CDS persistence service.
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
        public static CDSResponse CDSBrowse<E>(this IXimuraSessionRQ SessionRQ, out E data) where E : Content
        {
            //CDSContext pc = PersistenceContext.Initialize(typeof(E));
            //svc.Execute(typeof(E));

            //return (E)pc.ResponseData;
            data = null;
            return CDSResponse.BadRequest;
        }
        #endregion

        //#region Browse<T>
        //public static BrowseContext<T> Browse<T>(IXimuraSessionRQ SessionRQ) where T : Content
        //{
        //    return Browse<T>(SessionRQ, CDSBrowseConstraints.Relational);
        //}

        //public static BrowseContext<T> Browse<T>(IXimuraSessionRQ SessionRQ, CDSBrowseConstraints constraints) where T : Content
        //{
        //    return new BrowseContext<T>(SessionRQ, constraints);
        //}
        //#endregion // Browse<T>

        public class CDSBrowseContext<T> : IQueryable<T>, IQueryProvider
            where T : Content
        {
            #region Declarations
            private IXimuraSessionRQ mSessionRQ;
            private Expression mExpression = null;
            private IList<T> results;
            private CDSBrowseConstraints mConstraints;
            #endregion // Declarations
            #region Constructor
            /// <summary>
            /// This is the default constructor for the browse context.
            /// </summary>
            /// <param name="SessionRQ">The session object to execute under.</param>
            /// <param name="constraints">The browse constraints identifying what sort of request should be accepted.</param>
            public CDSBrowseContext(IXimuraSessionRQ SessionRQ, CDSBrowseConstraints constraints)
            {
                mSessionRQ = SessionRQ;
                mConstraints = constraints;
            }
            #endregion // Constructor

            #region IQueryProvider Members

            public IQueryable<S> CreateQuery<S>(Expression expression)
            {
                if (typeof(S) != typeof(T))
                    throw new Exception("Only " + typeof(T).FullName + " objects are supported.");

                this.mExpression = expression;

                return (IQueryable<S>)this;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return (IQueryable<T>)(this as IQueryProvider).CreateQuery<T>(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                MethodCallExpression methodcall = mExpression as MethodCallExpression;

                foreach (var param in methodcall.Arguments)
                {
                    ProcessExpression(param);
                }

                return (TResult)results.GetEnumerator();
            }

            public object Execute(Expression expression)
            {
                return (this as IQueryProvider).Execute<IEnumerator<T>>(expression);
            }

            #endregion

            #region IEnumerable<T> Members

            public IEnumerator<T> GetEnumerator()
            {
                return (this as IQueryable).Provider.Execute<IEnumerator<T>>(mExpression);
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator<T>)(this as IQueryable).GetEnumerator();
            }

            #endregion

            #region IQueryable Members

            public Type ElementType
            {
                get { return typeof(T); }
            }

            public Expression Expression
            {
                get { return Expression.Constant(this); }
            }

            public IQueryProvider Provider
            {
                get { return this; }
            }

            #endregion

            private void ProcessExpression(Expression expression)
            {
                //RQRSContract<CDSRequestFolder, CDSResponseFolder> rqEnv = null;
                //Content rsData = null;
                //try
                //{
                //    rqEnv = EnvelopeRequest(
                //        new EnvelopeAddress(scContentDataStore.CDSCommandID, ));
                //    //Get the request
                //    CDSRequestFolder rq = rqEnv.ContractRequest;
                //    //Set the value type
                //    rq.DataType = itemType;
                //    rq.ByReference = rqH.ByReference;
                //    rq.DataReferenceType = rqH.RefType;
                //    rq.DataReferenceValue = rqH.RefValue;

                //    if (rqH.IDContent.HasValue)
                //        rq.DataContentID = rqH.IDContent.Value;
                //    if (rqH.IDVersion.HasValue)
                //        rq.DataVersionID = rqH.IDVersion.Value;

                //    rq.Data = null;

                //    SessionRQ.ProcessRequest(rqEnv, rqH.Priority);

                //    CDSResponseFolder rs = rqEnv.ContractResponse;
                //    if (rqEnv.ContractRequest.InternalCall &&
                //        rqH.Action == CDSStateAction.ResolveReference)
                //    {
                //        cid = rq.DataContentID;
                //        vid = rq.DataVersionID;
                //    }
                //    else
                //    {
                //        cid = rs.CurrentContentID;
                //        vid = rs.CurrentVersionID;
                //    }

                //    rsData = rs.Data;

                //    return rs.Status;
                //}
                //catch (Exception ex)
                //{
                //    throw ex;
                //}
                //finally
                //{
                //    if (rsData != null && rsData.ObjectPoolCanReturn)
                //        rsData.ObjectPoolReturn();
                //    rqH.ObjectPoolReturn();
                //    EnvelopeReturn(rqEnv);
                //}
            }


        }
    }
}
