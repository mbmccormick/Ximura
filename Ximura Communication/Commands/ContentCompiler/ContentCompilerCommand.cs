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
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;

using Ximura;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// The content compiler command is used to produce customized content for each individual user from a common set of Model Data Templates.
    /// </summary>
    [XimuraAppModule("A33D715F-6960-45ad-B4C9-A3BF7FBFD15C", "ContentCompilerCommand")]
#if (DEBUG)
    [XimuraAppConfiguration(ConfigurationLocation.Hybrid,
        "xmrres://XimuraComm/Ximura.Communication.ContentCompilerCommand/Ximura.Communication.Commands.ContentCompiler.Configuration.ContentCompilerConfiguration_Default.xml")]
#else
    [XimuraAppConfiguration(ConfigurationLocation.Hybrid, 
        "xmrres://XimuraComm/Ximura.Communication.ContentCompilerCommand/Ximura.Communication.Commands.ContentCompiler.Configuration.ContentCompilerConfiguration_Default.xml")]
#endif
    public class ContentCompilerCommand : FiniteStateMachine<ContentCompilerRequest,ContentCompilerResponse,RQRSFolder,RQRSFolder,
        ContentCompilerContext, ContentCompilerState, ContentCompilerSettings, ContentCompilerConfiguration, ContentCompilerPerformance>
    {
        #region Declarations
        private System.ComponentModel.IContainer components = null;
        StartCCState startState;
        FinishCCState finishState;
        OutputSelectorCCState outputSelectorState;

        ModelDataTemplateCompilerCCState modeldataTemplateState;
        PreTransformInsertionCCState preTransformInsertionState;
        OutputTransformCCState outputTransformState;
        OutputMultipartMIMECCState outputMultipartMIMEState;
        OutputXPathCCState outputXPathState;

        #endregion
        #region Constructors
        /// <summary>
        /// Empty constructor for use by the component model.
        /// </summary>
        public ContentCompilerCommand() : this(null) { }
        /// <summary>
        /// Default constructor for use by the component model.
        /// </summary>
        /// <param name="container">The container that the transport should be added to.</param>
        public ContentCompilerCommand(System.ComponentModel.IContainer container):base(container)
        {
            InitializeComponent();
            RegisterContainer(components);
        }
        #endregion

        #region InitializeComponent()
        /// <summary>
        /// This method initializes the components for the Site Manager.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            startState = new StartCCState(components);
            finishState = new FinishCCState(components);

            modeldataTemplateState = new ModelDataTemplateCompilerCCState(components);
            outputSelectorState = new OutputSelectorCCState(components);
            preTransformInsertionState = new PreTransformInsertionCCState(components);

            outputTransformState = new OutputTransformCCState(components);
            outputXPathState = new OutputXPathCCState(components);
            outputMultipartMIMEState = new OutputMultipartMIMECCState(components);

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
            // outputMultipartMIMEState
            // 
            this.mStateExtender.SetEnabled(this.outputMultipartMIMEState, true);
            this.outputMultipartMIMEState.Identifier = "OutputMultipartMIME";
            this.mStateExtender.SetNextStateID(this.outputMultipartMIMEState, null);
            this.mStateExtender.SetStateID(this.outputMultipartMIMEState, "OutputMultipartMIME");

            // 
            // outputTransformState
            // 
            this.mStateExtender.SetEnabled(this.outputTransformState, true);
            this.outputTransformState.Identifier = "OutputTransform";
            this.mStateExtender.SetNextStateID(this.outputTransformState, null);
            this.mStateExtender.SetStateID(this.outputTransformState, "OutputTransform");
            // 
            // outputXPathState
            // 
            this.mStateExtender.SetEnabled(this.outputXPathState, true);
            this.outputXPathState.Identifier = "OutputXPath";
            this.mStateExtender.SetNextStateID(this.outputXPathState, null);
            this.mStateExtender.SetStateID(this.outputXPathState, "OutputXPath");

            // 
            // preTransformInsertionState
            // 
            this.mStateExtender.SetEnabled(this.preTransformInsertionState, true);
            this.preTransformInsertionState.Identifier = "PreTransform";
            this.mStateExtender.SetNextStateID(this.preTransformInsertionState, null);
            this.mStateExtender.SetStateID(this.preTransformInsertionState, "PreTransform");
            // 
            // outputSelectorState
            // 
            this.mStateExtender.SetEnabled(this.outputSelectorState, true);
            this.outputSelectorState.Identifier = "OutputSelector";
            this.mStateExtender.SetNextStateID(this.outputSelectorState, null);
            this.mStateExtender.SetStateID(this.outputSelectorState, "OutputSelector");
            // 
            // modeldataTemplateState
            // 
            this.mStateExtender.SetEnabled(this.modeldataTemplateState, true);
            this.modeldataTemplateState.Identifier = "ModelDataCompiler";
            this.mStateExtender.SetNextStateID(this.modeldataTemplateState, null);
            this.mStateExtender.SetStateID(this.modeldataTemplateState, "ModelDataCompiler");

            // 
            // State_Extender
            // 
            this.mStateExtender.InitialState = "Start";
            ((System.ComponentModel.ISupportInitialize)(this.mStateExtender)).EndInit();
        }
        #endregion

        #region ProcessRequest(ContentCompilerContext context)
        /// <summary>
        /// This method processes the controller compilation request.
        /// </summary>
        /// <param name="jobContext">The specific job context.</param>
        protected override void ProcessRequest(ContentCompilerContext context)
        {
            try
            {
                //This method sets up the inital state.
                context.Initialize();

                //Parse the request settings, and validate them.
                context.RequestValidate();

                //Get the Model Data template
                context.ModelTemplateLoad();

                //Merge the template to get the model data
                context.ModelTemplateCompile();

                //OK, now loop through the ModelData and include any merge content.
                context.ModelDataLoadInserts();

                //OK, select the appropriate output state, such as XPath, XSLTTransform etc
                context.OutputSelect();

                //Prepares the output, such as transforming the XML
                context.OutputPrepare();

                //Process the output. adding any post compile features such as inserts
                context.OutputComplete();

            }
            catch (ContentCompilerException ccex)
            {
                context.Response.Body = null;
                context.Response.Status = ccex.ResponseCode.ToString();
                context.Response.Substatus = ccex.Message;
            }
            catch (Exception ex)
            {
                context.Response.Body = null;
                context.Response.Status = CH.HTTPCodes.InternalServerError_500;
                context.Response.Substatus = ex.Message;
            }
            finally
            {
                //Finish up and get out of here.
                context.Finish();
            }
        }
        #endregion // ProcessRequest(ContentCompilerContext context)

        #region ContextInitialize
        protected override void ContextInitialize(ContentCompilerContext newContext,
            SecurityManagerJob job, RQRSContract<ContentCompilerRequest, ContentCompilerResponse> Data)
        {
            //Reset the context with the job and the data.
            newContext.Reset(ContextConnection, job, Data, mRemoteContextPoolAccess);
        }
        #endregion // ContextInitialize
    }
}
