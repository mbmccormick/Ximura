#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
#endregion // using
namespace Ximura.WCF
{
    [ServiceContract(Namespace = "http://wcf.aegea.biz")]
    public interface IDoSomethingSimple
    {
        [OperationContract]
        void PassThisThrough(string id);
    }
}
