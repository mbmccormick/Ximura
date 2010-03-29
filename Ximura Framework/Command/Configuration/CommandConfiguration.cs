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
﻿#region using
using System;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;

using Ximura;
using Ximura.Data;


using CH = Ximura.Common;
using RH = Ximura.Reflection;

using Ximura.Framework;



#endregion // using
namespace Ximura.Framework
{
    public class CommandConfiguration<T> : CommandConfiguration
        where T : TimerPollJob, new()
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public CommandConfiguration() { }

        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public CommandConfiguration(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        #region TimerPollJobCreate(List<TimerPollJob> jobs, XmlNode node)
        /// <summary>
        /// This method creates a specific poll job and adds it to the poll job collection.
        /// </summary>
        /// <param name="jobs">The poll job collection.</param>
        /// <param name="node">The configuration node for the poll job.</param>
        protected override void TimerPollJobCreate(List<TimerPollJob> jobs, XmlElement node)
        {
            T newJob = new T();
            newJob.Configure(node, NSM, null);
            jobs.Add(newJob);
        }
        #endregion // TimerPollJobCreate(List<TimerPollJob> jobs, XmlNode node)
    }

    /// <summary>
    /// This is the base class that holds the command configuration.
    /// </summary>
    [XimuraContentTypeID("DB4784B5-7B8C-4bf5-96BD-174A1ECC90B9")]
    [XimuraDataContentDefault(
        "xmrres://XimuraFramework/Ximura.Framework.CommandConfiguration/Ximura.Framework.Command.Configuration.CommandConfiguration_Default.xml")]
    [XimuraDataContentSchemaReference("http://schema.ximura.org/configuration/command/1.0",
        "xmrres://XimuraFramework/Ximura.Framework.CommandConfiguration/Ximura.Framework.Command.Configuration.CommandConfiguration.xsd")]
    public class CommandConfiguration : ConfigurationBase, IXimuraCommandConfiguration
    {
        #region Declarations
        /// <summary>
        /// This list contains the TimerPoll jobs for the configuration.
        /// </summary>
        protected List<TimerPollJob> mPollJobs = null;
        /// <summary>
        /// This object is used to syncronize access to the mPollJobs collection.
        /// </summary>
        protected object syncTimerPollJobs = new object();
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public CommandConfiguration() { }
        /// <summary>
        /// This is the deserialization constructor. 
        /// </summary>
        /// <param name="info">The Serialization info object that contains all the relevant data.</param>
        /// <param name="context">The serialization context.</param>
        public CommandConfiguration(SerializationInfo info, StreamingContext context)
            :
            base(info, context) { }
        #endregion

        #region Reset()
        /// <summary>
        /// This override resets cached and calculated values.
        /// </summary>
        public override void Reset()
        {
            ProcessSessionUserRealm = null;
            TimerFirstCall = true;
            TimerNextCall = null;
            base.Reset();
        }
        #endregion // Reset()

        #region XPScAdd(Dictionary<string, string> mappingShortcuts)
        /// <summary>
        /// This method adds the XPath shortcuts in to the collection. You should
        /// override this method to add your own shorcuts.
        /// </summary>
        /// <param name="mappingShortcuts">The mapping shorcut collection.</param>
        protected override void XPScAdd(Dictionary<string, string> mappingShortcuts)
        {
            string basePath = "//r:Configuration";
            mappingShortcuts.Add("r", basePath);
        }
        #endregion // XPScAdd(Dictionary<string, string> mappingShortcuts)
        #region NamespaceManagerAdd(XmlNamespaceManager nsm)
        /// <summary>
        /// This override adds the ximura namespace to the default Namespace manager.
        /// </summary>
        /// <param name="nsm">The system default namespace manager.</param>
        protected override void NamespaceManagerAdd(XmlNamespaceManager nsm)
        {
            base.NamespaceManagerAdd(nsm);
            nsm.AddNamespace("comconf", "http://schema.ximura.org/configuration/command/1.0");
        }
        #endregion // NamespaceManagerAdd(XmlNamespaceManager nsm)
        #region NamespaceDefaultShortName
        /// <summary>
        /// This is the short name used in the namespace manager to refer to the root namespace.
        /// </summary>
        protected override string NamespaceDefaultShortName
        {
            get
            {
                return "r";
            }
        }
        #endregion // NamespaceDefaultShortName

