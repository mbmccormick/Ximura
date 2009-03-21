#region using
using System;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;

using Ximura;
using Ximura.Helper;
using CH = Ximura.Helper.Common;
using Ximura.Server;
using Ximura.Command;
using Ximura.Communication;
#endregion // using
namespace Ximura.Communication
{
    /// <summary>
    /// This is the start state.
    /// </summary>
    public class StartState : SiteControllerState
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public StartState() : this(null) { }
        /// <summary>
        /// This is the component model constructor.
        /// </summary>
        /// <param name="container">The container</param>
        public StartState(IContainer container)
            : base(container)
        {
        }
        #endregion // Constructors

        #region IN --> Initialize(SiteManagerContext context)
        /// <summary>
        /// This method initializes the request and resolves the request to the appropriate section.
        /// </summary>
        /// <param name="context">The current context.</param>
        public override void Initialize(SiteControllerContext context)
        {
            try
            {
                MappingSettings map;
                IDictionary<string, string> variables;

                //Resolve the incoming Uri from the scripts.
                bool resolved = context.ContextSettings.ResolvePath(
                    context.RequestServerType,
                    context.RequestURI,
                    context.RequestUserAgent,
                    context.RequestMethod,
                    out variables,
                    out map);

                context.ScriptSettings = map;
                context.ScriptRequestResolved = resolved;

                context.ScriptRequest.ResponseID = map.MappingID;
                context.ScriptRequest.ResponseTemplate = map.Template;

                //Add the variables for the request.
                foreach (KeyValuePair<string, string> item in variables)
                {
                    context.ScriptRequest.VariableAdd(item.Key, item.Value);
                }
                //Add any Uri or script query parameters
                if (map.VariableColl != null && map.VariableColl.Count > 0)
                {
                    foreach (VariableHolder vh in map.VariableColl)
                    {
                        switch (vh.VariableType)
                        {
                            case "parameter":
                                context.ScriptRequest.VariableAdd(vh.VariableID, vh.Variable);
                                break;
                            case "query":
                                context.ScriptRequest.RequestQueryParameterSet(vh.VariableID, vh.Variable);
                                break;
                        }
                    }
                }

                context.CheckChangeState("PR_" + map.ProtocolState);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
