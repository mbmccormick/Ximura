#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Net.Security;
using System.ServiceModel.Security.Tokens;
using System.IdentityModel.Selectors;
using System.ServiceModel.Security;
using System.Reflection;
#endregion // using
namespace Ximura.WCF
{
    public class Server
    {
        static void Main(string[] args)
        {
            X509Certificate2 cert2 = new X509Certificate2();

            ServiceHost Host =
                CreateHost<DoSomethingSimple, IDoSomethingSimple>(
                new Uri("https://localhost:8089/WCF"), true);

            Host.Open();

            Console.WriteLine("Server listening: press any key to finish.");
            Console.ReadLine();
            Console.WriteLine("Server closing ...");

            Host.Close();

            Console.WriteLine("Server closed!");
        }

        public static ServiceHost CreateHost<S, I>(Uri uri, bool publishMetaData)
            where S : class, I, new()
        {

            ServiceHost Host = new ServiceHost(typeof(S));

            //ServiceEndpoint ep = Host.AddServiceEndpoint(typeof(I), ConnectionWindowsBinding(), uri);
            ServiceEndpoint ep = Host.AddServiceEndpoint(typeof(I), ConnectionUsernameBinding(), uri);

            //Host.Credentials.ServiceCertificate.SetCertificate(
            //    StoreLocation.LocalMachine,
            //    StoreName.My,
            //    X509FindType.FindBySubjectName,
            //    "Contoso.com");

            //if (publishMetaData)
            //{
            //    // Add the meta data (MEX) publishing            
            //    ServiceMetadataBehavior smb = Host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            //    if (smb == null)
            //        smb = new ServiceMetadataBehavior();
            //    smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            //    smb.HttpGetEnabled = true;
            //    smb.HttpGetUrl = new Uri(uri.AbsoluteUri + @"/mex");
            //    Host.Description.Behaviors.Add(smb);
            //    // Add MEX endpoint
            //    Host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName
            //        , MetadataExchangeBindings.CreateMexHttpBinding()
            //        , smb.HttpGetUrl);
            //}

            ServiceCredentials scb = new ServiceCredentials();
            scb.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            scb.UserNameAuthentication.CustomUserNamePasswordValidator = new CustomUserNameValidator();
            Host.Description.Behaviors.Add(scb);

            return Host;
        }

        #region ConnectionUsernameBinding()
        /// <summary>
        /// This method returns a standard binding.
        /// </summary>
        /// <returns>This returns the binding.</returns>
        protected static Binding ConnectionUsernameBinding()
        {
            //OK, now do the binding.
            WSHttpBinding binding = new WSHttpBinding();

            binding.Name = "bindingStandard";
            //binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;

            binding.ReliableSession.Enabled = true;

            binding.Security.Mode = SecurityMode.TransportWithMessageCredential;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;

            binding.MaxReceivedMessageSize = 255000;// int.MaxValue;

            binding.TransactionFlow = true;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;


            return binding;
        }
        #endregion
        #region ConnectionWindowsBinding()
        /// <summary>
        /// This property returns the binding for the connection.
        /// </summary>
        /// <returns>The binding.</returns>
        protected static Binding ConnectionWindowsBinding()
        {
            WSHttpBinding binding = new WSHttpBinding();

            binding.Name = "bindingWindows";
            binding.ReliableSession.Enabled = true;
            binding.Security.Mode = SecurityMode.Message;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            binding.MaxReceivedMessageSize = 255000;

            binding.TransactionFlow = true;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;

            return binding;
        }
        #endregion

