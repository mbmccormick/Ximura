#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.ServiceModel.Channels;
#endregion // using
namespace Ximura.WCF
{
    /// <summary>
    /// This is the dispatcher that intercepts the WCF calls.
    /// </summary>
    public class Dispatcher : 
        IInstanceProvider, IInputSessionShutdown,
        IContractBehavior, IEndpointBehavior,
        IMetadataExchange, IOperationBehavior,
        IOperationContractGenerationExtension, 
        IServiceBehavior,
        IPolicyExportExtension, IPolicyImportExtension
    {
        #region Constructor
        /// <summary>
        /// The default constructor.
        /// </summary>
        public Dispatcher()
        {

        }
        #endregion // Constructor


        #region IInstanceProvider Members

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return null;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IInputSessionShutdown Members

        public void ChannelFaulted(IDuplexContextChannel channel)
        {

        }

        public void DoneReceiving(IDuplexContextChannel channel)
        {

        }

        #endregion

        #region IContractBehavior Members

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {

        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {

        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {

        }

        #endregion

        #region IEndpointBehavior Members
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
        #endregion

        #region IMetadataExchange Members

        public IAsyncResult BeginGet(Message request, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public Message EndGet(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public Message Get(Message request)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IOperationBehavior Members

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
            throw new NotImplementedException();
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            throw new NotImplementedException();
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            throw new NotImplementedException();
        }

        public void Validate(OperationDescription operationDescription)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IOperationContractGenerationExtension Members

        public void GenerateOperation(OperationContractGenerationContext context)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            throw new NotImplementedException();
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            throw new NotImplementedException();
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IPolicyExportExtension Members

        public void ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IPolicyImportExtension Members

        public void ImportPolicy(MetadataImporter importer, PolicyConversionContext context)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
