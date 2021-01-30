using System;
using System.IO;
using UnityEngine;
using SFB;

public class SaveScene : MonoBehaviour
{
    [SerializeField] GameObject Camera;

    public void Save()
    {
        var savePath = StandaloneFileBrowser.SaveFilePanel("Save Scene TXT", "", "scene", "txt");
        if (string.IsNullOrEmpty(savePath))
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
                var path = saveSceneTarget.Path;
                var pos = obj.transform.position;
                var ang = obj.transform.eulerAngles;
                var sca = obj.transform.localScale;
                var line = path + "," + pos.x + "," + pos.y + "," + pos.z + "," + ang.x + "," + ang.y + "," + ang.z + "," + sca.x + "," + sca.y + "," + sca.z;
                if (path == "Camera")
                {
                    // カメラだけは位置と回転を適用しても操作すると実行時の前回位置に釣られるのでこちらも必要
                    var orbit = Camera.GetComponent<Battlehub.RTCommon.MouseOrbit>();
                    line += "," + orbit.Target.position.x + "," + orbit.Target.position.y + "," + orbit.Target.position.z;
                }
                csv += line + "\n";
            }
        }

        File.WriteAllText(savePath, csv);
    }
}
