using UnityEngine;
using System.Collections;
using UnityEditor;

public class MakeScriptableObject
{
	// [MenuItem("Assets/Create/ColumnGroup")]
	public static void CreateMyAsset()
	{
		ColumnGroup asset = ScriptableObject.CreateInstance<ColumnGroup>();

		AssetDatabase.CreateAsset(asset, "Assets/NewScripableObject.asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();

		Selection.activeObject = asset;
	}
}