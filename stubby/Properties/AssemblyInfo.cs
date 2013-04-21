using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("stubby4net")]
[assembly: AssemblyDescription("A small server for stubbing external systems during development.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Eric Mrak")]
[assembly: AssemblyProduct("stubby4net")]
[assembly: AssemblyCopyright("Copyright ©  2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("afa901aa-9750-470b-9d6f-a03ed5b5ae92")]

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
[assembly: AssemblyVersion("1.0.0.2")]
[assembly: AssemblyFileVersion("1.0.0.2")]


// Make other asseblies "friendly" (able to access internal classes of this assembly)
[assembly: InternalsVisibleTo("unit")]