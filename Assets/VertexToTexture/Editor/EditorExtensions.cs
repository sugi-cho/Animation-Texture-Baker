using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorExtensions
{
	[MenuItem ("Assets/Select All/Materials")]
	public static void SelectAllMaterial ()
	{
		Selection.objects = Selection.GetFiltered (typeof(Material), SelectionMode.DeepAssets);
	}

	[MenuItem ("Assets/Select All/Meshes")]
	public static void SelectAllMesh ()
	{
		Selection.objects = Selection.GetFiltered (typeof(Mesh), SelectionMode.DeepAssets);
	}

	[MenuItem ("Assets/Select All/Textures")]
	public static void SelectAllTexture ()
	{
		Selection.objects = Selection.GetFiltered (typeof(Texture), SelectionMode.DeepAssets);
	}

	[MenuItem ("Assets/Select All/Shaders")]
	public static void SelectAllShader ()
	{
		Selection.objects = Selection.GetFiltered (typeof(Shader), SelectionMode.DeepAssets);
	}

	[MenuItem ("Assets/Copy and Create New.asset")]
	public static void CopyObject ()
	{
		var o = (Object)Selection.activeObject;
		if (o == null)
			return;
		var newObj = MovieTexture.Instantiate<Object> (o);
		var path = AssetDatabase.GetAssetPath (o);
		path = System.IO.Path.GetDirectoryName (path);
		AssetDatabase.CreateAsset (newObj, path + '/' + o.name + ".asset");
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();

		Selection.activeObject = newObj;
	}

}
