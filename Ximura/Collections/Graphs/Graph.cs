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
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    //public class Graph<VRefType, ERefType>: IGraph
    //{
    //    #region Declarations
    //    VertexCollection<VRefType> mVertexes;
    //    Dictionary<string, IVertexAttributeCollection> mVertexAttributes;
    //    Dictionary<string, IEdgeAttributeCollection> mEdgeAttributes;
    //    EdgeCollection<ERefType> mEdges;
    //    #endregion // Declarations
    //    #region Constructors
    //    public Graph()
    //    {
    //        Initialize();
    //    }

    //    public Graph(IEnumerable<VRefType> vertexes):this()
    //    {
    //        Initialize();
    //    }
    //    #endregion // Constructors


    //    protected virtual void Initialize()
    //    {
    //        mVertexes = new VertexCollection<VRefType>();
    //        mEdges = new EdgeCollection<ERefType>();
    //    }

    //    #region Unused
    //    //public class BrowseContext : IQueryable<VertexResult>, IQueryProvider
    //    //{
    //    //    #region Declarations
    //    //    private Expression mExpression = null;
    //    //    private IList<VertexResult> results;
    //    //    #endregion // Declarations
    //    //    #region Constructor
    //    //    /// <summary>
    //    //    /// This is the default constructor for the browse context.
    //    //    /// </summary>
    //    //    /// <param name="SessionRQ">The session object to execute under.</param>
    //    //    /// <param name="constraints">The browse constraints identifying what sort of request should be accepted.</param>
    //    //    internal BrowseContext()
    //    //    {

    //    //    }
    //    //    #endregion // Constructor

    //    //    #region IQueryProvider Members

    //    //    public IQueryable<S> CreateQuery<S>(Expression expression)
    //    //    {
    //    //        if (typeof(S) != typeof(VertexResult))
    //    //            throw new Exception("Only " + typeof(VertexResult).FullName + " objects are supported.");

    //    //        this.mExpression = expression;

    //    //        return (IQueryable<S>)this;
    //    //    }

    //    //    public IQueryable CreateQuery(Expression expression)
    //    //    {
    //    //        return (IQueryable<VertexResult>)(this as IQueryProvider).CreateQuery<VertexResult>(expression);
    //    //    }

    //    //    public TResult Execute<TResult>(Expression expression)
    //    //    {
    //    //        MethodCallExpression methodcall = mExpression as MethodCallExpression;

    //    //        foreach (var param in methodcall.Arguments)
    //    //        {
    //    //            ProcessExpression(param);
    //    //        }

    //    //        return (TResult)results.GetEnumerator();
    //    //    }

    //    //    public object Execute(Expression expression)
    //    //    {
    //    //        return (this as IQueryProvider).Execute<IEnumerator<VertexResult>>(expression);
    //    //    }

    //    //    #endregion

    //    //    #region IEnumerable<T> Members

    //    //    public IEnumerator<T> GetEnumerator()
    //    //    {
    //    //        return (this as IQueryable).Provider.Execute<IEnumerator<VertexResult>>(mExpression);
    //    //    }

    //    //    #endregion

    //    //    #region IEnumerable Members

    //    //    IEnumerator IEnumerable.GetEnumerator()
    //    //    {
    //    //        return (IEnumerator<VertexResult>)(this as IQueryable).GetEnumerator();
    //    //    }

    //    //    #endregion

    //    //    #region IQueryable Members

    //    //    public Type ElementType
    //    //    {
    //    //        get { return typeof(VertexResult); }
    //    //    }

    //    //    public Expression Expression
    //    //    {
    //    //        get { return Expression.Constant(this); }
    //    //    }

    //    //    public IQueryProvider Provider
    //    //    {
    //    //        get { return this; }
    //    //    }

    //    //    #endregion

    //    //    private void ProcessExpression(Expression expression)
    //    //    {
    //    //        //RQRSContract<CDSRequestFolder, CDSResponseFolder> rqEnv = null;
    //    //        //Content rsData = null;
    //    //        //try
    //    //        //{
    //    //        //    rqEnv = EnvelopeRequest(
    //    //        //        new EnvelopeAddress(scContentDataStore.CDSCommandID, ));
    //    //        //    //Get the request
    //    //        //    CDSRequestFolder rq = rqEnv.ContractRequest;
    //    //        //    //Set the value type
    //    //        //    rq.DataType = itemType;
    //    //        //    rq.ByReference = rqH.ByReference;
    //    //        //    rq.DataReferenceType = rqH.RefType;
    //    //        //    rq.DataReferenceValue = rqH.RefValue;

    //    //        //    if (rqH.IDContent.HasValue)
    //    //        //        rq.DataContentID = rqH.IDContent.Value;
    //    //        //    if (rqH.IDVersion.HasValue)
    //    //        //        rq.DataVersionID = rqH.IDVersion.Value;

    //    //        //    rq.Data = null;

    //    //        //    SessionRQ.ProcessRequest(rqEnv, rqH.Priority);

    //    //        //    CDSResponseFolder rs = rqEnv.ContractResponse;
    //    //        //    if (rqEnv.ContractRequest.InternalCall &&
    //    //        //        rqH.Action == CDSStateAction.ResolveReference)
    //    //        //    {
    //    //        //        cid = rq.DataContentID;
    //    //        //        vid = rq.DataVersionID;
    //    //        //    }
    //    //        //    else
    //    //        //    {
    //    //        //        cid = rs.CurrentContentID;
    //    //        //        vid = rs.CurrentVersionID;
    //    //        //    }

    //    //        //    rsData = rs.Data;

    //    //        //    return rs.Status;
    //    //        //}
    //    //        //catch (Exception ex)
    //    //        //{
    //    //        //    throw ex;
    //    //        //}
    //    //        //finally
    //    //        //{
    //    //        //    if (rsData != null && rsData.ObjectPoolCanReturn)
    //    //        //        rsData.ObjectPoolReturn();
    //    //        //    rqH.ObjectPoolReturn();
    //    //        //    EnvelopeReturn(rqEnv);
    //    //        //}
    //    //    }


    //    //}

    //    //public virtual BrowseContext Query
    //    //{
    //    //    get
    //    //    {
    //    //        return new BrowseContext();
    //    //    }
    //    //}

    //    #endregion // Unused


    //}

}
