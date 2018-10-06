using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class AssetBundleMenu
{
	[MenuItem("AssetBundles/Clear Cache")]
	static void ClearCache()
	{
		Caching.ClearCache ();
	}
	
	[MenuItem("AssetBundles/Build for PC")]
	static void TogglePCBuild ()
	{
		EditorPrefs.SetBool("buildPC", !EditorPrefs.GetBool("buildPC", false));
	}
	[MenuItem("AssetBundles/Build for PC", true)]
	static bool TogglePCBuildValidate ()
	{
		Menu.SetChecked("AssetBundles/Build for PC", EditorPrefs.GetBool("buildPC", false));
		return true;
	}
	
	[MenuItem("AssetBundles/Build for OSX")]
	static void ToggleOSXBuild ()
	{
		EditorPrefs.SetBool("buildOSX", !EditorPrefs.GetBool("buildOSX", false));
	}
	[MenuItem("AssetBundles/Build for OSX", true)]
	static bool ToggleOSXBuildValidate ()
	{
		Menu.SetChecked("AssetBundles/Build for OSX", EditorPrefs.GetBool("buildOSX", false));
		return true;
	}
	
	[MenuItem("AssetBundles/Build for iOS")]
	static void ToggleiOSBuild ()
	{
		EditorPrefs.SetBool("buildiOS", !EditorPrefs.GetBool("buildiOS", false));
	}
	[MenuItem("AssetBundles/Build for iOS", true)]
	static bool ToggleiOSBuildValidate ()
	{
		Menu.SetChecked("AssetBundles/Build for iOS", EditorPrefs.GetBool("buildiOS", false));
		return true;
	}
	
	[MenuItem("AssetBundles/Build for Android")]
	static void ToggleAndroidBuild ()
	{
		EditorPrefs.SetBool("buildAndroid", !EditorPrefs.GetBool("buildAndroid", false));
	}
	[MenuItem("AssetBundles/Build for Android", true)]
	static bool ToggleAndroidBuildValidate ()
	{
		Menu.SetChecked("AssetBundles/Build for Android", EditorPrefs.GetBool("buildAndroid", false));
		return true;
	}
	
	[MenuItem("AssetBundles/Build Asset Bundles")]
	static void BuildAssetBundles() 
	{
		// Bring up save panel
		string path = EditorUtility.SaveFolderPanel ("Save Bundle", "", "");
		if (path.Length != 0) 
		{   
			if(EditorPrefs.GetBool("buildPC", false)) 
				BuildBundle(path + "/PC", BuildTarget.StandaloneWindows);
			
			if(EditorPrefs.GetBool("buildOSX", false))
				BuildBundle(path + "/OSX", BuildTarget.StandaloneOSX);
			
			if(EditorPrefs.GetBool("buildiOS", false))
				BuildBundle(path + "/iOS", BuildTarget.iOS);
			
			if(EditorPrefs.GetBool("buildAndroid", false))
				BuildBundle(path + "/Android", BuildTarget.Android);          
		}
	}
	
	static void BuildBundle(string path, BuildTarget target)
	{
		if (!Directory.Exists (path))
			Directory.CreateDirectory (path);
		
		BuildPipeline.BuildAssetBundles (path, BuildAssetBundleOptions.UncompressedAssetBundle, target);
	}
}