        #region CommandID
        ///// <summary>
        ///// The command identifier.
        ///// </summary>
        //public Guid CommandID
        //{
        //    get 
        //    {
        //        Guid? val = this.XmlMappingGetToGuidNullable(XPSc("r", "comconf:commandconfig", "comconf:id"));
        //        return val.HasValue?val.Value:Guid.Empty; 
        //    }
        //    protected set { XmlMappingSet(XPSc("r", "comconf:commandconfig", "comconf:id"), value); }
        //}
        #endregion // CommandID
        #region CommandName
        ///// <summary>
        ///// The command friendly name.
        ///// </summary>
        //public string CommandName
        //{
        //    get { return XmlMappingGetToString(XPSc("r", "comconf:commandconfig", "comconf:name")); }
        //    protected set { XmlMappingSet(XPSc("r", "comconf:commandconfig", "comconf:name"), value); }
        //}
        #endregion // CommandName
        #region CommandDescription
        ///// <summary>
        ///// The command friendly description.
        ///// </summary>
        //public string CommandDescription
        //{
        //    get { return XmlMappingGetToString(XPSc("r", "comconf:commandconfig", "comconf:description")); }
        //    protected set { XmlMappingSet(XPSc("r", "comconf:commandconfig", "comconf:description"), value); }
        //}
        #endregion // CommandDescription

        #region CommandEnabled
        /// <summary>
        /// Returns true if the command is enabled.
        /// </summary>
        public bool CommandEnabled
        {
            get { return XmlMappingGetToBool(XPSc("r", "comconf:commandconfig", "comconf:enabled")); }
            protected set { XmlMappingSet(XPSc("r", "comconf:commandconfig", "comconf:enabled"), value ? "true" : "false"); }
        }
        #endregion // CommandEnabled

        #region CommandPriority
        /// <summary>
        /// The command thread priority.
        /// </summary>
        public JobPriority CommandPriority
        {
            get 
            { 
                JobPriority priority= JobPriority.Normal;
                try
                {
                    priority = (JobPriority)Enum.Parse(typeof(JobPriority), 
                        XmlMappingGetToString(XPSc("r", "comconf:commandconfig", "comconf:priority")));
                }
                catch
                {
                }

                return priority;
            }
            protected set 
            { 
                XmlMappingSet(XPSc("r", "comconf:commandconfig", "comconf:priority"), value.ToString()); 
            }

        }
        #endregion // CommandPriority

