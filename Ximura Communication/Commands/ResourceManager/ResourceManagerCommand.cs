#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using CDS = Ximura.Data.CDSHelper;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The Resource Manager is an extension of the Content Data Store. The Resource Manager has built in 
    /// cataloging extensions of various content types to enable content to be stored in logical groupings that have
    /// been predefined.
    /// </summary>
    [XimuraAppModule("40BC9B93-92F8-4f66-8698-E59A9631876D", "ResourceManagerCommand")]
#if (DEBUG)
    [XimuraAppConfiguration(ConfigurationLocation.Hybrid,
        "xmrres://XimuraComm/Ximura.Communication.ResourceManagerCommand/Ximura.Communication.Commands.ResourceManager.Configuration.ResourceManagerConfiguration_Default.xml")]
#else
    [XimuraAppConfiguration(ConfigurationLocation.Hybrid, 
        "xmrres://XimuraComm/Ximura.Communication.ResourceManagerCommand/Ximura.Communication.Commands.ResourceManager.Configuration.ResourceManagerConfiguration_Default.xml")]
#endif
    public class ResourceManagerCommand : FiniteStateMachine<ResourceManagerRequest, ResourceManagerResponse,
        RQRSFolder,RQRSFolder,ResourceManagerContext,ResourceManagerState,ResourceManagerSettings
        , ResourceManagerConfiguration, ResourceManagerPerformance>
    {
        #region Declarations
        private System.ComponentModel.IContainer components = null;
        StartRMState startState;
        FinishRMState finishState;
        OutputSelectorRMState outputSelectorState;
        OutputResourceRMState outputResourceState;
        OutputFileRMState outputFileState;
        OutputCDSRMState outputCDSState;

        ETagCacheFlushRMState etagCacheFlushState;
        #endregion
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public ResourceManagerCommand() : this(null) { }
        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public ResourceManagerCommand(System.ComponentModel.IContainer container)
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
            startState = new StartRMState(components);
            finishState = new FinishRMState(components);
            outputSelectorState = new OutputSelectorRMState(components);
            outputResourceState = new OutputResourceRMState(components);
            outputFileState = new OutputFileRMState(components);
            outputCDSState = new OutputCDSRMState(components);
            etagCacheFlushState = new ETagCacheFlushRMState(components);

            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).BeginInit();
            // 
            // startState
            // 
            this.mStateExtender.SetEnabled(this.startState, true);
            this.startState.Identifier = "Start";
            this.mStateExtender.SetNextStateID(this.startState, null);
            this.mStateExtender.SetStateID(this.startState, "Start");
            // 
            // finishState
            //
            this.mStateExtender.SetEnabled(this.finishState, true);
            this.finishState.Identifier = "Finish";
            this.mStateExtender.SetNextStateID(this.finishState, null);
            this.mStateExtender.SetStateID(this.finishState, "Finish");
            // 
            // outputSelectorState
            // 
            this.mStateExtender.SetEnabled(this.outputSelectorState, true);
            this.outputSelectorState.Identifier = "OutputSelector";
            this.mStateExtender.SetNextStateID(this.outputSelectorState, null);
            this.mStateExtender.SetStateID(this.outputSelectorState, "OutputSelector");

            // 
            // outputResourceState
            // 
            this.mStateExtender.SetEnabled(this.outputResourceState, true);
            this.outputResourceState.Identifier = "RS_OutputResource";
            this.mStateExtender.SetNextStateID(this.outputResourceState, null);
            this.mStateExtender.SetStateID(this.outputResourceState, "RS_OutputResource");

            //
            // outputFileState
            //
            this.mStateExtender.SetEnabled(this.outputFileState, true);
            this.outputFileState.Identifier = "RS_OutputFile";
            this.mStateExtender.SetNextStateID(this.outputFileState, null);
            this.mStateExtender.SetStateID(this.outputFileState, "RS_OutputFile");

            // 
            // outputCDSState
            // 
            this.mStateExtender.SetEnabled(this.outputCDSState, true);
            this.outputCDSState.Identifier = "RS_OutputCDS";
            this.mStateExtender.SetNextStateID(this.outputCDSState, null);
            this.mStateExtender.SetStateID(this.outputCDSState, "RS_OutputCDS");

            // 
            // etagCacheFlushState
            // 
            this.mStateExtender.SetEnabled(this.etagCacheFlushState, true);
            this.etagCacheFlushState.Identifier = "ETagCacheFlush";
            this.mStateExtender.SetNextStateID(this.etagCacheFlushState, null);
            this.mStateExtender.SetStateID(this.etagCacheFlushState, "ETagCacheFlush");


            // 
            // State_Extender
            // 
            this.mStateExtender.InitialState = "Start";
            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).EndInit();
        }
        #endregion

        #region ContextInitialize
        protected override void ContextInitialize(ResourceManagerContext newContext,
            SecurityManagerJob job, RQRSContract<ResourceManagerRequest, ResourceManagerResponse> Data)
        {
            //Reset the context with the job and the data.
            newContext.Reset(ContextConnection, job, Data, mRemoteContextPoolAccess);
        }
        #endregion // ContextInitialize

        #region ProcessRequest(ResourceManagerContext context)
        /// <summary>
        /// This request retrieves the relevant resource and prepares it for HTTP output.
        /// </summary>
        /// <param name="context">The request context.</param>
        protected override void ProcessRequest(ResourceManagerContext context)
        {
            try
            {
                //This method sets up the inital state.
                context.Initialize();

                //Parse the request settings, and validate them,
                //check here whether we have an Etag to validate against.
                if (context.RequestValidate())
                {
                    //OK, we are sending a binary response,
                    //select the appropriate output state, such as resource, path
                    context.OutputSelect();

                    //Create the output body.
                    context.OutputPrepare();

                    //Update the cache collection.
                    context.OutputComplete();
                }
            }
            catch (ResourceManagerException ccex)
            {
                context.Response.Status = ccex.ResponseCode.ToString();
                context.Response.Substatus = ccex.Message;
            }
            catch (Exception ex)
            {
                context.Response.Status = CH.HTTPCodes.InternalServerError_500;
                context.Response.Substatus = ex.Message;
            }
            finally
            {
                //Finish up and get out of here.
                context.Finish();
            }
        } 
        #endregion
    }
}
