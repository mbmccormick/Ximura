#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Ximura;
using Ximura.Data;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using CDS = Ximura.Data.CDSHelper;
using Ximura.Data;
using Ximura.Framework;
using Ximura.Framework;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the content data store for the internet server.
    /// </summary>
    public class InternetServerCDSCommand: ContentDataStore
    {
        #region Declarations
        private System.ComponentModel.Container components;

        private ControllerScriptPM controllerScriptPM;
        private StylesheetPM<ModelTemplate, ModelTemplate> modelTemplatePM;
        private StylesheetPM<Stylesheet, Stylesheet> styleSheetPM;

		#endregion
        #region Constructors
		/// <summary>
		/// Empty constructor used during the design mode.
		/// </summary>
		public InternetServerCDSCommand():this((IContainer)null){}
		/// <summary>
		/// This is the component model constructor.
		/// </summary>
        public InternetServerCDSCommand(IContainer container)
            : base(container)
		{
			InitializeComponents();
			RegisterContainer(components);
		}
		#endregion

        #region InitializeComponents()
        private void InitializeComponents()
        {
            components = new System.ComponentModel.Container();
        }
        #endregion // InitializeComponents()
    }
}
