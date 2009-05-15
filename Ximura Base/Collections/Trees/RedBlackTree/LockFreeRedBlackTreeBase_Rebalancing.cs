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
using System.Threading;
using System.Text;

using Ximura;
using Ximura.Helper;
#endregion // using
namespace Ximura.Collections
{
    public partial class LockFreeRedBlackTreeBase<TKey, TVal, TVert>
    {
        #region Struct --> TreeTraversalWindow<TKey, TVal>
        protected internal struct TreeTraversalWindow<TKey, TVal>
        {
            #region Declarations
            /// <summary>
            /// This private property specifies whether the vertexes should be locked when added to the window.
            /// </summary>
            private bool lockItems;
            #endregion // Declarations
            #region Constructor
            /// <summary>
            /// This is the default constructor for the structure.
            /// </summary>
            /// <param name="lockItems">The property specifies whether the structure should lock vertexes when added to the window.</param>
            public TreeTraversalWindow(bool lockItems)
            {
                this.lockItems = lockItems;
                Grandparent = null;
                Parent = null;
                Current = null;
            }
            #endregion // Constructor

            #region Vertexes
            /// <summary>
            /// The vertexes.
            /// </summary>
            public LockFreeRedBlackVertex<TKey, TVal> Grandparent, Parent, Current;
            #endregion // Public properties

            #region MoveDown(LockFreeRedBlackVertex<TKey, TVal> newCurrent)
            /// <summary>
            /// This method adds a new item and moves up the existing item to the window. The method releases the grandparent 
            /// if it is defined.
            /// </summary>
            /// <param name="newCurrent">The new vertex to add.</param>
            public void MoveDown(LockFreeRedBlackVertex<TKey, TVal> newCurrent)
            {
                if (lockItems)
                {
                    newCurrent.Lock();
                    Release(Grandparent);
                }

                Grandparent = Parent;
                Parent = Current;
                Current = newCurrent;
            }
            #endregion // MoveDown(LockFreeRedBlackVertex<TKey, TVal> newCurrent)
            #region MoveUp()
            /// <summary>
            /// This method moves the window up the tree. The new grandparent is found from the current grandparent's parent.
            /// The Current vertex is released and the other vertexes are moved down.
            /// </summary>
            public void MoveUp()
            {
                LockFreeRedBlackVertex<TKey, TVal> newGP = Grandparent.Parent;

                if (lockItems)
                {
                    if (newGP!=null)
                        newGP.Lock();
                    Release(Current);
                }

                Current = Parent;
                Parent = Grandparent;
                Grandparent = newGP;
            }
            #endregion // MoveUp()

            #region Release()
            /// <summary>
            /// This method releases the vertex that are currently held, starting with the Grandparent and moving down.
            /// </summary>
            public void Release()
            {
                Release(Grandparent);
                Release(Parent);
                Release(Current);
            }
            #endregion // Release()
            #region Release(LockFreeRedBlackVertex<TKey, TVal> vertex)
            /// <summary>
            /// This method releases a particular vertex.
            /// </summary>
            /// <param name="vertex">The vertex to release.</param>
            private void Release(LockFreeRedBlackVertex<TKey, TVal> vertex)
            {
                if (vertex != null)
                    vertex.Unlock();
            }
            #endregion // Release(LockFreeRedBlackVertex<TKey, TVal> vertex)

            #region ToString()
            /// <summary>
            /// This is a string representation of the window.
            /// </summary>
            /// <returns>Returns a description of the vertex window.</returns>
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                if (Grandparent != null)
                    sb.AppendFormat("G [{0}]\r\n", Grandparent);
                if (Parent != null)
                    sb.AppendFormat("P [{0}]\r\n", Parent);
                if (Current != null)
                    sb.AppendFormat("C [{0}]\r\n", Current);

                if (lockItems)
                {
                    sb.AppendLine();
                    sb.AppendLine("LOCKED");
                }

                return sb.ToString();
            }
            #endregion // ToString()
        }
        #endregion // Struct --> TreeTraversalWindow<TKey, TVal>

        protected void InsertFixup(TreeTraversalWindow<TKey, TVal> window)
        {
            LockFreeRedBlackVertex<TKey, TVal> vZ = window.Current;

            //while (vZ.IsRed)
            //{


            //}
        }

        protected bool RotateLeft(LockFreeRedBlackVertex<TKey, TVal> vX)
        {
            LockFreeRedBlackVertex<TKey, TVal> vY = vX.Right;

            vX.Right = vY.Left;
            vY.Left = vX;

            vY.Parent = vX.Parent;
            vX.Parent = vY;

            if (vY.Parent == null)
            {
                mRoot = vY;
                return true;
            }

            return false;
        }

        protected bool RotateRight(LockFreeRedBlackVertex<TKey, TVal> vX)
        {
            LockFreeRedBlackVertex<TKey, TVal> vY = vX.Parent;

            vY.Left = vX.Right;
            vX.Right = vY;

            vX.Parent = vY.Parent;
            vY.Parent = vX;


            if (vX.Parent == null)
            {
                mRoot = vX;
                return true;
            }

            return false;
        }


    }
}
