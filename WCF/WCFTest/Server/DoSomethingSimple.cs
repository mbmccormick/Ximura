using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IdentityModel.Claims;

namespace Ximura.WCF
{
    [ServiceBehaviorAttribute(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.PerSession,
        AutomaticSessionShutdown = false)]
    public class DoSomethingSimple : IDoSomethingSimple
    {
        public DoSomethingSimple()
        {
            // Write the primary identity and Windows identity. The primary identity is derived from the
            // the credentials used to authenticate the user. The Windows identity may be a null string.
            Console.WriteLine("PrimaryIdentity: {0}", ServiceSecurityContext.Current.PrimaryIdentity.Name);
            Console.WriteLine("WindowsIdentity: {0}", ServiceSecurityContext.Current.WindowsIdentity.Name);
            Console.WriteLine();
            // Write the claimsets in the authorization context. By default, there is only one claimset
            // provided by the system. 
            foreach (ClaimSet claimset in ServiceSecurityContext.Current.AuthorizationContext.ClaimSets)
            {
                foreach (Claim claim in claimset)
                {
                    // Write out each claim type, claim value, and the right. There are two
                    // possible values for the right: "identity" and "possessproperty". 
                    Console.WriteLine("Claim Type = {0}", claim.ClaimType);
                    Console.WriteLine("\t Resource = {0}", claim.Resource.ToString());
                    Console.WriteLine("\t Right = {0}", claim.Right);
                }
            }
        }
        #region IDoSomethingSimple Members

        public void PassThisThrough(string item)
        {
            Console.WriteLine("Client says: " + item);
        }

        #endregion
    }
}
