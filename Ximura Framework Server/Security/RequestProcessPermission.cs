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
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security.Principal;
#endregion // using
namespace Ximura.Framework
{
    [Serializable()]
    public class RequestProcessPermission: CodeAccessPermission
    {
        public override IPermission Copy()
        {
            throw new NotImplementedException();
        }

        public override void FromXml(SecurityElement elem)
        {
            throw new NotImplementedException();
        }

        public override IPermission Intersect(IPermission target)
        {
            throw new NotImplementedException();
        }

        public override bool IsSubsetOf(IPermission target)
        {
            throw new NotImplementedException();
        }

        public override SecurityElement ToXml()
        {
            throw new NotImplementedException();
        }
    }
}