        #region ProcessSessionRequired
        /// <summary>
        /// This property specifies whether a process session is required.
        /// </summary>
        public bool ProcessSessionRequired
        {
            get { return XmlMappingGetToBool(XPScA("r", "required", "comconf:commandconfig", "comconf:processsession")); }
            protected set { XmlMappingSet(XPScA("r", "required", "comconf:commandconfig", "comconf:processsession"), value ? "true" : "false"); }
        }
        #endregion // ProcessSessionRequired
        #region ProcessSessionUseSystemCredentials
        /// <summary>
        /// This property specifies whether a process session is required.
        /// </summary>
        public bool ProcessSessionUseSystemCredentials
        {
            get { return XmlMappingGetToBool(XPScA("r", "usesystemcredentials", "comconf:commandconfig", "comconf:processsession")); }
            protected set { XmlMappingSet(XPScA("r", "usesystemcredentials", "comconf:commandconfig", "comconf:processsession"), value ? "true" : "false"); }
        }
        #endregion // ProcessSessionUseSystemCredentials
        #region ProcessSessionUserName
        /// <summary>
        /// This property returns the process session username.
        /// </summary>
        public string ProcessSessionUserName
        {
            get { return XmlMappingGetToString(XPScA("r", "account", "comconf:commandconfig", "comconf:processsession")); }
            protected set { XmlMappingSet(XPScA("r", "account", "comconf:commandconfig", "comconf:processsession"), value); }
        }
        #endregion // ProcessSessionUserName
        #region ProcessSessionRealmDomain
        /// <summary>
        /// This method returns the process session realm and domain.
        /// </summary>
        public string ProcessSessionRealmDomain
        {
            get { return XmlMappingGetToString(XPScA("r", "realm", "comconf:commandconfig", "comconf:processsession")); }
            protected set { XmlMappingSet(XPScA("r", "realm", "comconf:commandconfig", "comconf:processsession"), value); }
        }
        #endregion // ProcessSessionRealmDomain
        #region ProcessSessionAuthType
        /// <summary>
        /// This property returns the process session authentication type.
        /// </summary>
        public string ProcessSessionAuthType
        {
            get { return XmlMappingGetToString(XPScA("r", "authtype", "comconf:commandconfig", "comconf:processsession")); }
            protected set { XmlMappingSet(XPScA("r", "authtype", "comconf:commandconfig", "comconf:processsession"), value); }
        }
        #endregion // ProcessSessionAuthType
        #region ProcessSessionSecurityData
        /// <summary>
        /// this property is the session security data.
        /// </summary>
        protected string ProcessSessionSecurityData
        {
            get { return XmlMappingGetToString(XPSc("r", "comconf:commandconfig", "comconf:processsession")); }
            set { XmlMappingSet(XPSc("r", "comconf:commandconfig", "comconf:processsession"), value); }
        }
        #endregion // ProcessSessionSecurityData
        #region ProcessSessionHash(byte[] seed)
        /// <summary>
        /// This method returns the process session hash.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public byte[] ProcessSessionHash(byte[] seed)
        {
            // convert the password to byte[]
            byte[] bPwd = CH.ConvertStringToByte(ProcessSessionSecurityData);
            // concatenate the password and seed
            byte[] bPwdAndSeed = CH.ConcatenateByteArray(bPwd, seed);
            // return the hashed password
            return CH.ComputeHash(bPwdAndSeed);
        }
        #endregion // ProcessSessionHash(byte[] seed)
        #region ProcessSessionUserRealm
        /// <summary>
        /// This method returns the user session realm.
        /// </summary>
        public string ProcessSessionUserRealm
        {
            get;
            protected set;
        }
        #endregion // ProcessSessionUserRealm

        #region TimerEnabled
        /// <summary>
        /// Returns true if the timer function is enabled.
        /// </summary>
        public virtual bool TimerEnabled
        {
            get { return XmlMappingGetToBool(XPScA("r", "enabled", "comconf:timerpoll")); }
            protected set { XmlMappingSet(XPScA("r", "enabled", "comconf:timerpoll"), value); }
        }
        #endregion // TimerAutoStart
        #region TimerAutoStart
        /// <summary>
        /// Returns true if the timer should call the OnTimerEvent on initialization.
        /// </summary>
        public virtual bool TimerAutoStart
        {
            get { return XmlMappingGetToBool(XPScA("r", "autostart", "comconf:timerpoll")); }
            protected set { XmlMappingSet(XPScA("r", "autostart", "comconf:timerpoll"), value); }
        }
        #endregion // TimerAutoStart

