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
using System.Diagnostics;
#endregion // using
namespace Ximura.Framework
{
	/// <summary>
	/// This is the empty logging provider. This class does nothing but is provided
	/// to allow logging code to be inserted in to a command, which can be executed when
	/// no provider is available.
	/// </summary>
	public sealed class NullLoggerAgent : LoggerAgentBase
	{
		#region Static Single Instance implementation
		private static NullLoggerAgent no_log = null;

		/// <summary>
		/// We specify a private constructor as we only require a single instance
		/// of this class across a domain.
		/// </summary>
		private NullLoggerAgent(){}
		/// <summary>
		/// This static method is used to retrieve the single instance of the null
		/// logging provider.
		/// </summary>
		/// <returns></returns>
		public static IXimuraLogging NoLog()
		{
			if (no_log == null)
				no_log = new NullLoggerAgent();

			return no_log as IXimuraLogging;
		}
		#endregion
			
		#region AcceptCategory
		/// <summary>
		/// This method informs the logging manager whether the logger will log for 
		/// this particular category. The null logger will always return false.
		/// </summary>
		/// <param name="category">The category to log.</param>
		/// <returns>The method always returns false.</returns>
		public override bool AcceptCategory(string category, EventLogEntryType type)
		{
			return false;
		}
		#endregion

		#region Initialize
		/// <summary>
		/// This method is used for initialization. For the null logger this
		/// does not do anything.
		/// </summary>
		/// <param name="settings"></param>
		public override void Initialize(IXimuraLoggerSettings settings)
		{
			// Do nothing
		}

		#endregion

		/// <summary>
		/// This method does nothing in the null logger.
		/// </summary>
		/// <param name="message">The message</param>
		/// <param name="category">The category</param>
		public override void WriteLine(string message)
		{

		}

		/// <summary>
		/// This method does nothing in the null logger.
		/// </summary>
		/// <param name="message">The message</param>
		/// <param name="category">The category</param>
		public override void Write(string message)
		{

		}

		/// <summary>
		/// This method does nothing in the null logger.
		/// </summary>
		/// <param name="message">The message</param>
		/// <param name="category">The category</param>
		public override void Write(String message, String category)
		{

		}
	}
}
