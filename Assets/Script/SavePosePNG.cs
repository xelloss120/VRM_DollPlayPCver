using System;
using System.IO;
using UnityEngine;
using TriLibCore.SFB;
using VRM;

public class SavePosePNG : MonoBehaviour
{
    [SerializeField] SelectVRM SelectVRM;
    [SerializeField] Camera Camera;

    public void Save()
    {
        if (!SelectVRM.IsActive || SelectVRM.Marker == null)
        {
            return;
        }

        var item = StandaloneFileBrowser.SaveFilePanel("Save Pose PNG", "", "pose", "png");
        if (string.IsNullOrEmpty(item.Name))
        {
            return;
        }
        var ext = Path.GetExtension(item.Name);
        if (ext != ".png")
        {
            // 拡張子が無い場合のみ拡張子を付け足す
            item.Name += ".png";
        }

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
        tex = GetResized(tex, 512, 512);
        color = tex.GetPixels();

        // Humanoid状態で保存したポーズ画像をFullModeで読み込む際の不具合対策
        for (int i = 0; i < 512 * 5; i++)
        {
            color[i] = new Color(0, 0, 0, 0);
        }

        color[0].r = (float)'p' / byte.MaxValue;
        color[0].g = (float)'o' / byte.MaxValue;
        color[0].b = (float)'s' / byte.MaxValue;
        color[0].a = (float)'e' / byte.MaxValue;

        var index = 0;
        for (int i = 0; i < SelectVRM.Marker.List.Count; i++)
        {
            if (!SelectVRM.IsFullBone)
            {
                var link = SelectVRM.Marker.List[i].GetComponent<LinkRotation>();
                if (link.IsPrimal)
                {
                    SetColor(color, index);
                    index++;
                }
            }
            else
            {
                SetColor(color, i);
            }
        }

        // 指
        index = 512 * 1;
        var start = (int)HumanBodyBones.LeftThumbProximal;
        var end = (int)HumanBodyBones.RightLittleDistal;
        for (var i = start; i <= end; i++)
        {
            SetColor(color, index + i * 3, (HumanBodyBones)i);
        }

        // ブレンドシェイプ
        index = 512 * 2;
        color[index + 0] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Neutral)));
        color[index + 1] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.A)));
        color[index + 2] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.I)));
        color[index + 3] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.U)));
        color[index + 4] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.E)));
        color[index + 5] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.O)));
        color[index + 6] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink)));
        color[index + 7] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Joy)));
        color[index + 8] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Angry)));
        color[index + 9] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow)));
        color[index + 10] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Fun)));
        color[index + 11] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookUp)));
        color[index + 12] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookDown)));
        color[index + 13] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookLeft)));
        color[index + 14] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookRight)));
        color[index + 15] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink_L)));
        color[index + 16] = GetColor(SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink_R)));
        var ii = 17;
        var skinneds = SelectVRM.RootMarker.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var skinned in skinneds)
        {
            for (int i = 0; i < skinned.sharedMesh.blendShapeCount; i++)
            {
                color[index + ii] = GetColor(skinned.GetBlendShapeWeight(i));
                ii++;
            }
        }

        // VRIKターゲットの位置と回転を保存
        index = 512 * 5;
        var count = SelectVRM.Marker.List.Count;
        SetColor(color, index - 1, count - 10);
        SetColor(color, index - 4, count - 9);
        SetColor(color, index - 7, count - 8);
        SetColor(color, index - 10, count - 7);
        SetColor(color, index - 13, count - 6);
        SetColor(color, index - 16, count - 5);
        SetColor(color, index - 19, count - 10, false);
        SetColor(color, index - 22, count - 9, false);
        SetColor(color, index - 25, count - 8, false);
        SetColor(color, index - 28, count - 7, false);
        SetColor(color, index - 31, count - 6, false);
        SetColor(color, index - 34, count - 5, false);
        SetColor(color, index - 37, count - 4, false);
        SetColor(color, index - 40, count - 3, false);
        SetColor(color, index - 43, count - 2, false);
        SetColor(color, index - 46, count - 1, false);

        tex.SetPixels(color);
        tex.Apply();

        File.WriteAllBytes(item.Name, tex.EncodeToPNG());
    }

    Color GetColor(float value)
    {
        Color color;
        var bytes = BitConverter.GetBytes(value);
        color.r = (float)bytes[0] / byte.MaxValue;
        color.g = (float)bytes[1] / byte.MaxValue;
        color.b = (float)bytes[2] / byte.MaxValue;
        color.a = (float)bytes[3] / byte.MaxValue;
        return color;
    }

    void SetColor(Color[] color, int i)
    {
        var index = i * 3 + 1;
        var angle = SelectVRM.Marker.List[i].localEulerAngles;
        color[index + 0] = GetColor(angle.x);
        color[index + 1] = GetColor(angle.y);
        color[index + 2] = GetColor(angle.z);
    }

    void SetColor(Color[] color, int index, int count, bool pos = true)
    {
        Vector3 vector3;
        if (pos)
        {
            vector3 = SelectVRM.Marker.List[count].localPosition;
        }
        else
        {
            vector3 = SelectVRM.Marker.List[count].localEulerAngles;
        }
        color[index - 0] = GetColor(vector3.x);
        color[index - 1] = GetColor(vector3.y);
        color[index - 2] = GetColor(vector3.z);
    }

    void SetColor(Color[] color, int index, HumanBodyBones bones)
    {
        var angle = SelectVRM.Animator.GetBoneTransform(bones).localEulerAngles;
        color[index + 0] = GetColor(angle.x);
        color[index + 1] = GetColor(angle.y);
        color[index + 2] = GetColor(angle.z);
    }

    /// <summary>
    /// https://light11.hatenadiary.com/entry/2018/04/19/194015
    /// </summary>
    Texture2D GetResized(Texture2D texture, int width, int height)
    {
        // リサイズ後のサイズを持つRenderTextureを作成して書き込む
        var rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(texture, rt);

        // リサイズ後のサイズを持つTexture2Dを作成してRenderTextureから書き込む
        var preRT = RenderTexture.active;
        RenderTexture.active = rt;
        var ret = new Texture2D(width, height);
        ret.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        ret.Apply();
        RenderTexture.active = preRT;

        RenderTexture.ReleaseTemporary(rt);
        return ret;
    }
}
