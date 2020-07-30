using UnityEngine;

public class Reverse : MonoBehaviour
{
    [SerializeField] SelectVRM SelectVRM;

    public void Set()
    {
        if (!SelectVRM.IsActive || SelectVRM.Marker == null)
        {
            return;
        }

        // 以下はLoadFileのStartでやってるPrimalBonesに対するAdd順を前提とした処理

        SetCenter(SelectVRM.Marker.List[0]);
        SetCenter(SelectVRM.Marker.List[1]);
        SetCenter(SelectVRM.Marker.List[2]);
        SetCenter(SelectVRM.Marker.List[3]);
        SetCenter(SelectVRM.Marker.List[4]);
        SetCenter(SelectVRM.Marker.List[5]);

        var LeftUpperArm = SelectVRM.Marker.List[6].localEulerAngles;
        var LeftLowerArm = SelectVRM.Marker.List[7].localEulerAngles;
        var LeftHand = SelectVRM.Marker.List[8].localEulerAngles;

        SetLR_Ang(SelectVRM.Marker.List[6], SelectVRM.Marker.List[9].localEulerAngles); // LeftUpperArm
        SetLR_Ang(SelectVRM.Marker.List[7], SelectVRM.Marker.List[10].localEulerAngles); // LeftLowerArm
        SetLR_Ang(SelectVRM.Marker.List[8], SelectVRM.Marker.List[11].localEulerAngles); // LeftHand

        SetLR_Ang(SelectVRM.Marker.List[9], LeftUpperArm); // RightUpperArm
        SetLR_Ang(SelectVRM.Marker.List[10], LeftLowerArm); // RightLowerArm
        SetLR_Ang(SelectVRM.Marker.List[11], LeftHand); // RightHand

        var LeftUpperLeg = SelectVRM.Marker.List[12].localEulerAngles;
        var LeftLowerLeg = SelectVRM.Marker.List[13].localEulerAngles;
        var LeftFoot = SelectVRM.Marker.List[14].localEulerAngles;

        SetLR_Ang(SelectVRM.Marker.List[12], SelectVRM.Marker.List[15].localEulerAngles); // LeftUpperLeg
        SetLR_Ang(SelectVRM.Marker.List[13], SelectVRM.Marker.List[16].localEulerAngles); // LeftLowerLeg
        SetLR_Ang(SelectVRM.Marker.List[14], SelectVRM.Marker.List[17].localEulerAngles); // LeftFoot

        SetLR_Ang(SelectVRM.Marker.List[15], LeftUpperLeg); // RightUpperLeg
        SetLR_Ang(SelectVRM.Marker.List[16], LeftLowerLeg); // RightLowerLeg
        SetLR_Ang(SelectVRM.Marker.List[17], LeftFoot); // RightFoot

        // 以下はVRIK向け
        var count = SelectVRM.Marker.List.Count;

        SetCenter(SelectVRM.Marker.List[count - 8]);
        SetCenter(SelectVRM.Marker.List[count - 7]);

        var vrikLeftArmPos = SelectVRM.Marker.List[count - 6].localPosition;
        var vrikLeftArmAng = SelectVRM.Marker.List[count - 6].localEulerAngles;
        SetLR_Pos(SelectVRM.Marker.List[count - 6], SelectVRM.Marker.List[count - 5].localPosition);
        SetLR_Ang(SelectVRM.Marker.List[count - 6], SelectVRM.Marker.List[count - 5].localEulerAngles);
        SetLR_Pos(SelectVRM.Marker.List[count - 5], vrikLeftArmPos);
        SetLR_Ang(SelectVRM.Marker.List[count - 5], vrikLeftArmAng);

        var vrikRighttArmPos = SelectVRM.Marker.List[count - 4].localPosition;
        var vrikRightArmAng = SelectVRM.Marker.List[count - 4].localEulerAngles;
        SetLR_Pos(SelectVRM.Marker.List[count - 4], SelectVRM.Marker.List[count - 3].localPosition);
        SetLR_Ang(SelectVRM.Marker.List[count - 4], SelectVRM.Marker.List[count - 3].localEulerAngles);
        SetLR_Pos(SelectVRM.Marker.List[count - 3], vrikRighttArmPos);
        SetLR_Ang(SelectVRM.Marker.List[count - 3], vrikRightArmAng);
    }

    void SetCenter(Transform t)
    {
        var ang = t.localEulerAngles;
        ang.y *= -1;
        ang.z *= -1;
        t.localEulerAngles = ang;
    }

    void SetLR_Ang(Transform t, Vector3 ang)
    {
        ang.y *= -1;
        ang.z *= -1;
        t.localEulerAngles = ang;
    }

    void SetLR_Pos(Transform t, Vector3 pos)
    {
        pos.x *= -1;
        pos.z *= -1;
        t.localPosition = pos;
    }
}
