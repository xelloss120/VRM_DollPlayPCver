using System;
using System.IO;
using UnityEngine;
using TriLibCore.SFB;
using Battlehub.RTHandles;

public class SaveScene : MonoBehaviour
{
    [SerializeField] GameObject Camera;
    [SerializeField] RuntimeSceneComponent RuntimeSceneComponent;

    public void Save()
    {
        var saveItem = StandaloneFileBrowser.SaveFilePanel("Save Scene TXT", "", "scene", "txt");
        if (string.IsNullOrEmpty(saveItem.Name))
        {
            return;
        }
        var ext = Path.GetExtension(saveItem.Name);
        if (ext != ".txt")
        {
            // 拡張子が無い場合のみ拡張子を付け足す
            saveItem.Name += ".txt";
        }

        var csv = "";
        var objects = Array.FindAll(FindObjectsOfType<GameObject>(), (item) => item.transform.parent == null);
        foreach (var obj in objects)
        {
            // rootにあるオブジェクトに対してシーン保存対象かSaveSceneTargetの有無で判定
            var saveSceneTarget = obj.GetComponent<SaveSceneTarget>();
            if (saveSceneTarget != null)
            {
                var line = saveSceneTarget.Path;
                if (line == "Camera")
                {
                    line += GetTransform(RuntimeSceneComponent.CameraPosition);

                    // カメラは位置と回転を適用してもRTHandlesのPivotに視点が釣られるのでPivotの位置も必要
                    line += "," + RuntimeSceneComponent.Pivot.x + "," + RuntimeSceneComponent.Pivot.y + "," + RuntimeSceneComponent.Pivot.z;
                }
                else if (line == "Light" || line == "PointLight")
                {
                    var light = line == "Light" ? obj.GetComponent<LinkLight>().Light : obj.GetComponent<Light>();
                    line += GetTransform(obj.transform);
                    line += "," + light.intensity + "," + light.color.r + "," + light.color.g + "," + light.color.b + "," + light.range;
                }
                else
                {
                    line += GetTransform(obj.transform);
                }
                csv += line + "\n";
            }
        }

        File.WriteAllText(saveItem.Name, csv);
    }

    string GetTransform(Vector3 pos)
    {
        var ang = Vector3.zero;
        var sca = Vector3.zero;
        var str = GetTransform(pos, ang, sca);
        return str;
    }

    string GetTransform(Transform t)
    {
        var pos = t.position;
        var ang = t.eulerAngles;
        var sca = t.localScale;
        var str = GetTransform(pos, ang, sca);
        return str;
    }

    string GetTransform(Vector3 pos, Vector3 ang, Vector3 sca)
    {
        var str = "," + pos.x + "," + pos.y + "," + pos.z + "," + ang.x + "," + ang.y + "," + ang.z + "," + sca.x + "," + sca.y + "," + sca.z;
        return str;
    }
}
