using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("MeasureIt.Core")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyProduct("MeasureIt")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("c92eb844-4058-4a0a-9821-2d164340516d")]

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
[assembly: AssemblyVersion("1.14.0.*")]
[assembly: AssemblyFileVersion("1.14.0.0")]

// NuGet NuSpec Token Replacement should work, but is not: $version$
// TODO: TBD: https://github.com/NuGet/Home/issues/4149

[assembly: InternalsVisibleTo("MeasureIt.Autofac")]
[assembly: InternalsVisibleTo("MeasureIt.Castle.Interception")]
[assembly: InternalsVisibleTo("MeasureIt.Castle.Windsor")]
[assembly: InternalsVisibleTo("MeasureIt.Web.Http.Autofac")]
[assembly: InternalsVisibleTo("MeasureIt.Web.Http.Castle.Windsor")]
[assembly: InternalsVisibleTo("MeasureIt.Web.Http.Core")]
[assembly: InternalsVisibleTo("MeasureIt.Web.Mvc.Autofac")]
[assembly: InternalsVisibleTo("MeasureIt.Web.Mvc.Castle.Windsor")]
[assembly: InternalsVisibleTo("MeasureIt.Web.Mvc.Core")]
[assembly: InternalsVisibleTo("MeasureIt.Core.Tests")]