        #region TimerPollEnabled
        /// <summary>
        /// Returns true if the timer is enabled.
        /// </summary>
        public virtual bool TimerPollEnabled
        {
            get { return XmlMappingGetToBool(XPScA("r", "polltimeenabled", "comconf:timerpoll")); }
            protected set { XmlMappingSet(XPSc("r", "polltimeenabled", "comconf:timerpoll"), value); }
        }
        #endregion // TimerEnabled
        #region TimerPollValue
        /// <summary>
        /// This is the retry interval for the timer.
        /// </summary>
        public virtual int TimerPollValue
        {
            get { return XmlMappingGetToInt32(XPScA("r", "polltimerepeatvalue", "comconf:timerpoll")); }
            protected set { XmlMappingSet(XPScA("r", "polltimerepeatvalue", "comconf:timerpoll"), value); }
        }
        #endregion // TimerPollValue
        #region TimerPollValueType
        /// <summary>
        /// This is the timer type for the timer value, i.e. hours(h), minutes(m), days(d) etc.
        /// </summary>
        public virtual string TimerPollValueType
        {
            get { return XmlMappingGetToString(XPScA("r", "polltimerepeattype", "comconf:timerpoll")); }
            protected set { XmlMappingSet(XPScA("r", "polltimerepeattype", "comconf:timerpoll"), value); }
        }
        #endregion // TimerPollValue
        #region TimerPollInterval
        /// <summary>
        /// This is the time span interval between timer polls.
        /// </summary>
        public virtual TimeSpan? TimerPollInterval
        {
            get
            {
                return TimerPollJob.TimeSpanCalculate(TimerPollValue,TimerPollValueType);
            }
        }
        #endregion // TimerPollInterval

        #region TimerPollJobs/TimerPollJobsCreate()
        /// <summary>
        /// This is the collection of timer poll jobs. You should override this method and provide a collection
        /// of jobs if you wish to use the self-poll functionality.
        /// </summary>
        public virtual IEnumerable<TimerPollJob> TimerPollJobs
        {
            get
            {
                if (mPollJobs == null)
                {
                    TimerPollJobsCreate(ref mPollJobs);
                }

                return mPollJobs;
            }
        }
        #endregion // PollJobs
        #region TimerPollJobsCreate(List<TimerPollJob> jobs)
        /// <summary>
        /// This method creates the Poll job collection. This method will only be called once and will be wrapped around the 
        /// lock(syncTimerPollJobs) statement. If you wish to remove this lock, you should override the TimerPollJobs property.
        /// </summary>
        protected virtual void TimerPollJobsCreate(ref List<TimerPollJob> jobs)
        {
            lock (syncTimerPollJobs)
            {
                mPollJobs = new List<TimerPollJob>();
                try
                {
                    XmlNodeList nl = this.XmlDataDoc.SelectNodes("//comconf:timerpoll/comconf:polljob", NSM);

                    foreach (XmlElement node in nl)
                        TimerPollJobCreate(jobs, node);
                }
                catch (Exception)
                {

                }
            }
        }
        #endregion // TimerPollJobsCreate(List<TimerPollJob> jobs)
        #region TimerPollJobCreate(List<TimerPollJob> jobs, XmlNode node)
        /// <summary>
        /// This method creates a specific poll job and adds it to the poll job collection.
        /// </summary>
        /// <param name="jobs">The poll job collection.</param>
        /// <param name="node">The configuration node for the poll job.</param>
        protected virtual void TimerPollJobCreate(List<TimerPollJob> jobs, XmlElement node)
        {
            TimerPollJob newJob = new TimerPollJob();
            newJob.Configure(node, NSM, null);
            jobs.Add(newJob);
        }
        #endregion // TimerPollJobCreate(List<TimerPollJob> jobs, XmlNode node)

