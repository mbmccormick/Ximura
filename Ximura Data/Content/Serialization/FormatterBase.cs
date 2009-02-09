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
using System.IO;
using System.Runtime.Serialization;

using Ximura;
using Ximura.Helper;
using CH=Ximura.Helper.Common;
#endregion
namespace Ximura.Data
{
	/// <summary>
	/// FormatterBase is the base formatter class for Content based entities.
	/// </summary>
    public class FormatterBase : IXimuraFormatter
	{
		#region Declarations
		/// <summary>
		/// The Serialization binder.
		/// </summary>
		protected SerializationBinder myBinder = null;
		/// <summary>
		/// The default context which is set to persistence.
		/// </summary>
		protected StreamingContext myContext = new StreamingContext(StreamingContextStates.Persistence);
		/// <summary>
		/// The surrogate selector.
		/// </summary>
		protected ISurrogateSelector mySurrogateSelector=null;
		#endregion // Declarations
		#region Constructor
		/// <summary>
		/// The default constructor
		/// </summary>
		public FormatterBase(){}
		#endregion // Construcutor

		#region IFormatter Members
		#region Serialize
		/// <summary>
		/// This method serializes the object to the stream.
		/// </summary>
		/// <param name="serializationStream">The stream to serialize the object to.</param>
		/// <param name="graph">The object.</param>
		public virtual void Serialize(Stream serializationStream, object graph)
		{
            throw new NotImplementedException("FormatterBase/Serialize is not implemented.");
        }
		#endregion // Serialize
		#region Deserialize
		/// <summary>
		/// This method deserializes the stream and returns the object.
		/// </summary>
		/// <param name="serializationStream">The stream to deserialize.</param>
		/// <returns>A object.</returns>
		public virtual object Deserialize(Stream serializationStream)
		{
            return (Content)Deserialize(serializationStream, null);
		}

        /// <summary>
        /// This method deserializes the stream and returns the object from the pool if supplied.
        /// </summary>
        /// <param name="serializationStream">The stream to deserialize.</param>
        /// <param name="pMan">The pool manager. This can be null, in which case, a new object will be created.</param>
        /// <returns>The content object</returns>
        public virtual object Deserialize(Stream serializationStream, IXimuraPoolManager pMan)
        {
            throw new NotImplementedException("FormatterBase/Deserialize is not implemented.");
        }
		#endregion // Deserialize

		#region Properties
		/// <summary>
		/// The binder object which is initially set to null.
		/// </summary>
		public virtual SerializationBinder Binder
		{
			get
			{
				return myBinder;
			}
			set
			{
				myBinder = value;
			}
		}

		/// <summary>
		/// The streaming context. The initial value is set to persistence.
		/// </summary>
		public virtual StreamingContext Context
		{
			get
			{
				return myContext;
			}
			set
			{
				myContext=value;
			}
		}

		/// <summary>
		/// The surrogate selector. This is initially set to null.
		/// </summary>
		public virtual ISurrogateSelector SurrogateSelector
		{
			get
			{
				return mySurrogateSelector;
			}
			set
			{
				mySurrogateSelector=value;
			}
		}
		#endregion // Properties
		#endregion

    }
}