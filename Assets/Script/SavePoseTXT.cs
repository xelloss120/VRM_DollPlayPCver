using System.IO;
using UnityEngine;
using TriLibCore.SFB;

public class SavePoseTXT : MonoBehaviour
{
    [SerializeField] SelectVRM SelectVRM;

    public void Save()
    {
        if (!SelectVRM.IsActive || SelectVRM.Marker == null)
        {
            return;
        }

        var item = StandaloneFileBrowser.SaveFilePanel("Save Pose TXT", "", "pose.txt", "txt");
        if (string.IsNullOrEmpty(item.Name))
        {
            return;
        }

        var pose = "";
        foreach (Transform t in SelectVRM.Marker.List)
        {
            if (!SelectVRM.IsFullBone)
            {
                var link = t.GetComponent<LinkRotation>();
                if (link.IsPrimal) pose += GetAngleString(t.localEulerAngles);
            }
            else
            {
                pose += GetAngleString(t.localEulerAngles);
            }
        }

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