        #region TimerPollStartTime
        /// <summary>
        /// This is the retry interval for the timer.
        /// </summary>
        public virtual string TimerPollStartTime
        {
            get { return XmlMappingGetToString(XPScA("r", "polltimestartinhhmmss", "comconf:timerpoll")); }
            protected set { XmlMappingSet(XPScA("r", "polltimestartinhhmmss", "comconf:timerpoll"), value); }
        }
        #endregion // TimerPollValue
        #region ValidateStartTime(DateTime nextTime)
        /// <summary>
        /// This method validates the start time.
        /// </summary>
        /// <param name="nextTime">The time to check.</param>
        /// <returns>Returns true if the time is valid.</returns>
        protected virtual bool ValidateStartTime(DateTime nextTime)
        {
            return true;
        }
        #endregion // ValidateStartTime(DateTime nextTime)
        #region TimerPollEndTime
        /// <summary>
        /// This is the retry interval for the timer.
        /// </summary>
        public virtual string TimerPollEndTime
        {
            get { return XmlMappingGetToString(XPScA("r", "polltimeendinhhmmss", "comconf:timerpoll")); }
            protected set { XmlMappingSet(XPScA("r", "polltimeendinhhmmss", "comconf:timerpoll"), value); }
        }
        #endregion // TimerPollValue
        #region ValidateEndTime(DateTime nextTime)
        /// <summary>
        /// This method validates the end time.
        /// </summary>
        /// <param name="nextTime">The time to check.</param>
        /// <returns>Returns true if the time is valid.</returns>
        protected virtual bool ValidateEndTime(DateTime nextTime)
        {
            return true;
        }
        #endregion // ValidateEndTime(DateTime nextTime)

        #region TimerFirstCall
        /// <summary>
        /// This is the timer first call.
        /// </summary>
        protected virtual bool TimerFirstCall { get; set; }
        #endregion // TimerFirstCall
        #region TimerNotSet()
        /// <summary>
        /// This method sets the timer to a not set state.
        /// </summary>
        private void TimerNotSet()
        {
            TimerNextCall = null;
        }
        #endregion // TimerNotSet()
        #region TimerNextCall
        /// <summary>
        /// This is the next time the timer should run.
        /// </summary>
        public virtual DateTime? TimerNextCall
        {
            get;
            protected set;
        }
        #endregion // TimerNextCall
        #region TimerRecalculate()
        /// <summary>
        /// This method recalculates the timer next call.
        /// </summary>
        public virtual void TimerRecalculate()
        {
            TimerNextCall = null;

            //OK, if the pool functionality is not enabled then quit.
            if (!TimerPollEnabled)
                return;

            //OK, calculate the next time.
            DateTime? next = TimerPollJob.DateTimeCalculate(TimerPollValue, TimerPollValueType);

            //OK, check that the next time is after the start time.
            if (!ValidateStartTime(next.Value))
                return;
            //Check that the next time is before the end time.
            if (!ValidateEndTime(next.Value))
                return;
            
            TimerNextCall = next;
        }
        #endregion // TimerRecalculate()
        #region TimerDueTime
        /// <summary>
        /// This is the due time in milliseconds to the next timer call.
        /// </summary>
        public virtual long TimerDueTime
        {
            get
            {
                try
                {
                    if (!TimerNextCall.HasValue)
                        return System.Threading.Timeout.Infinite;

                    TimeSpan span = TimerNextCall.Value.Subtract(DateTime.Now);

                    if (span.TotalMilliseconds < 0)
                        return 1;

                    long milliseconds = (long)span.TotalMilliseconds;

                    return milliseconds>0?milliseconds:1;
                }
                catch (Exception ex)
                {
                    return System.Threading.Timeout.Infinite;
                }
            }
        }
        #endregion // TimerDueTime

        #region SettingGet(string id)
        /// <summary>
        /// This method gets the specific setting.
        /// </summary>
        /// <param name="id">The setting id.</param>
        /// <returns>Returns the setting value, or null if the setting cannot be found or is undefined.</returns>
        public virtual string SettingGet(string id)
        {
            XmlNode node = XmlDataDoc.SelectSingleNode(XPSc("r", "comconf:settings"), NSM);
            if (node == null)
                return null;

            XmlElement elem = (XmlElement)node.SelectSingleNode(string.Format("/r:setting[@id='{0}'", id),NSM);
            return elem == null ? null : elem.Value;
        }
        #endregion // SettingGet(string id)
        #region SettingSet(string id, string value)
        /// <summary>
        /// This protected method sets the specific setting.
        /// </summary>
        /// <param name="id">The setting id.</param>
        /// <param name="value">The value for the setting. Set this to null if you want the setting to be undefined.</param>
        protected virtual void SettingSet(string id, string value)
        {
            //XmlNode node = XmlDataDoc.SelectSingleNode(XPSc("r", "comconf:settings"), NSM);
            //if (node == null)
            //    return null;

            //XmlElement elem = (XmlElement)node.SelectSingleNode(string.Format("/r:setting[@id='{0}'", id), NSM);
            //return elem == null ? null : elem.Value;
            //TODO: Not implemented
        }
        #endregion // SettingSet(string id, string value)

