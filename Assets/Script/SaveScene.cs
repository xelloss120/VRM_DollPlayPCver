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
        var saveItem = StandaloneFileBrowser.SaveFilePanel("Save Scene TXT", "", "scene.txt", "txt");
        if (string.IsNullOrEmpty(saveItem.Name))
        {
            return;
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
                    var pos = RuntimeSceneComponent.CameraPosition;
                    var ang = Vector3.zero;
                    var sca = Vector3.zero;
                    line += "," + pos.x + "," + pos.y + "," + pos.z + "," + ang.x + "," + ang.y + "," + ang.z + "," + sca.x + "," + sca.y + "," + sca.z;

                    // カメラは位置と回転を適用してもRTHandlesのPivotに視点が釣られるのでPivotの位置も必要
                    line += "," + RuntimeSceneComponent.Pivot.x + "," + RuntimeSceneComponent.Pivot.y + "," + RuntimeSceneComponent.Pivot.z;
                }
                else
                {
                    var pos = obj.transform.position;
                    var ang = obj.transform.eulerAngles;
                    var sca = obj.transform.localScale;
                    line += "," + pos.x + "," + pos.y + "," + pos.z + "," + ang.x + "," + ang.y + "," + ang.z + "," + sca.x + "," + sca.y + "," + sca.z;
                }
                csv += line + "\n";
            }
        }

        File.WriteAllText(saveItem.Name, csv);
    }
}
