﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Hudl.Mjolnir.Tests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Hudl.Mjolnir.Tests")]
[assembly: AssemblyCopyright("Copyright ©  2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

// Disabled for now. A number of tests set static configuration state that,
// when leaked between parallel tests, makes them quite sad. If we can find
// a way to encapsulate that state, or perhaps just group those tests into
// a non-parallelized group, we should revisit this.
[assembly: CollectionBehavior(DisableTestParallelization = true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("070914b7-5f05-4d47-a275-98bb15754727")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
