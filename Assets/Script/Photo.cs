using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

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
        Texture2D tex = new Texture2D(RenTex.width, RenTex.height);
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
            string path = "Photo\\" + name + ".jpg";
            File.WriteAllBytes(path, tex.EncodeToJPG());
            //LoadFile.IMG(path);
        }
        if (PNG.isOn)
        {
            string path = "Photo\\" + name + ".png";
            File.WriteAllBytes(path, tex.EncodeToPNG());
            //LoadFile.IMG(path);
        }

        SE.Play();
    }
}
