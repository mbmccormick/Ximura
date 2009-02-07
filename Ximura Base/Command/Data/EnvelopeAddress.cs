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

using Ximura;
using Ximura.Data;
#endregion // using
namespace Ximura.Command
{
	/// <summary>
	/// EnvelopeAddress is used to route a request to the relevant destination within
	/// the Ximura Application framework.
	/// </summary>
	[Serializable]
	public struct EnvelopeAddress: IEquatable<EnvelopeAddress>
	{
        #region NullDestination
        /// <summary>
        /// This is the null destination property.
        /// </summary>
        public static readonly EnvelopeAddress NullDestination;
        #endregion // NullDestination

		#region Properties
		/// <summary>
		/// The destination command ID
		/// </summary>
		public Guid command;
		/// <summary>
		/// The subcommand.
		/// </summary>
		public object SubCommand;
		#endregion

		#region Static Constructor
        /// <summary>
        /// This is the default static constructor that creates the Null destination address.
        /// </summary>
        static EnvelopeAddress()
        {
            EnvelopeAddress.NullDestination = new EnvelopeAddress(Guid.Empty);
        }
        #endregion

        #region Constructor
        //public EnvelopeAddress(string command) : this(new Guid(command), null) { }
		//public EnvelopeAddress(string command, object subcommand):this(new Guid(command),subcommand){}
        /// <summary>
        /// This is the default constructor for the address command.
        /// </summary>
        /// <param name="command">The destination command ID.</param>
		public EnvelopeAddress(Guid command):this(command,null){}
        /// <summary>
        /// This is the default constructor for the Envelope address where a sub command is specified..
        /// </summary>
        /// <param name="command">The destination id of the command this message should be sent to.</param>
        /// <param name="subcommand">The sub address for the command. 
        /// Set this to Null if you do not require a subcommand.</param>
		public EnvelopeAddress(Guid command, object subcommand)
		{
			this.command=command;
			this.SubCommand=subcommand;
		}
		#endregion

        #region Equals
        /// <summary>
        /// This is the equals override.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>Returns true if the object is the same as this EnvelopeAddress.</returns>
        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is EnvelopeAddress))
            {
                return false;
            }

            return Equals((EnvelopeAddress)obj);
        } 
        #endregion

        #region GetHashCode()
        /// <summary>
        /// This is the hash code of the object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion // GetHashCode()


        #region IEquatable<EnvelopeAddress> Members
        /// <summary>
        /// This the equateble interface implementation.
        /// </summary>
        /// <param name="other">The address to compare.</param>
        /// <returns>Returns true if the address is equal.</returns>
        public bool Equals(EnvelopeAddress other)
        {
            return this==other;
        }

        /// <summary>
        /// This static operator is the equals operator.
        /// </summary>
        /// <param name="a">Op A</param>
        /// <param name="b">Op B</param>
        /// <returns>Returns true if the two addresses are the same.</returns>
        public static bool operator ==(EnvelopeAddress a, EnvelopeAddress b)
        {
            //if (a == null || b == null)
            //    return false;

            return a.command == b.command && a.SubCommand == b.SubCommand;

        }
        /// <summary>
        /// This is the not equals operator.
        /// </summary>
        /// <param name="a">Op A</param>
        /// <param name="b">Op B</param>
        /// <returns>Returns true if the two addresses are not the same.</returns>
        public static bool operator !=(EnvelopeAddress a, EnvelopeAddress b)
        {
            return !(a == b);
        }

        #endregion
    }
}
