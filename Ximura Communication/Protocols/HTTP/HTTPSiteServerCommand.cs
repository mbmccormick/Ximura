#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using AH = Ximura.Helper.AttributeHelper;
using Ximura.Framework;
using Ximura.Framework;
#endregion // using
namespace Ximura.Communication
{
    [XimuraAppModule("00A8A9C2-0739-4a5f-B2DD-F5BC134CB85E", "HTTPSiteServerCommand")]
#if (DEBUG)
    [XimuraAppConfiguration(ConfigurationLocation.Resource, 
        "xmrres://AegeaConnectCMS/Aegea.Connect.HTTPSiteServerCommand/Aegea.Connect.Commands.SiteServerProtocols.HTTP.Configuration.HTTPSiteServerConfiguration_Debug.xml")]
#else
    [XimuraAppConfiguration(ConfigurationLocation.Hybrid, 
        "xmrres://AegeaConnectCMS/Aegea.Connect.HTTPSiteServerCommand/Aegea.Connect.Commands.SiteServerProtocols.HTTP.Configuration.HTTPSiteServerConfiguration_Production.xml")]
#endif
    public class HTTPSiteServerCommand : SiteServerCommand
    {
        #region Declarations
        private System.ComponentModel.IContainer components = null;

        protected AwaitRequestHTTPSSState awaitRequestState;
        protected ProcessDataHTTPSSState processDataState;
        protected ConnectionHTTPSSState connectionState;
        #endregion
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public HTTPSiteServerCommand() : this(null) { }

        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public HTTPSiteServerCommand(System.ComponentModel.IContainer container)
            : base(container)
        {
            InitializeComponent();
            RegisterContainer(components);
        }
        #endregion

        #region InitializeComponent()
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            awaitRequestState = new AwaitRequestHTTPSSState(components);
            processDataState = new ProcessDataHTTPSSState(components);
            connectionState = new ConnectionHTTPSSState(components);

            ((System.ComponentModel.ISupportInitialize)(this.MessageResolver_Extender)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).BeginInit();

            // 
            // startState
            // 
            this.mStateExtender.SetEnabled(this.startState, true);
            this.mStateExtender.SetNextStateID(this.startState, null);
            this.mStateExtender.SetStateID(this.startState, "Start");
            // 
            // closeState
            // 
            this.mStateExtender.SetEnabled(this.closeState, true);
            this.mStateExtender.SetNextStateID(this.closeState, null);
            this.mStateExtender.SetStateID(this.closeState, "Close");

            // 
            // listenCfulState
            // 
            this.mStateExtender.SetEnabled(this.listenCfulState, true);
            this.mStateExtender.SetNextStateID(this.listenCfulState, null);
            this.mStateExtender.SetStateID(this.listenCfulState, "ListenConnectionful");
            // 
            // listenClessState
            // 
            this.mStateExtender.SetEnabled(this.listenClessState, false);
            this.mStateExtender.SetNextStateID(this.listenClessState, null);
            this.mStateExtender.SetStateID(this.listenClessState, "ListenConnectionless");

            // 
            // connectionState
            // 
            this.mStateExtender.SetEnabled(this.connectionState, true);
            this.mStateExtender.SetNextStateID(this.connectionState, null);
            this.mStateExtender.SetStateID(this.connectionState, "Connection");

            // 
            // awaitRequestState
            // 
            this.mStateExtender.SetEnabled(this.awaitRequestState, true);
            this.mStateExtender.SetNextStateID(this.awaitRequestState, null);
            this.mStateExtender.SetStateID(this.awaitRequestState, "AwaitRequest");
            // 
            // processDataState
            // 
            this.mStateExtender.SetEnabled(this.processDataState, true);
            this.mStateExtender.SetNextStateID(this.processDataState, null);
            this.mStateExtender.SetStateID(this.processDataState, "ProcessData");


            // 
            // MessageResolver_Extender
            // 
            this.MessageResolver_Extender.Enabled = false;
            // 
            // State_Extender
            // 
            this.mStateExtender.InitialState = "Start";

            ((System.ComponentModel.ISupportInitialize)(this.MessageResolver_Extender)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).EndInit();
        }
        #endregion


    }
}
