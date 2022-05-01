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

        pose += "Finger" + "\n";
        var start = (int)HumanBodyBones.LeftThumbProximal;
        var end = (int)HumanBodyBones.RightLittleDistal;
        for (var i = start; i <= end; i++)
        {
            var bone = (HumanBodyBones)i;
            var angle = SelectVRM.Animator.GetBoneTransform(bone).localEulerAngles;
            pose += GetAngleString(angle);
        }

        pose += "BlendShapeVRM" + "\n";
        var clips = SelectVRM.Proxy.BlendShapeAvatar.Clips;
        for (int i = 0; i < clips.Count; i++)
        {
            pose += SelectVRM.Proxy.GetValue(clips[i].Key) + "\n";
        }
        pose += "BlendShapeFull" + "\n";
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
