using UnityEngine;
using RootMotion.FinalIK;

public class LinkSwivelOffset : MonoBehaviour
{
    public VRIK VRIK;
    public enum Regio { Knee, Elbow }
    public Regio KE;
    public enum Side { L, R }
    public Side LR;

    void Update()
    {
        if (VRIK != null)
        {
            if (KE == Regio.Knee && LR == Side.L)
            {
                VRIK.solver.leftLeg.swivelOffset = transform.localEulerAngles.y;
            }
            if (KE == Regio.Knee && LR == Side.R)
            {
                VRIK.solver.rightLeg.swivelOffset = transform.localEulerAngles.y;
            }
            if (KE == Regio.Elbow && LR == Side.L)
            {
                VRIK.solver.leftArm.swivelOffset = transform.localEulerAngles.x;
            }
            if (KE == Regio.Elbow && LR == Side.R)
            {
                VRIK.solver.rightArm.swivelOffset = transform.localEulerAngles.x;
            }
        }
        transform.localPosition = Vector3.zero;
    }
}
