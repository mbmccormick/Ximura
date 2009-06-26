#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// For more details see http://ximura.org
//
// Contributors:
//     Paul Stancer - initial implementation
// *******************************************************************************
#endregion
#region using
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Text;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    #region VertexClassBase<T>
    /// <summary>
    /// This is the abstract class for class based data networks.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
#if (DEBUG)
    [DebuggerDisplay("Data = {DebugString}")]
#endif
    public abstract class VertexClassBase<T> : CollectionVertexClassBase<T>, ICollectionVertex<T>, IDisposable
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public VertexClassBase()
        {
            Next = null;
        }
        #endregion

        #region Next
        /// <summary>
        /// This is the next node in the list.
        /// </summary>
        public VertexClassBase<T> Next { get; set; }
        #endregion // Next

        #region IsTerminator
        /// <summary>
        /// This property specifies whether the vertex is a list terminator.
        /// </summary>
        public override bool IsTerminator { get { return Next == null; } }
        #endregion // IsTerminator

        #region HashID
        /// <summary>
        /// This is the hashID for the item.
        /// </summary>
        public int HashID { get; set; }
        #endregion // HashID

        #region Up
        /// <summary>
        /// The Up sentinel
        /// </summary>
        public virtual VertexClassBase<T> Up { get { return null; } set { } }
        #endregion // Up
        #region Down
        /// <summary>
        /// The down sentinel.
        /// </summary>
        public virtual VertexClassBase<T> Down { get { return null; } set { } }
        #endregion // Down

        #region IDisposable Members
        /// <summary>
        /// This method removes and object references.
        /// </summary>
        public virtual void Dispose()
        {
            Next = null;
            if (IsLocked) Unlock();
            GC.SuppressFinalize(this);
        }
        #endregion

        #region ICollectionVertex<T> Members
        /// <summary>
        /// This is the vertex data.
        /// </summary>
        public virtual T Data
        {
            get { return Value; }
        }

        #endregion

        #region ToString()
        /// <summary>
        /// This override provides quick and easy debugging support.
        /// </summary>
        /// <returns>Returns a string representation of the vertex.</returns>
        public override string ToString()
        {
            if (IsSentinel)
                return string.Format("[H{0:X} SNTL]{1}{2}{3}{4}"
                    , HashID
                    , IsLocked ? "L" : ""
                    , Down == null ? "" : string.Format("D{0:X} ", Down.HashID)
                    , Up == null ? "" : string.Format("U{0:X} ", Up.HashID)
                    , IsTerminator ? " END" : "");
            else
                return string.Format("[H{0:X} = {1}]{2}{3}{4}{5}"
                    , HashID
                    , Value.ToString()
                    , IsLocked ? "L" : ""
                    , Down == null ? "" : string.Format("D{0:X} ", Down.HashID)
                    , Up == null ? "" : string.Format("U{0:X} ", Up.HashID)
                    , IsTerminator ? " END" : "");
        }
        #endregion // ToString()

#if(DEBUG)
        /// <summary>
        /// This debug method returns the next five items in the chain.
        /// </summary>
        public string DebugString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                VertexClassBase<T> item = this;

                for (int loop = 0; loop < 5; loop++)
                {
                    sb.Append(item.ToString());
                    if (item.IsTerminator)
                        break;
                    else
                        sb.Append(" -> ");
                    item = item.Next;
                }

                if (!item.IsTerminator)
                    sb.Append(" ...");

                return sb.ToString();
            }
        }
#endif
    }
    #endregion // VertexClassBase<T>

    #region VertexClassData<T>
    /// <summary>
    /// This vertex class holds the collection data.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    public class VertexClassData<T> : VertexClassBase<T>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public VertexClassData():base()
        {
            Value = default(T);
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public VertexClassData(T item, int hashID):base()
        {
            Value = item;
            HashID = hashID;
        }
        #endregion

        #region Value
        /// <summary>
        /// This is the data for the vertex.
        /// </summary>
        public override T Value
        {
            get;
            set;
        }
        #endregion // Value

        #region IsSentinel
        /// <summary>
        /// This property specifies that the data item is a sentinel.
        /// </summary>
        public override bool IsSentinel { get { return false; } }
        #endregion // IsSentinel

        #region IDisposable Members
        /// <summary>
        /// This method removes and object references.
        /// </summary>
        public override void Dispose()
        {
            if (typeof(T).IsClass)
                Value = default(T);

            base.Dispose();
        }
        #endregion
    }
    #endregion // VertexClassData<T>

    #region VertexClassDataSentinel<T>
    /// <summary>
    /// This class is a base sentinel for the linked list.
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    public class VertexClassDataSentinel<T> : VertexClassBase<T>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public VertexClassDataSentinel()
            : base()
        {
        }
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public VertexClassDataSentinel(int hashID)
            : base()
        {
            HashID = hashID;
        }
        #endregion

        #region Value
        /// <summary>
        /// This override sets the value to the default of the type.
        /// </summary>
        public override T Value
        {
            get { return default(T); }
            set { }
        }
        #endregion // Value
        #region IsSentinel
        /// <summary>
        /// This property specifies that the data item is a sentinel.
        /// </summary>
        public override bool IsSentinel { get { return true; } }
        #endregion // IsSentinel

    }
    #endregion // VertexClassDataSentinel<T>

    #region VertexClassSentinel<T>
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The data type.</typeparam>
    public class VertexClassSentinel<T> : VertexClassBase<T>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public VertexClassSentinel():base()
        {
            Up = null;
            Down = null;
        }

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public VertexClassSentinel(int hashID, VertexClassBase<T> down)
            : base()
        {
            Up = null;
            Down = down;
            HashID = hashID;
        }
        #endregion

        #region Value
        /// <summary>
        /// This override sets the value to the default of the type.
        /// </summary>
        public override T Value
        {
            get { return default(T); }
            set { }
        }
        #endregion // Value
        #region IsSentinel
        /// <summary>
        /// This property specifies that the data item is a sentinel.
        /// </summary>
        public override bool IsSentinel { get { return true; } }
        #endregion // IsSentinel

        #region Up
        /// <summary>
        /// The Up sentinel.
        /// </summary>
        public override VertexClassBase<T> Up { get; set; }
        #endregion // Up
        #region Down
        /// <summary>
        /// The down item.
        /// </summary>
        public override VertexClassBase<T> Down { get; set; }
        #endregion // Down

        #region IDisposable Members
        /// <summary>
        /// This method removes and object references.
        /// </summary>
        public override void Dispose()
        {
            Up = null;
            Down = null;

            base.Dispose();
        }
        #endregion
    }
    #endregion // VertexClassSentinel<T>



}
