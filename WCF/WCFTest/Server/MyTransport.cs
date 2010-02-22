using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;

namespace Ximura.WCF
{
    class MyTransport: TransportBindingElement
    {
        public override string Scheme
        {
            get { throw new NotImplementedException(); }
        }

        public override BindingElement Clone()
        {
            throw new NotImplementedException();
        }
    }
}