        #region SupportsConfigInitialization
        /// <summary>
        /// This property determines whether the configuration class supports config section handler initiialization.
        /// </summary>
        public virtual bool SupportsConfigInitialization
        {
            get
            {
                return true;
            }
        }
        #endregion // SupportsConfigInitialization

        #region Load
        /// <summary>
        /// This method loads the confiuration data from the command.
        /// </summary>
        /// <param name="appDef"></param>
        /// <param name="commDef"></param>
        /// <param name="data"></param>
        /// <param name="sh"></param>
        /// <returns></returns>
        public virtual bool Load(
            IXimuraApplicationDefinition appDef, IXimuraCommand commDef, Stream data, IXimuraConfigSH sh)
        {
            bool response = false;
            if (data != null)
                response = base.Load(data)>0;
            else
                response = base.Load();

            if (!response)
                return false;

            //if (SupportsConfigInitialization)
            //    return LoadConfigInitialize(appDef, commDef, sh);

            return response;
        }
        #endregion // Load

        #region LoadConfigInitialize
        ///// <summary>
        ///// This method initializes the configuration from the settings.
        ///// </summary>
        ///// <param name="appDef"></param>
        ///// <param name="commDef"></param>
        ///// <param name="sh"></param>
        ///// <returns></returns>
        //protected virtual bool LoadConfigInitialize(
        //    IXimuraApplicationDefinition appDef, IXimuraCommand commDef, IXimuraConfigSH sh)
        //{
        //    AppCommandConfigSH appConfig = sh as AppCommandConfigSH;
        //    if (appConfig == null)
        //        return sh == null;

        //    ID = commDef.CommandID;
        //    //CommandID = commDef.CommandName;
        //    //CommandName = commDef.CommandName;
        //    //CommandDescription = commDef.CommandDescription;
        //    //CommandEnabled = appConfig.Enabled;
        //    CommandPriority = appConfig.Priority;

        //    TimerPollEnabled = appConfig.GetSettingTimer("pollenable") == "true";
        //    TimerAutoStart = appConfig.GetSettingTimer("autostart") == "true";

        //    TimerPollStartTime = appConfig.GetSettingTimer("pollstarttimehhmmss");
        //    TimerPollEndTime = appConfig.GetSettingTimer("pollendtimehhmmss");

        //    int pollValue;
        //    if (int.TryParse(appConfig.GetSettingTimer("polltimerepeatinsec"), out pollValue))
        //    {
        //        TimerPollValue = pollValue;
        //        TimerPollValueType = "s";
        //    }
        //    //else
        //    //{
        //    //    TimerPollValue = 0;
        //    //    TimerPollEnabled = false;
        //    //}


        //    IXimuraCommandProcessConfigSH pConfigSH = appConfig as IXimuraCommandProcessConfigSH;
        //    if (pConfigSH == null)
        //        return true;

        //    ProcessSessionRequired = pConfigSH.RequiresProcessSession;
        //    ProcessSessionUserName = pConfigSH.UserName;
        //    ProcessSessionRealmDomain  = pConfigSH.UserRealm;
        //    ProcessSessionUserRealm = pConfigSH.UserSessionRealm;
        //    ProcessSessionSecurityData = pConfigSH.SecurityData;

        //    return true;
        //}
        #endregion // LoadConfigInitialize
    }

    #region TimerPollJob
    /// <summary>
    /// The Poll job class contains the information for processing regular poll jobs.
    /// </summary>
    public class TimerPollJob
    {
        #region Declarations
        /// <summary>
        /// This dictionary contains the configuration values.
        /// </summary>
        protected Dictionary<string, string> mValues = null;
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor for the timer poll job.
        /// </summary>
        /// <param name="data">The XmlElement containing the data.</param>
        /// <param name="NSM">The namespace manager.</param>
        public TimerPollJob()
        {
        }
        #endregion // Constructors

