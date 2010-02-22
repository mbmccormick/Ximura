#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IdentityModel.Claims;
using System.ServiceModel.Security;
using System.ServiceModel.Description;
#endregion 
namespace Ximura.WCF
{
    public class Client
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press a key to connect...");
            Console.ReadLine();
            Console.WriteLine("Connecting...");

            EndpointAddress address = new EndpointAddress("https://localhost:8089/WCF");
            IDoSomethingSimple mProxy = null;
            ChannelFactory<IDoSomethingSimple> mFactory = null;
            try
            {
                //ClientCredentials clientCredentials = new ClientCredentials();
                //clientCredentials.UserName.UserName = "sonua";
                //clientCredentials.UserName.Password = "password";
                //BindingParameterCollection bindingParms = new BindingParameterCollection();
                //bindingParms.Add(clientCredentials);

                // mFactory = ConnectionBindingUserNamePassword().BuildChannelFactory<IDoSomethingSimple>(bindingParms);

                //mFactory.Open();
                //mFactory.CreateChannel(address);
                mFactory = new ChannelFactory<IDoSomethingSimple>(ConnectionBindingUserNamePassword(), address);
                mFactory.Credentials.UserName.UserName = "fredo";
                mFactory.Credentials.UserName.Password = "fredo2";

                //HttpDigestClientCredential gg =  mFactory.Credentials.HttpDigest;


                mProxy = mFactory.CreateChannel();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            Console.WriteLine("Connected");
            try
            {
                for (string item = "Starting ..."; item != "END"; item = Console.ReadLine())
                {
                    mProxy.PassThisThrough(item);
                }

                Console.WriteLine("----------------------------------------");
                Console.WriteLine("---------------   ENDED   --------------");


                if (mFactory.State != System.ServiceModel.CommunicationState.Faulted)
                {
                    mFactory.Close();
                }
            }
            catch (TimeoutException)
            {
                // Handle the timeout exception.
                mFactory.Abort();
            }
            catch (CommunicationException)
            {
                // Handle the communication exception.
                mFactory.Abort();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception: " + ex.ToString());
                mFactory.Abort();
            }

        }


        #region ConnectionBindingWindowsSecurity()
        /// <summary>
        /// This property returns the binding for the connection.
        /// </summary>
        /// <returns>The binding.</returns>
        static Binding ConnectionBindingWindowsSecurity()
        {
            WSHttpBinding binding = new WSHttpBinding();

            binding.ReliableSession.Enabled = true;
            binding.Security.Mode = SecurityMode.Message;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            binding.MaxReceivedMessageSize = 255000;
            binding.TransactionFlow = true;

            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;

            return binding;
        }
        #endregion // ConnectionBindingWindowsSecurity()

        #region ConnectionBindingUserNamePassword()
        /// <summary>
        /// This property returns the binding for the connection.
        /// </summary>
        /// <returns>The binding.</returns>
        static Binding ConnectionBindingUserNamePassword()
        {
            WSHttpBinding binding = new WSHttpBinding();

            binding.ReliableSession.Enabled = true;

            //Security is provided at the SOAP message level.
            binding.Security.Mode = SecurityMode.TransportWithMessageCredential;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;

            binding.MaxReceivedMessageSize = 255000;
            binding.TransactionFlow = true;

            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            


            return binding;
        }
        #endregion // ConnectionBindingWindowsSecurity()


    }
}
