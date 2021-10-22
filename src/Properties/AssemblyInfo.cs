﻿using System.Reflection;

#if __MACOS__ || __TVOS__ || __WATCHOS__ || __IOS__

using Foundation;
[assembly: LinkerSafe]
#endif

// Information about this assembly is defined by the following attributes. 
// Change them to the values specific to your project.

#if NETFRAMEWORK
[assembly: AssemblyTitle ("ChipmunkBinding (.NET Framework 4.6)")]
#elif WINDOWS_UWP
[assembly: AssemblyTitle ("ChipmunkBinding (Windows Universal)")]
#elif __ANDROID__
[assembly: AssemblyTitle ("ChipmunkBinding (Xamarin.Android)")]
#elif NETCOREAPP
[assembly: AssemblyTitle("ChipmunkBinding (.NET Core)")]
#elif NETSTANDARD
[assembly: AssemblyTitle ("ChipmunkBinding (.NET Standard)")]
#elif __TVOS__
[assembly: AssemblyTitle ("ChipmunkBinding (Xamarin.tvOS)")]
#elif __WATCHOS__
[assembly: AssemblyTitle ("ChipmunkBinding (Xamarin.watchOS)")]
#elif __IOS__
[assembly: AssemblyTitle ("ChipmunkBinding (Xamarin.iOS)")]
#elif __MACOS__
[assembly: AssemblyTitle ("ChipmunkBinding (Xamarin.Mac)")]
#else
[assembly: AssemblyTitle ("ChipmunkBinding (.NET Framework)")]
#endif

[assembly: AssemblyDescription("Binding library for native Chipmunk2D")]
[assembly: AssemblyCompany("Codefoco")]
[assembly: AssemblyProduct("ChipmunkBinding")]
[assembly: AssemblyCopyright("Copyright © Vinicius Jarina 2021")]
[assembly: AssemblyCulture("")]

// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.

[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyInformationalVersion("0.1.0+155.Branch.main.Sha.2bffe65059140b063c0ca22a830593629f64b2cb")]
[assembly: AssemblyFileVersion("0.1.0.0")]

