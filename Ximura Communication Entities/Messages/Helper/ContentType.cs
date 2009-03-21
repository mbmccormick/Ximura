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
﻿#region using
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;
using Ximura;
using Ximura.Helper;
using Ximura.Data;
using CH = Ximura.Helper.Common;
#endregion
namespace Ximura.Communication
{
    /// <summary>
    /// This structure processes the content type field and extracts the media type
    /// and the parameters.
    /// </summary>
    public class ContentType
    {
        public string MediaType = null;
        public Dictionary<string, string> Parameters = null;

        public ContentType(string data)
        {
            string[] items = data.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            MediaType = items[0].TrimStart().ToLowerInvariant();

            if (items.Length <= 1)
                return;

            Parameters = new Dictionary<string, string>(items.Length - 1);
            int count = 1;
            while (count < items.Length)
            {
                string item = items[count];
                int pos = item.IndexOf('=');
                if (pos == -1)
                    Parameters.Add(item.ToLowerInvariant().TrimStart(), null);
                else
                    Parameters.Add(
                        item.Substring(0, pos).ToLowerInvariant().TrimStart()
                        , item.Substring(pos + 1));

                count++;
            }
        }

        public string Parameter(string id)
        {
            if (Parameters == null || !Parameters.ContainsKey(id))
                return null;

            return Parameters[id];
        }
    }
}
