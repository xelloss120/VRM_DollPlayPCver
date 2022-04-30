using System.IO;
using UnityEngine;
using TriLibCore.SFB;
using VRM;

public class SavePoseTXT : MonoBehaviour
{
    [SerializeField] SelectVRM SelectVRM;

    public void Save()
    {
        if (!SelectVRM.IsActive || SelectVRM.Marker == null)
        {
            return;
        }

        var item = StandaloneFileBrowser.SaveFilePanel("Save Pose TXT", "", "pose", "txt");
        if (string.IsNullOrEmpty(item.Name))
        {
            return;
        }
        var ext = Path.GetExtension(item.Name);
        if (ext != ".txt")
        {
            // 拡張子が無い場合のみ拡張子を付け足す
            item.Name += ".txt";
        }

        var pose = "";
        for (int i = 0; i < SelectVRM.Marker.List.Count; i++)
        {
            if (!SelectVRM.IsFullBone)
            {
                var link = SelectVRM.Marker.List[i].GetComponent<LinkRotation>();
                if (link.IsPrimal)
                {
                    pose += GetAngleString(SelectVRM.Marker.List[i].localEulerAngles);
                }
            }
            else
            {
                pose += GetAngleString(SelectVRM.Marker.List[i].localEulerAngles);
            }
        }

        pose += "指" + "\n";
        var start = (int)HumanBodyBones.LeftThumbProximal;
        var end = (int)HumanBodyBones.RightLittleDistal;
        for (var i = start; i <= end; i++)
        {
            var bone = (HumanBodyBones)i;
            var angle = SelectVRM.Animator.GetBoneTransform(bone).localEulerAngles;
            pose += GetAngleString(angle);
        }

        pose += "ブレンドシェイプ" + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Neutral)) + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.A)) + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.I)) + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.U)) + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.E)) + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.O)) + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink)) + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Joy)) + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Angry)) + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow)) + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Fun)) + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookUp)) + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookDown)) + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookLeft)) + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.LookRight)) + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink_L)) + "\n";
        pose += SelectVRM.Proxy.GetValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink_R)) + "\n";
        var skinneds = SelectVRM.RootMarker.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var skinned in skinneds)
        {
            for (int i = 0; i < skinned.sharedMesh.blendShapeCount; i++)
            {
                pose += skinned.GetBlendShapeWeight(i) + "\n";
            }
        }

        pose += "IK" + "\n";
        var count = SelectVRM.Marker.List.Count;
        pose += GetAngleString(SelectVRM.Marker.List[count - 10].localPosition);
        pose += GetAngleString(SelectVRM.Marker.List[count - 9].localPosition);
        pose += GetAngleString(SelectVRM.Marker.List[count - 8].localPosition);
        pose += GetAngleString(SelectVRM.Marker.List[count - 7].localPosition);
        pose += GetAngleString(SelectVRM.Marker.List[count - 6].localPosition);
        pose += GetAngleString(SelectVRM.Marker.List[count - 5].localPosition);
        pose += GetAngleString(SelectVRM.Marker.List[count - 10].localEulerAngles);
        pose += GetAngleString(SelectVRM.Marker.List[count - 9].localEulerAngles);
        pose += GetAngleString(SelectVRM.Marker.List[count - 8].localEulerAngles);
        pose += GetAngleString(SelectVRM.Marker.List[count - 7].localEulerAngles);
        pose += GetAngleString(SelectVRM.Marker.List[count - 6].localEulerAngles);
        pose += GetAngleString(SelectVRM.Marker.List[count - 5].localEulerAngles);
        pose += GetAngleString(SelectVRM.Marker.List[count - 4].localEulerAngles);
        pose += GetAngleString(SelectVRM.Marker.List[count - 3].localEulerAngles);
        pose += GetAngleString(SelectVRM.Marker.List[count - 2].localEulerAngles);
        pose += GetAngleString(SelectVRM.Marker.List[count - 1].localEulerAngles);

        File.WriteAllText(item.Name, pose);
    }

    string GetAngleString(Vector3 angle)
    {
        var x = angle.x.ToString();
        var y = angle.y.ToString();
        var z = angle.z.ToString();
        var str = x + "," + y + "," + z + "\n";
        return str;
    }
}
