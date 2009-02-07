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
using System.Globalization;
using System.Security.Cryptography;

using Ximura;
using Ximura.Data;
using Ximura.Server;

#endregion // using
namespace Ximura
{
	#region CommandRSCallback
	/// <summary>
	/// The command call back delegate is used for asynchronous command calls.
	/// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The arguments including the envelope.</param>
	public delegate void CommandRSCallback(object sender, CommandRSEventArgs args);

    /// <summary>
    /// The command call back delegate is used for asynchronous command calls.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The arguments including the envelope.</param>
    /// <returns>Returns the true if the response can be deemed a success, otherwise if this returns false
    /// any further jobs using this validation ID will be ignored.</returns>
    public delegate DependencyValidateRSStatus DependencyValidateRSCallback(object sender, CommandRSEventArgs args);

	/// <summary>
	/// This delegate is used to provide feedback for long running jobs.
	/// </summary>
	public delegate void CommandProgressCallback(object sender, CommandProgressEventArgs args);

    /// <summary>
    /// This enumeration is used to signal the job status.
    /// </summary>
    public enum DependencyValidateRSStatus
    {
        /// <summary>
        /// The job has failed; all further jobs using this dependency key will be ignored, and any queued jobs
        /// will be cancelled.
        /// </summary>
        Failure,
        /// <summary>
        /// The job executed successfully
        /// </summary>
        Success,
        /// <summary>
        /// The entire completion job will be aborted.
        /// </summary>
        /// <remarks>This is not currently implemented and will be treated as a failure.</remarks>
        Abort
    }
	#endregion
	
	#region CommandRSEventArgs
	/// <summary>
	/// This class is used to send information when a command request completes.
	/// </summary>
	public class CommandRSEventArgs : EventArgs
	{
		IXimuraRQRSEnvelope mData;
		Guid? mJobID;

		/// <summary>
		/// This is the default constructor.
		/// </summary>
		/// <param name="job">The job.</param>
		public CommandRSEventArgs(JobBase job)
		{
			mJobID = job.ID;
			mData = job.Data;
		}

		/// <summary>
		/// This is the returning job data.
		/// </summary>
        public IXimuraRQRSEnvelope Data
		{
			get{return mData;}
		}
		/// <summary>
		/// This is the returning job id.
		/// </summary>
		public Guid? ID
		{
			get{return mJobID;}
		}
        /// <summary>
        /// This method is used for pooling resets.
        /// </summary>
        public void Reset()
        {
            mJobID = Guid.Empty;
            mData = null;
        }
	}
	#endregion

	#region CommandProgressEventArgs
	/// <summary>
	/// This class is used to report progress during a command request.
	/// </summary>
	public class CommandProgressEventArgs : EventArgs
	{
        #region Declarations
        private int mProgress;
        private string mDescription;
        #endregion // Declarations

		#region Constructors
		/// <summary>
		/// This is the empty constructor.
		/// </summary>
		public CommandProgressEventArgs()
		{
            mProgress = 0;
            mDescription = null;
		}
		/// <summary>
		/// This method can be used to return a straight progress indicator.
		/// </summary>
        /// <param name="progress">The command progress.</param>
        public CommandProgressEventArgs(int progress)
            : this(progress, null)
		{
		}
        /// <summary>
        /// This method can be used to return a straight progress indicator.
        /// </summary>
        /// <param name="progress">The command progress.</param>
        /// <param name="description">The command progress description.</param>
        public CommandProgressEventArgs(int progress, string description)
        {
            mProgress = progress;
            mDescription = description;
        }
		#endregion // Constructors

        #region Progress
        /// <summary>
        /// The command progress.
        /// </summary>
        public int Progress
        {
            get { return mProgress; }
            set { mProgress = value; }
        }
        #endregion // Progress
        #region Description
        /// <summary>
        /// The progress description.
        /// </summary>
        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; }
        }
        #endregion // Description
	}
	#endregion
}