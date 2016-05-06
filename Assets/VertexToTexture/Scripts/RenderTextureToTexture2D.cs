using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RenderTextureToTexture2D : MonoBehaviour
{

    public static Texture2D Convert(RenderTexture rt)
    {
        TextureFormat format;

        switch (rt.format)
        {
            case RenderTextureFormat.ARGBFloat:
                format = TextureFormat.RGBAFloat;
                break;
            case RenderTextureFormat.ARGBHalf:
                format = TextureFormat.RGBAHalf;
                break;
            case RenderTextureFormat.ARGBInt:
                format = TextureFormat.RGBA32;
                break;
            case RenderTextureFormat.ARGB32:
                format = TextureFormat.ARGB32;
                break;
            default:
                format = TextureFormat.ARGB32;
                Debug.LogWarning("Unsuported RenderTextureFormat.");
                break;
        }

        return Convert(rt, format);
    }

    public static Texture2D Convert(RenderTexture rt, TextureFormat format)
    {
        var tex2d = new Texture2D(rt.width, rt.height, format, false);
        var rect = Rect.MinMaxRect(0f, 0f, tex2d.width, tex2d.height);
        RenderTexture.active = rt;
        tex2d.ReadPixels(rect, 0, 0);
        RenderTexture.active = null;
        return tex2d;
    }


#if UNITY_EDITOR
    [MenuItem("Assets/Convert/RenderTexture -> Texture2D")]
    public static void ConvertSelected()
    {
        var rt = (RenderTexture)Selection.activeObject;
        if (rt == null)
        {
            Debug.LogError("Invalide Object Selected");
            return;
        }
        var tex2d = Convert(rt);
        AssetDatabase.CreateAsset(tex2d, "Assets/" + rt.name + "_tex2d.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = tex2d;
    }
#endif
}
