#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    public class ResourceManagerContext :
        JobContext<ResourceManagerState, ResourceManagerSettings, ResourceManagerRequest, ResourceManagerResponse, ResourceManagerConfiguration, ResourceManagerPerformance>
    {
        #region Declarations
        InternetMessageFragmentBody mBody;
        #endregion // Declarations
        #region Constructor
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public ResourceManagerContext()
            : base()
        {
        }
        #endregion // Constructor

        #region Reset()
        /// <summary>
        /// This override resets the context
        /// </summary>
        public override void Reset()
        {
            mBody = null;
            base.Reset();
        }
        #endregion // Reset()

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
        public bool RequestValidate()
        {
            return CurrentState.RequestValidate(this);
        }
        #endregion // RequestValidate()

        #region OutputSelect()
        /// <summary>
        /// This emthod selects the appropriate output method, i.e. XSLT, or XPath
        /// </summary>
        public void OutputSelect()
        {
            CheckChangeState("OutputSelector");
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
