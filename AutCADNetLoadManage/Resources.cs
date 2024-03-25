using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace AutoCADNetLoadManager
{
	[CompilerGenerated]
	[DebuggerNonUserCode]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	internal class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(resourceMan, null))
				{
					ResourceManager resourceManager = resourceMan = new ResourceManager("AddInManager.Properties.Resources", typeof(Resources).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static string AppName => ResourceManager.GetString("AppName", resourceCulture);

		internal static string ClassNotExist => ResourceManager.GetString("ClassNotExist", resourceCulture);

		internal static string DefaultDescription => ResourceManager.GetString("DefaultDescription", resourceCulture);

		internal static string DependencyNotExist => ResourceManager.GetString("DependencyNotExist", resourceCulture);

		internal static string FileNotExist => ResourceManager.GetString("FileNotExist", resourceCulture);

		internal static string HelpFileWithExt => ResourceManager.GetString("HelpFileWithExt", resourceCulture);

		internal static string LoadAnotherFile => ResourceManager.GetString("LoadAnotherFile", resourceCulture);

		internal static string LoadCancelled => ResourceManager.GetString("LoadCancelled", resourceCulture);

		internal static string LoadFailed => ResourceManager.GetString("LoadFailed", resourceCulture);

		internal static string LoadFileFilter => ResourceManager.GetString("LoadFileFilter", resourceCulture);

		internal static string LoadSucceed => ResourceManager.GetString("LoadSucceed", resourceCulture);

		internal static string NoCHMFile => ResourceManager.GetString("NoCHMFile", resourceCulture);

		internal static string NoIniFile => ResourceManager.GetString("NoIniFile", resourceCulture);

		internal static string NoItemsSelected => ResourceManager.GetString("NoItemsSelected", resourceCulture);

		internal static string NotValidAddin => ResourceManager.GetString("NotValidAddin", resourceCulture);

		internal static string RemoteFile => ResourceManager.GetString("RemoteFile", resourceCulture);

		internal static string RuinedFile => ResourceManager.GetString("RuinedFile", resourceCulture);

		internal static string RunFailed => ResourceManager.GetString("RunFailed", resourceCulture);

		internal static string SameFile => ResourceManager.GetString("SameFile", resourceCulture);

		internal static string SameFileLoaded => ResourceManager.GetString("SameFileLoaded", resourceCulture);

		internal static string SaveClicked => ResourceManager.GetString("SaveClicked", resourceCulture);

		internal static string SaveIniError => ResourceManager.GetString("SaveIniError", resourceCulture);

		internal static string UnloadDualInterface => ResourceManager.GetString("UnloadDualInterface", resourceCulture);

		internal static string VersionTooOld => ResourceManager.GetString("VersionTooOld", resourceCulture);

		internal Resources()
		{
		}
	}
}
