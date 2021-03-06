﻿#region Copyright
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
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Resources;

#if (WIN)
[assembly: ComVisible(false)]
#endif

[assembly: AssemblyTitle("Ximura")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Ximura")]
[assembly: AssemblyProduct("Ximura Framework")]
[assembly: AssemblyCopyright("\x00a9 Paul Stancer 2000-2011")]
[assembly: AssemblyTrademark("")]

[assembly: AssemblyVersion("4.0.0.*")]

[assembly: CLSCompliant(true)]

[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile(@"../../../Keys/Ximura.snk")]

[assembly: NeutralResourcesLanguage("en-US")]

#if (WindowsCE)
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2232:MarkWindowsFormsEntryPointsWithStaThread")]
#endif
