using UnityEngine;
using UnityEngine.UI;

public class LinkHand : MonoBehaviour
{
    [SerializeField] Text TextL;
    [SerializeField] Text TextR;
    [SerializeField] Slider SliderL;
    [SerializeField] Slider SliderR;
    [SerializeField] SelectVRM SelectVRM;

    Animator Anim = null;

    struct Finger
    {
        public Transform Proximal;
        public Transform Intermediate;
        public Transform Distal;
    }

    struct Hand
    {
        public Finger Thumb;
        public Finger Index;
        public Finger Middle;
        public Finger Ring;
        public Finger Little;
    }

    Hand HandL;
    Hand HandR;

    Vector3 LeftFingerAngle = new Vector3(0, 0, 80);
    Vector3 LeftFingerAngleI = new Vector3(0, 0, -5);
    Vector3 LeftFingerAngleM = new Vector3(0, 0, -5);
    Vector3 LeftFingerAngleT = new Vector3(30, -30, 0);
    Vector3 LeftFingerAngleP = new Vector3(0, 10, 0);

    Vector3 RightFingerAngle = new Vector3(0, 0, -80);
    Vector3 RightFingerAngleI = new Vector3(0, 0, 5);
    Vector3 RightFingerAngleM = new Vector3(0, 0, 5);
    Vector3 RightFingerAngleT = new Vector3(30, 30, 0);
    Vector3 RightFingerAngleP = new Vector3(0, -10, 0);

