#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Ximura;
using Ximura.Data;

using CH = Ximura.Common;
using RH = Ximura.Reflection;
using AH = Ximura.AttributeHelper;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class BinaryDepthNode<T>
        //where T : class
    {
        #region Declarations
        private List<BinaryDepthNode<T>> mCollection = null;

        public int? Level { get; private set; }
        public byte? Value { get; private set; }
        public byte? Mask { get; private set; }
        public T Reference { get; private set; }

        public BinaryDepthNode<T> Parent { get; set; }
        public BinaryDepthNode<T> Left { get; set; }
        public BinaryDepthNode<T> Right { get; set; }
        public BinaryDepthNode<T> Child { get; set; }

        internal delegate void DelNodeAdd(BinaryDepthNode<T> node);
        internal DelNodeAdd NodeAddDelegate = null;
        #endregion // declarations
        #region Constructor
        public BinaryDepthNode()
        {
            Reset();
        }

        public BinaryDepthNode(BinaryDepthNode<T> Parent, byte Value, byte Mask, int? Level)
            : this()
        {
            this.Parent = Parent;
            this.NodeAddDelegate = Parent.NodeAddDelegate == null ? Parent.NodeAdd : Parent.NodeAddDelegate;

            this.Value = Value;
            this.Mask = Mask;
            this.Level = Level;
        }
        #endregion // Constructor

        public virtual void Reset()
        {
            if (mCollection != null)
            {
                List<BinaryDepthNode<T>> oldCollection = mCollection;
                mCollection = null;
                oldCollection.ForEach(n => n.Reset());
                oldCollection.Clear();
                oldCollection = null;
            }

            Value = null;
            Mask = null;
            Level = null;
            Parent = null;
            Left = null;
            Right = null;
            Child = null;
            Reference = default(T);
            NodeAddDelegate = null;
        }


        #region NodeAdd(BinaryDepthNode<T> node)
        /// <summary>
        /// This method adds the node to the collection.
        /// </summary>
        /// <param name="node">The node to add.</param>
        internal void NodeAdd(BinaryDepthNode<T> node)
        {
            if (NodeAddDelegate != null)
            {
                NodeAddDelegate(node);
                return;
            }
            if (mCollection == null)
                mCollection = new List<BinaryDepthNode<T>>();

            mCollection.Add(node);
        }
        #endregion // NodeAdd(BinaryDepthNode<T> node)

        public void Add(RegistryRecordParser data, T reference)
        {
            NodeAdd(data, reference);
        }

        public bool Contains(byte[] data)
        {
            return NodeResolve(data) == null;
        }

        public bool Resolve(byte[] data, out T reference)
        {
            BinaryDepthNode<T> node = Child.NodeResolve(data);

            if (node != null)
            {
                reference = node.Reference;
                return true;
            }

            reference = default(T);
            return false;
        }

        protected enum ChildScanDirection
        {
            Left = -1,
            NotSet = 0,
            Right = 1
        }

        protected BinaryDepthNode<T> SiblingScan(BinaryDepthNode<T> siblingScan, byte value)
        {
            ChildScanDirection direction = ChildScanDirection.NotSet;
            while (siblingScan != null && siblingScan.Value != value)
            {
                //if ((value & siblingScan.Mask.Value) == siblingScan.Value.Value)
                //    break;

                if ((value >= siblingScan.Value.Value) 
                    && (value <= (siblingScan.Value.Value | (siblingScan.Mask.Value ^ 255))))
                    break;


                //Go left?
                if (value < siblingScan.Value.Value)
                {
                    if (direction == ChildScanDirection.Right)
                    {
                        siblingScan = null;
                        break;
                    }

                    direction = ChildScanDirection.Left;
                    siblingScan = siblingScan.Left;
                    continue;
                }
                //Go right?
                if (value > siblingScan.Value.Value)
                {
                    if (direction == ChildScanDirection.Left)
                    {
                        siblingScan = null;
                        break;
                    }

                    direction = ChildScanDirection.Right;
                    siblingScan = siblingScan.Right;
                    continue;
                }

                break;
            }

            return siblingScan;
        }

        protected BinaryDepthNode<T> NodeResolve(byte[] data)
        {
            try
            {
                byte value = data[this.Level.Value];
                BinaryDepthNode<T> sibling = SiblingScan(this, value);

                if (sibling == null)
                    return null;

                if (sibling.Child == null)
                    return sibling;

                return sibling.Child.NodeResolve(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected bool NodeAdd(RegistryRecordParser address, T reference)
        {
            int nextLevel = this.Level.HasValue ? this.Level.Value + 1 : 0;

            if (!address.LevelOK(nextLevel))
            {
                Reference = reference;
                return true;
            }

            byte value, mask;
            address.GetLevel(nextLevel, out value, out mask);

            if (mask == 0)
            {
                Reference = reference;
                return false;
            }

            BinaryDepthNode<T> connect;
            if (Child == null)
            {
                Child = new BinaryDepthNode<T>(this, value, mask, nextLevel);
                NodeAdd(Child);
                connect = Child;
            }
            else
                connect = Child.ResolveOrInsertHorizontal(value, mask);

            bool success = connect.NodeAdd(address, reference);

            return success;
        }

        protected BinaryDepthNode<T> InsertLeft(BinaryDepthNode<T> newNode)
        {
            NodeAdd(newNode);

            BinaryDepthNode<T> left = this.Left;

            newNode.Left = left;
            newNode.Right = this;

            if (left != null)
                left.Right = newNode;

            this.Left = newNode;

            return newNode;
        }

        protected BinaryDepthNode<T> InsertRight(BinaryDepthNode<T> newNode)
        {
            NodeAdd(newNode);

            BinaryDepthNode<T> right = this.Right;

            newNode.Left = this;
            newNode.Right = right;

            if (right != null)
                right.Left = newNode;

            this.Right = newNode;

            return newNode;
        }

        protected BinaryDepthNode<T> ResolveOrInsertHorizontal(byte value, byte mask)
        {
            BinaryDepthNode<T> sibling = this;

            ChildScanDirection direction = ChildScanDirection.NotSet;
            while (sibling != null && sibling.Value != value)
            {
                //Go left?
                if (value < sibling.Value.Value)
                {
                    if (direction == ChildScanDirection.Right)
                    {
                        sibling = sibling.InsertLeft(new BinaryDepthNode<T>(this.Parent, value, mask, this.Level)); ;
                        break;
                    }

                    direction = ChildScanDirection.Left;
                    if (sibling.Left == null)
                    {
                        sibling = sibling.InsertLeft(new BinaryDepthNode<T>(this.Parent, value, mask, this.Level));
                        break;
                    }

                    sibling = sibling.Left;
                    continue;
                }
                //Go right?
                if (value > sibling.Value.Value)
                {
                    if (direction == ChildScanDirection.Left)
                    {
                        sibling = sibling.InsertRight(new BinaryDepthNode<T>(this.Parent, value, mask, this.Level)); ;
                        break;

                    }

                    direction = ChildScanDirection.Right;
                    if (sibling.Right == null)
                    {
                        sibling = sibling.InsertRight(new BinaryDepthNode<T>(this.Parent, value, mask, this.Level));
                        break;
                    }

                    sibling = sibling.Right;
                    continue;
                }

                break;
            }

            return sibling;
        }

        #region NodeDebugDump
        /// <summary>
        /// This debug method dumps the entire tree to a file.
        /// </summary>
        public void NodeDebugDump()
        {
            string id = Environment.TickCount.ToString();
            string location = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), id + ".txt");

            using (StreamWriter sw = File.CreateText(location))
            {
                mCollection.ForIndex((i, j) => sw.WriteLine(string.Format("\t{0}\t{1}", i, j.NodeDebugParentLeftRight)));

                sw.Close();
            }
        }

        #endregion // Debug
        #region NodeDebug
        public string NodeDebugParentLeftRight
        {
            get
            {
                return string.Format("{0}\t\t-->\t{1}<-{2}->{3}"
                    , this.NodeDebugParent
                    //, this.NodeDebugLeft
                    //, this.NodeDebugRight
                    , this.Left == null ? "*" : this.Left.NodeDebug
                    , this.NodeDebug
                    , this.Right == null ? "*" : this.Right.NodeDebug
                );
            }
        }

        public string NodeDebugParent
        {
            get
            {
                Stack<BinaryDepthNode<T>> pushStack = new Stack<BinaryDepthNode<T>>();

                BinaryDepthNode<T> parent = this.Parent;
                while (parent != null)
                {
                    pushStack.Push(parent);
                    parent = parent.Parent;
                }

                string IP = "";
                string Mask = "";

                while (pushStack.Count > 0)
                {
                    BinaryDepthNode<T> val = pushStack.Pop();
                    IP += ((val.Value.HasValue) ? val.Value.Value.ToString() + "." : "");
                    Mask += ((val.Mask.HasValue) ? val.Mask.Value.ToString() + "." : "");
                }

                IP += ((this.Value.HasValue) ? this.Value.Value.ToString() : "");
                Mask += ((this.Mask.HasValue) ? this.Mask.Value.ToString() : "");

                return string.Format("[{0}/{1}]{2}", IP, Mask, Reference);
            }
        }

        public string NodeDebugLeft
        {
            get
            {
                Stack<BinaryDepthNode<T>> pushStack = new Stack<BinaryDepthNode<T>>();

                BinaryDepthNode<T> left = this.Left;
                while (left != null)
                {
                    pushStack.Push(left);
                    left = left.Left;
                }

                StringBuilder sb = new StringBuilder();

                while (pushStack.Count > 0)
                {
                    BinaryDepthNode<T> val = pushStack.Pop();
                    sb.Append(val.NodeDebugValue);
                    sb.Append("<");
                }

                sb.Append(NodeDebug);
                return sb.ToString();
            }
        }

        public string NodeDebugRight
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(NodeDebug);

                BinaryDepthNode<T> right = this.Right;

                while (right != null)
                {
                    sb.Append(">");
                    sb.Append(right.NodeDebugValue);
                    right = right.Right;
                }

                return sb.ToString();
            }
        }

        public string NodeDebug
        {
            get
            {
                return string.Format("({0}|{1}/{2:X}{3})"
                    , this.Level.HasValue ? this.Level.Value.ToString() : "*"
                    , this.Value.HasValue ? this.Value.Value.ToString() : "*"
                    , this.Mask.HasValue ? this.Mask.Value : 0
                    , this.Child == null ? "" : "!");
            }
        }

        public string NodeDebugValue
        {
            get
            {
                return string.Format("{0}/{1:X}{2}"
                    , this.Value.HasValue ? this.Value.Value.ToString() : "*"
                    , this.Mask.HasValue ? this.Mask.Value : 0
                    , this.Child == null ? "" : "!");
            }
        }
        #endregion // NodeDebug

        public override string ToString()
        {
            return NodeDebugParentLeftRight; 
        }
    }

}
