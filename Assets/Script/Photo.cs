using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TriLibCore.SFB;

public class Photo : MonoBehaviour
{
    [SerializeField] Camera Camera;
    [SerializeField] Toggle JPG;
    [SerializeField] Toggle PNG;
    [SerializeField] Toggle RGBA32;
    [SerializeField] Toggle RGBAFloat;
    [SerializeField] AudioSource SE;
    [SerializeField] LoadFile LoadFile;

    public void Get()
    {
        var rt = Camera.targetTexture;
        var tf = RGBA32.isOn ? TextureFormat.RGBA32 : TextureFormat.RGBAFloat;
        var tex = new Texture2D(rt.width, rt.height, tf, false);
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        var pixels = tex.GetPixels();
        for (int i = 0; i < pixels.Length; i++) pixels[i] = pixels[i].gamma;
        tex.SetPixels(pixels);
        tex.Apply();

        var dt = DateTime.Now;
        var name = dt.ToString("yyyyMMddHHmmss");

        if (JPG.isOn)
        {
            var path = GetPath(name, "jpg");
            if (path != "")
            {
                File.WriteAllBytes(path, tex.EncodeToJPG());
                SE.Play();
            }
        }
        if (PNG.isOn)
        {
            var path = GetPath(name, "png");
            if (path != "")
            {
                File.WriteAllBytes(path, tex.EncodeToPNG());
                SE.Play();
            }
        }
    }

    string GetPath(string name, string type)
    {
        var item = StandaloneFileBrowser.SaveFilePanel("Save Photo", "", name, type);
        if (string.IsNullOrEmpty(item.Name))
        {
            return "";
        }
        var ext = Path.GetExtension(item.Name);
        if (ext != "." + type)
        {
            // 拡張子が無い場合のみ拡張子を付け足す
            item.Name += "." + type;
        }
        return item.Name;
    }
}