    void Update()
    {
        if (!SelectVRM.IsActive)
        {
            Anim = null;
            return;
        }

        if (Anim != SelectVRM.Animator)
        {
            Anim = SelectVRM.Animator;

            HandL.Thumb.Proximal = Anim.GetBoneTransform(HumanBodyBones.LeftThumbProximal);
            HandL.Thumb.Intermediate = Anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
            HandL.Thumb.Distal = Anim.GetBoneTransform(HumanBodyBones.LeftThumbDistal);
            HandL.Index.Proximal = Anim.GetBoneTransform(HumanBodyBones.LeftIndexProximal);
            HandL.Index.Intermediate = Anim.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate);
            HandL.Index.Distal = Anim.GetBoneTransform(HumanBodyBones.LeftIndexDistal);
            HandL.Middle.Proximal = Anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
            HandL.Middle.Intermediate = Anim.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate);
            HandL.Middle.Distal = Anim.GetBoneTransform(HumanBodyBones.LeftMiddleDistal);
            HandL.Ring.Proximal = Anim.GetBoneTransform(HumanBodyBones.LeftRingProximal);
            HandL.Ring.Intermediate = Anim.GetBoneTransform(HumanBodyBones.LeftRingIntermediate);
            HandL.Ring.Distal = Anim.GetBoneTransform(HumanBodyBones.LeftRingDistal);
            HandL.Little.Proximal = Anim.GetBoneTransform(HumanBodyBones.LeftLittleProximal);
            HandL.Little.Intermediate = Anim.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate);
            HandL.Little.Distal = Anim.GetBoneTransform(HumanBodyBones.LeftLittleDistal);

            HandR.Thumb.Proximal = Anim.GetBoneTransform(HumanBodyBones.RightThumbProximal);
            HandR.Thumb.Intermediate = Anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
            HandR.Thumb.Distal = Anim.GetBoneTransform(HumanBodyBones.RightThumbDistal);
            HandR.Index.Proximal = Anim.GetBoneTransform(HumanBodyBones.RightIndexProximal);
            HandR.Index.Intermediate = Anim.GetBoneTransform(HumanBodyBones.RightIndexIntermediate);
            HandR.Index.Distal = Anim.GetBoneTransform(HumanBodyBones.RightIndexDistal);
            HandR.Middle.Proximal = Anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
            HandR.Middle.Intermediate = Anim.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate);
            HandR.Middle.Distal = Anim.GetBoneTransform(HumanBodyBones.RightMiddleDistal);
            HandR.Ring.Proximal = Anim.GetBoneTransform(HumanBodyBones.RightRingProximal);
            HandR.Ring.Intermediate = Anim.GetBoneTransform(HumanBodyBones.RightRingIntermediate);
            HandR.Ring.Distal = Anim.GetBoneTransform(HumanBodyBones.RightRingDistal);
            HandR.Little.Proximal = Anim.GetBoneTransform(HumanBodyBones.RightLittleProximal);
            HandR.Little.Intermediate = Anim.GetBoneTransform(HumanBodyBones.RightLittleIntermediate);
            HandR.Little.Distal = Anim.GetBoneTransform(HumanBodyBones.RightLittleDistal);

            var lz = HandL.Index.Proximal.localEulerAngles.z;
            var rz = HandR.Index.Proximal.localEulerAngles.z;
            lz = lz < 180 ? lz : lz - 360;
            rz = rz > 180 ? rz - 360 : rz;
            SliderL.value = lz > 0 ? lz / LeftFingerAngle.z : -lz / LeftFingerAngleI.z;
            SliderR.value = rz < 0 ? rz / RightFingerAngle.z : -rz / RightFingerAngleI.z;

            return;
        }

        if (Anim != null && !SelectVRM.IsFullBone)
        {
            SetFingerAngle(SliderL.value, HandL, LeftFingerAngle, LeftFingerAngleT, LeftFingerAngleI, LeftFingerAngleM, LeftFingerAngleP);
            SetFingerAngle(SliderR.value, HandR, RightFingerAngle, RightFingerAngleT, RightFingerAngleI, RightFingerAngleM, RightFingerAngleP);
        }

        TextL.gameObject.SetActive(!SelectVRM.IsFullBone);
        TextR.gameObject.SetActive(!SelectVRM.IsFullBone);
        SliderL.gameObject.SetActive(!SelectVRM.IsFullBone);
        SliderR.gameObject.SetActive(!SelectVRM.IsFullBone);
    }

    void SetFingerAngle(float value, Hand hand, Vector3 fingerAngle, Vector3 fingerAngleT, Vector3 fingerAngleI, Vector3 fingerAngleM, Vector3 fingerAngleP)
    {
        var sw = value > 0;
        var coef = sw ? value : -value;

        hand.Thumb.Proximal.localEulerAngles = fingerAngleT * coef;
        hand.Thumb.Intermediate.localEulerAngles = fingerAngleT * coef;
        hand.Thumb.Distal.localEulerAngles = fingerAngleT * coef;

        if (sw)
        {
            hand.Index.Proximal.localEulerAngles = fingerAngle * coef;
            hand.Index.Intermediate.localEulerAngles = fingerAngle * coef;
            hand.Index.Distal.localEulerAngles = fingerAngle * coef;
            hand.Middle.Proximal.localEulerAngles = fingerAngle * coef;
            hand.Middle.Intermediate.localEulerAngles = fingerAngle * coef;
            hand.Middle.Distal.localEulerAngles = fingerAngle * coef;
        }
        else
        {
            hand.Index.Proximal.localEulerAngles = (fingerAngleI + fingerAngleP) * coef;
            hand.Index.Intermediate.localEulerAngles = fingerAngleI * coef;
            hand.Index.Distal.localEulerAngles = fingerAngleI * coef;
            hand.Middle.Proximal.localEulerAngles = (fingerAngleM - fingerAngleP) * coef;
            hand.Middle.Intermediate.localEulerAngles = fingerAngleM * coef;
            hand.Middle.Distal.localEulerAngles = fingerAngleM * coef;
        }

        hand.Ring.Proximal.localEulerAngles = fingerAngle * coef;
        hand.Ring.Intermediate.localEulerAngles = fingerAngle * coef;
        hand.Ring.Distal.localEulerAngles = fingerAngle * coef;
        hand.Little.Proximal.localEulerAngles = fingerAngle * coef;
        hand.Little.Intermediate.localEulerAngles = fingerAngle * coef;
        hand.Little.Distal.localEulerAngles = fingerAngle * coef;
    }
}
