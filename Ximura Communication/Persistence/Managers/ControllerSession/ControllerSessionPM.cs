#region using
using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

using Ximura;
using Ximura.Data;
using Ximura.Data;

using Ximura.Framework;
using Ximura.Framework;
using CH = Ximura.Common;
using RH = Ximura.Reflection;
#endregion
namespace Ximura.Communication
{
    /// <summary>
    /// This CDS persistence manager contains the temporary session objects.
    /// </summary>
    [CDSStateActionPermit(CDSStateAction.Read)]
    [CDSStateActionPermit(CDSStateAction.Delete)]
    [CDSStateActionPermit(CDSStateAction.VersionCheck)]
    public class ControllerSessionPM :
        PersistenceManagerCDSState<ControllerSession, ControllerSession, CommandConfiguration>
    {
        #region Declarations
        private Dictionary<Guid, SessionHolder> mSessions;
        private object syncSession = new object();

        private struct SessionHolder
        {
            #region Declarations
            private ControllerSession mSession;
            private DateTime mLastAccessed;
            #endregion // Declarations

            #region Constructor
            /// <summary>
            /// This is the default constructor.
            /// </summary>
            /// <param name="session">The session.</param>
            public SessionHolder(ControllerSession session)
            {
                mSession = session;
                mLastAccessed = DateTime.Now;
            }
            #endregion // Constructor

            #region LastAccessed
            /// <summary>
            /// The last accessed time.
            /// </summary>
            public DateTime LastAccessed
            {
                get { return mLastAccessed; }
            }
            #endregion // LastAccessed

            #region Session
            /// <summary>
            /// The session
            /// </summary>
            public ControllerSession Session
            {
                get 
                {
                    mLastAccessed = DateTime.Now;
                    return mSession; 
                }
                set { mSession = value; }
            }
            #endregion // Session

            #region Expiry
            /// <summary>
            /// This is the time since the session was last accessed.
            /// </summary>
            public TimeSpan Expiry
            {
                get
                {
                    return DateTime.Now - mLastAccessed;
                }
            }
            #endregion // Expiry

        }

        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public ControllerSessionPM() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public ControllerSessionPM(IContainer container)
            : base(container)
        {
            mSessions = new Dictionary<Guid, SessionHolder>();
        }
        #endregion // Constructors

        #region Read(CDSContext context)
        /// <summary>
        /// This method returns the standard document.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool Read(CDSContext context)
        {
            try
            {
                ControllerSession csr = null;

                Guid? sessionID = context.Request.DataContentID;

                lock (syncSession)
                {
                    if (sessionID.HasValue && mSessions.ContainsKey(sessionID.Value))
                    {
                        SessionHolder sh = mSessions[sessionID.Value];
                        if (sh.Expiry.Minutes <= 20)
                            csr = sh.Session;
                        else
                        {
                            mSessions[sessionID.Value].Session.ObjectPoolReturn();
                            mSessions.Remove(sessionID.Value);
                        }
                    }
                    
                    if (csr == null)
                    {
                        csr = context.GetObjectPool<ControllerSession>().Get();
                        csr.Load();
                        csr.IDContent = Guid.NewGuid();
                        csr.IDVersion = Guid.NewGuid();

                        mSessions.Add(csr.IDContent, new SessionHolder(csr));
                    }
                }

                //using (Stream strmData = new MemoryStream(RH.ResourceLoadFromUri(ResolverScriptLocation)))
                //{
                //    csr.Load(strmData);
                //}

                context.Response.Data = csr;
                context.Response.Status = CH.HTTPCodes.OK_200;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion // Read(CDSContext context)

        #region Delete(CDSContext context)
        /// <summary>
        /// This method deletes an active session from the collection.
        /// </summary>
        /// <param name="context">The current CDS context.</param>
        /// <returns>Returns true.</returns>
        protected override bool Delete(CDSContext context)
        {
            Guid? sessionID = context.Request.DataContentID;
            
            lock (syncSession)
            {
                if (sessionID.HasValue && mSessions.ContainsKey(sessionID.Value))
                {
                    SessionHolder sh = mSessions[sessionID.Value];
                    mSessions.Remove(sessionID.Value);
                    ControllerSession cs = sh.Session;
                    sh.Session = null;

                    if (cs != null && cs.ObjectPoolCanReturn)
                        cs.ObjectPoolReturn();

                    context.Response.Status = CH.HTTPCodes.OK_200;
                }
                else
                    context.Response.Status = CH.HTTPCodes.NotFound_404;
            }

            context.Response.Data = null;
            return true;
        }
        #endregion // Delete(CDSContext context)
    }
}