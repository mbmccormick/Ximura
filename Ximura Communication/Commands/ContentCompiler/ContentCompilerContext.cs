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
using Ximura.Data;

using CH = Ximura.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class ContentCompilerContext: 
        JobContext<ContentCompilerState, ContentCompilerSettings,
            ContentCompilerRequest, ContentCompilerResponse, ContentCompilerConfiguration, ContentCompilerPerformance>
    {
        #region Declarations
        InternetMessageFragmentBody mBody;
        Model mModelData = null;
        ModelTemplate mTemplate = null;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public ContentCompilerContext():base()
        {
        }
        #endregion // Constructor

        #region Reset()
        /// <summary>
        /// This override resets the context
        /// </summary>
        public override void Reset()
        {
            //if (mTemplate != null && mTemplate.ObjectPoolCanReturn)
            //    mTemplate.ObjectPoolReturn();
            mTemplate = null;
            //The model does is not transmitted outside the command, so should
            //be returned to the pool when the context resets.
            if (mModelData != null && mModelData.ObjectPoolCanReturn)
                mModelData.ObjectPoolReturn();
            mModelData = null;
            mBody = null;
            base.Reset();
        }
        #endregion // Reset()

        #region Body
        /// <summary>
        /// This is the output body.
        /// </summary>
        public InternetMessageFragmentBody Body
        {
            get { return mBody; }
            set { mBody = value; }
        }
        #endregion // Body

        #region ModelData
        /// <summary>
        /// This is the model that folds the compiled data for the request.
        /// </summary>
        public Model ModelData
        {
            get { return mModelData; }
            set { mModelData = value; }
        }
        #endregion // ModelData

        #region Template
        /// <summary>
        /// This is the template used to generate the model.
        /// </summary>
        public ModelTemplate Template
        {
            get { return mTemplate; }
            set { mTemplate = value; }
        }
        #endregion // Template

        #region StylesheetGet(string refType, string refValue)
        /// <summary>
        /// This method retrieves a stylesheet from the cache.
        /// </summary>
        /// <param name="refValue">The stylesheet name.</param>
        /// <returns>Returns the stylesheet.</returns>
        public Stylesheet StylesheetGet(string refType, string refValue)
        {
            return ContextSettings.CacheGet<Stylesheet>(refType, refValue);
        }
        #endregion // StylesheetGet(string refType, string refValue)

        #region TemplateGet(string refType, string refValue)
        /// <summary>
        /// This method returns a modeltemplate from the cache.
        /// </summary>
        /// <param name="refType">The model template collection type.</param>
        /// <param name="refValue">The model template name</param>
        /// <returns></returns>
        public ModelTemplate TemplateGet(string refType, string refValue)
        {
            return ContextSettings.CacheGet<ModelTemplate>(refType, refValue);
        }
        #endregion // TemplateGet(string refType, string refValue)

        #region Initialize()
        /// <summary>
        /// This method initializes the context.
        /// </summary>
        public void Initialize()
        {
            //OK, set the initial state.
            this.ChangeState();
            CurrentState.Initialize(this);
        }
        #endregion
        #region Finish()
        /// <summary>
        /// This method completes the current request.
        /// </summary>
        public void Finish()
        {
            this.ChangeState("Finish");
            CurrentState.Finish(this);
        }
        #endregion // Finish()

        #region RequestValidate()
        /// <summary>
        /// This method validates the request.
        /// </summary>
        public void RequestValidate()
        {
            CurrentState.RequestValidate(this);
        }
        #endregion // RequestValidate()

        #region ModelTemplateLoad()
        /// <summary>
        /// This method loads the template.
        /// </summary>
        public void ModelTemplateLoad()
        {
            CurrentState.ModelTemplateLoad(this);
        }
        #endregion // ModelTemplateLoad()

        #region ModelTemplateCompile()
        /// <summary>
        /// This method compiles the template.
        /// </summary>
        public void ModelTemplateCompile()
        {
            CurrentState.ModelTemplateCompile(this);
        }
        #endregion // ModelTemplateCompile()

        #region ModelDataLoadInserts()
        /// <summary>
        /// This method loads any insert data in the Model
        /// </summary>
        public void ModelDataLoadInserts()
        {
            CurrentState.ModelDataLoadInserts(this);
        }
        #endregion // ModelDataLoadInserts()


        #region OutputSelect()
        /// <summary>
        /// This method selects the appropriate output method, i.e. XSLT, or XPath
        /// </summary>
        public void OutputSelect()
        {
            this.ChangeState("OutputSelector");
            CurrentState.OutputSelect(this);
        }
        #endregion // OutputSelect()


        public void OutputPrepare()
        {
            CurrentState.OutputPrepare(this);
        }

        public void OutputComplete()
        {
            CurrentState.OutputComplete(this);
        }



    }
}