        #region Configure(XmlElement element, XmlNamespaceManager NSM)
        /// <summary>
        /// This method sets the time poll data from the Xml configuration node.
        /// </summary>
        /// <param name="element">The xml element containing the time poll data.</param>
        /// <param name="NSM">The namespace manager.</param>
        /// <param name="subCommand">The subcommand object.</param>
        public virtual void Configure(XmlElement element, XmlNamespaceManager NSM, object subCommand)
        {
            this.Subcommand = subCommand;

            if (element == null)
                throw new ArgumentNullException("element", "element cannot be null");

            ID = element.HasAttribute("id") ? element.Attributes["id"].Value : (string)null;
            IDType = element.HasAttribute("type") ? element.Attributes["type"].Value : (string)null;

            AutoStart = element.HasAttribute("polltimeautostart") ? element.Attributes["polltimeautostart"].Value == "true" : false;

            Enabled = element.HasAttribute("polltimeenabled") ? element.Attributes["polltimeenabled"].Value == "true" : false;

            int poll;
            if (element.HasAttribute("polltimerepeatvalue") && int.TryParse(element.Attributes["polltimerepeatvalue"].Value, out poll))
                PollValue = poll;
            else
                PollValue = null;

            PollType = element.HasAttribute("polltimerepeattype") ? element.Attributes["polltimerepeattype"].Value : (string)null;

            XmlNodeList nl = element.SelectNodes("r:value", NSM);
            foreach (XmlElement node in nl)
                ValueSet(node, NSM);
        }
        #endregion // ExtractTimeSpan(XmlElement element, bool autoStart)
        #region ValueSet(XmlElement node, XmlNamespaceManager NSM)
        /// <summary>
        /// This method adds the specific values for the extender properties.
        /// </summary>
        /// <param name="node">the value node.</param>
        /// <param name="NSM">The namespace manager.</param>
        protected virtual void ValueSet(XmlElement node, XmlNamespaceManager NSM)
        {
            if (mValues == null)
                mValues = new Dictionary<string, string>();

            string key = node.Attributes["id"].Value;
            if (!mValues.ContainsKey(key))
                mValues.Add(key, node.InnerText);
        }
        #endregion // ValueProcess(XmlElement node, XmlNamespaceManager NSM)

        #region TimerRecalculate(bool autoStart)
        /// <summary>
        /// This method calculates the next time for the job.
        /// </summary>
        /// <param name="autoStart">A boolean value indicating whether this is the first time the method has been called.</param>
        public virtual void TimerRecalculate(bool autoStart)
        {
            NextPollTime = null;

            if (!Enabled)
                return;

            if (autoStart && AutoStart)
            {
                NextPollTime = DateTime.Now;
                return;
            }

            TimeSpan? span = PollInterval;

            if (span.HasValue)
                NextPollTime = DateTime.Now + span;
        }
        #endregion // TimerRecalculate(bool autoStart)

        #region Enabled
        /// <summary>
        /// A boolean property that indicates whether the poll job is enabled.
        /// </summary>
        public bool Enabled { get; protected set; }
        #endregion // Enabled
        #region AutoStart
        /// <summary>
        /// A boolean value indicating whether the command should autostart, i.e. call the command, when it first starts.
        /// </summary>
        public bool AutoStart { get; protected set; }
        #endregion // AutoStart
        #region PollValue
        /// <summary>
        /// The numeric poll value.
        /// </summary>
        public int? PollValue { get; protected set; }
        #endregion // PollValue
        #region PollType
        /// <summary>
        /// The poll value type, i.e. s,m,h,d etc
        /// </summary>
        public string PollType { get; protected set; }
        #endregion // PollType

