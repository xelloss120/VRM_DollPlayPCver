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
    [SerializeField] AudioSource SE;
    [SerializeField] LoadFile LoadFile;

    public void Get()
    {
        RenderTexture RenTex = Camera.targetTexture;
        RenderTexture.active = RenTex;
        Texture2D tex = new Texture2D(RenTex.width, RenTex.height, TextureFormat.RGBAFloat, false);
        tex.ReadPixels(new Rect(0, 0, RenTex.width, RenTex.height), 0, 0);
        var color = tex.GetPixels();
        for (int i = 0; i < color.Length; i++)
        {
            color[i].r = Mathf.LinearToGammaSpace(color[i].r);
            color[i].g = Mathf.LinearToGammaSpace(color[i].g);
            color[i].b = Mathf.LinearToGammaSpace(color[i].b);
        }
        tex.SetPixels(color);
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