        public static string DecryptEncryptedData(string Base64EncryptedData, string PathToPrivateKeyFile)
        {
            X509Certificate2 myCertificate;
            try
            {
                myCertificate = new X509Certificate2(PathToPrivateKeyFile);
            }
            catch
            {
                throw new CryptographicException("Unable to open key file.");
            }

            RSACryptoServiceProvider rsaObj;
            if (myCertificate.HasPrivateKey)
            {
                rsaObj = (RSACryptoServiceProvider)myCertificate.PrivateKey;
            }
            else
                throw new CryptographicException("Private key not contained within certificate.");

            if (rsaObj == null)
                return String.Empty;

            byte[] decryptedBytes;
            try
            {
                decryptedBytes = rsaObj.Decrypt(Convert.FromBase64String(Base64EncryptedData), false);
            }
            catch
            {
                throw new CryptographicException("Unable to decrypt data.");
            }

            //    Check to make sure we decrpyted the string 
            if (decryptedBytes.Length == 0)
                return String.Empty;
            else
                return System.Text.Encoding.UTF8.GetString(decryptedBytes);
        }


        // These public methods create custom bindings based on the built-in 
        // authentication modes that use the static methods of 
        // the System.ServiceModel.Channels.SecurityBindingElement class.
        // http://msdn.microsoft.com/en-us/library/aa751836.aspx
        // http://msdn.microsoft.com/en-us/library/aa702632.aspx
        public static Binding CreateAnonymousForCertificateBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.
                CreateAnonymousForCertificateBindingElement());
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateAnonymousForSslNegotiatedBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.
                CreateSslNegotiationBindingElement(false));
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateCertificateOverTransportBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.
                CreateCertificateOverTransportBindingElement());
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpsTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateIssuedTokenBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.
                CreateIssuedTokenBindingElement(
                new IssuedSecurityTokenParameters()));
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateIssuedTokenForCertificateBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.
                CreateIssuedTokenForCertificateBindingElement(
                new IssuedSecurityTokenParameters()));
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateIssuedTokenForSslNegotiatedBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.
                CreateIssuedTokenForSslBindingElement(
                new IssuedSecurityTokenParameters()));
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateIssuedTokenOverTransportBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.
                CreateIssuedTokenOverTransportBindingElement(
                new IssuedSecurityTokenParameters()));
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpsTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateKerberosBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.CreateKerberosBindingElement());
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateKerberosOverTransportBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.
                CreateKerberosOverTransportBindingElement());
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpsTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateMutualCertificateBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.
                CreateMutualCertificateBindingElement());
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateMutualCertificateDuplexBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.
                CreateMutualCertificateDuplexBindingElement());
            bec.Add(new CompositeDuplexBindingElement());
            bec.Add(new OneWayBindingElement());
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateMutualSslNegotiatedBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.
                CreateSslNegotiationBindingElement(true));
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateSecureConversationBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.
                CreateSecureConversationBindingElement(
                SecurityBindingElement.CreateSspiNegotiationBindingElement()));
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateSspiNegotiatedBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.CreateSspiNegotiationBindingElement());
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateSspiNegotiatedOverTransportBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.
                CreateSspiNegotiationOverTransportBindingElement());
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpsTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateUserNameForCertificateBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.
                CreateUserNameForCertificateBindingElement());
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateUserNameForSslNegotiatedBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.CreateUserNameForSslBindingElement());
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpTransportBindingElement());
            return new CustomBinding(bec);
        }

        public static Binding CreateUserNameOverTransportBinding()
        {
            BindingElementCollection bec = new BindingElementCollection();
            bec.Add(SecurityBindingElement.
                CreateUserNameOverTransportBindingElement());
            bec.Add(new TextMessageEncodingBindingElement());
            bec.Add(new HttpsTransportBindingElement());
            //bec.Add(new HttpTransportBindingElement());
            return new CustomBinding(bec);
        }

        // This method creates a CustomBinding based on a WSFederationHttpBinding which does not use secure conversation.
        public static CustomBinding CreateFederationBindingWithoutSecureSession(WSFederationHttpBinding inputBinding)
        {
            // This CustomBinding starts out identical to the specified WSFederationHttpBinding.
            CustomBinding outputBinding = new CustomBinding(inputBinding.CreateBindingElements());
            // Find the SecurityBindingElement for message security.
            SecurityBindingElement security = outputBinding.Elements.Find<SecurityBindingElement>();
            // If the security mode is message, then the secure session settings are the protection token parameters.
            SecureConversationSecurityTokenParameters secureConversation;
            if (WSFederationHttpSecurityMode.Message == inputBinding.Security.Mode)
            {
                SymmetricSecurityBindingElement symmetricSecurity = security as SymmetricSecurityBindingElement;
                secureConversation = symmetricSecurity.ProtectionTokenParameters as SecureConversationSecurityTokenParameters;
            }
            // If the security mode is message, then the secure session settings are the endorsing token parameters.
            else if (WSFederationHttpSecurityMode.TransportWithMessageCredential == inputBinding.Security.Mode)
            {
                TransportSecurityBindingElement transportSecurity = security as TransportSecurityBindingElement;
                secureConversation = transportSecurity.EndpointSupportingTokenParameters.Endorsing[0] as SecureConversationSecurityTokenParameters;
            }
            else
            {
                throw new NotSupportedException(String.Format("Unhandled security mode {0}.", inputBinding.Security.Mode));
            }
            // Replace the secure session SecurityBindingElement with the bootstrap SecurityBindingElement.
            int securityIndex = outputBinding.Elements.IndexOf(security);
            outputBinding.Elements[securityIndex] = secureConversation.BootstrapSecurityBindingElement;
            // Return modified binding.
            return outputBinding;
        }

        private static void InstallCertificate(string cerFileName)
        {
            X509Certificate2 certificate = new X509Certificate2(cerFileName);
            X509Store store = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine);

            store.Open(OpenFlags.ReadWrite);
            store.Add(certificate);
            store.Close();
        }

        private void Hmm()
        {
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ClientCertificates.MyRoot.cer");
            byte[] buffer = new byte[((int)(manifestResourceStream.Length - 1L)) + 1];
            manifestResourceStream.Read(buffer, 0, (int)manifestResourceStream.Length);
            manifestResourceStream.Close();

            var cert = new X509Certificate2(buffer);
            var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            store.Add(cert);
            store.Close();

            /*
            // The CRL is also needed, no idea why
            manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ClientCertificates.MyRoot.crl");
            buffer = new byte[((int)(manifestResourceStream.Length - 1L)) + 1];
            manifestResourceStream.Read(buffer, 0, (int)manifestResourceStream.Length);
            manifestResourceStream.Close();
            cert = new X509Certificate2(buffer);
            store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            store.Add(cert);
            store.Close();
             * */

            // This is the key 
            manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ClientCertificates.MyTestServer.cer");
            buffer = new byte[((int)(manifestResourceStream.Length - 1L)) + 1];
            manifestResourceStream.Read(buffer, 0, (int)manifestResourceStream.Length);
            manifestResourceStream.Close();

            cert = new X509Certificate2(buffer);
            store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            store.Add(cert);
            store.Close();
        }
    }

    public class CustomUserNameValidator : UserNamePasswordValidator
    {
        public CustomUserNameValidator()
        {

        }
        // This method validates users. It allows in two users, test1 and test2 
        // with passwords 1tset and 2tset respectively.
        // This code is for illustration purposes only and 
        // must not be used in a production environment because it is not secure.	
        public override void Validate(string userName, string password)
        {
            if (null == userName || null == password)
            {
                throw new ArgumentNullException();
            }

            if (!(userName == "test1" && password == "1tset") && !(userName == "test2" && password == "2tset"))
            {
                // This throws an informative fault to the client.
                throw new FaultException("Unknown Username or Incorrect Password");
                // When you do not want to throw an infomative fault to the client,
                // throw the following exception.
                // throw new SecurityTokenException("Unknown Username or Incorrect Password");
            }
        }
    }


}