        #region PollInterval
        /// <summary>
        /// The Timespan interval for the job.
        /// </summary>
        public TimeSpan? PollInterval
        {
            get
            {
                if (PollValue.HasValue)
                    return TimeSpanCalculate(PollValue.Value, PollType);
                else
                    return null;
            }
        }
        #endregion // PollInterval

        #region Active
        /// <summary>
        /// This property determines whether the job is currently active. This property is set when the command is currently 
        /// processing and stops multiple calls being initiated.
        /// </summary>
        public bool Active { get; set; }
        #endregion // Active
        #region ID
        /// <summary>
        /// The job id.
        /// </summary>
        public string ID { get; protected set; }
        #endregion // ID
        #region IDType
        /// <summary>
        /// The job id type.
        /// </summary>
        public string IDType { get; protected set; }
        #endregion // IDType
        #region Subcommand
        /// <summary>
        /// The subcommand object that is passed in the Envelope request.
        /// </summary>
        public object Subcommand { get; set; }
        #endregion // Subcommand
        #region NextPollTime
        /// <summary>
        /// The next poll time, or null if no time is set.
        /// </summary>
        public DateTime? NextPollTime { get; set; }
        #endregion // NextPollTime
        #region RequestFormat
        /// <summary>
        /// This action can be overriden to provide formating support for the timer poll request.
        /// </summary>
        public virtual Action<RQRSFolder> RequestFormat { get; protected set; }
        #endregion // RequestFormat

        #region DateTimeCalculate(DateTime baseTime, double timeValue, string timeType)
        /// <summary>
        /// This method calculates the DateTime value from the time value and time type.
        /// </summary>
        /// <param name="timeValue">The time value.</param>
        /// <param name="timeType">
        /// The time type: 
        ///     ms = milliseconds
        ///      s = seconds
        ///      m = minutes
        ///      h = hours
        ///      d = days
        /// </param>
        /// <returns></returns>
        public static DateTime? DateTimeCalculate(double timeValue, string timeType)
        {
            return DateTimeCalculate(DateTime.Now, timeValue, timeType);
        }
        /// <summary>
        /// This method calculates the DateTime value from the time value and time type.
        /// </summary>
        /// <param name="baseTime">The base time to calculate from.</param>
        /// <param name="timeValue">The time value.</param>
        /// <param name="timeType">
        /// The time type: 
        ///     ms = milliseconds
        ///      s = seconds
        ///      m = minutes
        ///      h = hours
        ///      d = days
        /// </param>
        /// <returns></returns>
        public static DateTime? DateTimeCalculate(DateTime baseTime, double timeValue, string timeType)
        {
            TimeSpan? span = TimerPollJob.TimeSpanCalculate(timeValue, timeType);

            if (span.HasValue)
                return baseTime + span.Value;

            return null;
        }
        #endregion // DateTimeCalculate(DateTime baseTime, double timeValue, string timeType)

        #region TimeSpanCalculate(double timeValue, string timeType)
        /// <summary>
        /// This method calculates the TimeSpan from the time value and time type.
        /// </summary>
        /// <param name="timeValue">The time value.</param>
        /// <param name="timeType">
        /// The time type: 
        ///     ms = milliseconds
        ///      s = seconds
        ///      m = minutes
        ///      h = hours
        ///      d = days
        /// </param>
        /// <returns>Returns a TimeSpan corresponding to the specified time value, or null if the timetype cannot be resolved.</returns>
        public static TimeSpan? TimeSpanCalculate(double timeValue, string timeType)
        {
            switch (timeType)
            {
                case "ms":
                    return TimeSpan.FromMilliseconds(timeValue);
                case "s":
                    return TimeSpan.FromSeconds(timeValue);
                case "m":
                    return TimeSpan.FromMinutes(timeValue);
                case "h":
                    return TimeSpan.FromHours(timeValue);
                case "d":
                    return TimeSpan.FromDays(timeValue);
                default:
                    return null;
            }
        }
        #endregion // TimeSpanCalculate(double timeValue, string timeType)
    }
    #endregion // TimerPollJob

}
