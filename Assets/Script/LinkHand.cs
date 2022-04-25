using UnityEngine;
using UnityEngine.UI;

public class LinkHand : MonoBehaviour
{
    [SerializeField] GameObject HandUI;

    [SerializeField] Slider SliderL_Thumb;
    [SerializeField] Slider SliderL_Index;
    [SerializeField] Slider SliderL_Middle;
    [SerializeField] Slider SliderL_Ring;
    [SerializeField] Slider SliderL_Little;
    [SerializeField] Slider SliderL_Piece;

    [SerializeField] Slider SliderR_Thumb;
    [SerializeField] Slider SliderR_Index;
    [SerializeField] Slider SliderR_Middle;
    [SerializeField] Slider SliderR_Ring;
    [SerializeField] Slider SliderR_Little;
    [SerializeField] Slider SliderR_Piece;

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
    Vector3 LeftFingerAngleT = new Vector3(30, -30, 0);
    Vector3 LeftFingerAngleP = new Vector3(0, 20, 0);

    Vector3 RightFingerAngle = new Vector3(0, 0, -80);
    Vector3 RightFingerAngleT = new Vector3(30, 30, 0);
    Vector3 RightFingerAngleP = new Vector3(0, -20, 0);

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

            var leftY = HandL.Index.Proximal.localEulerAngles.y;
            var rightY = HandR.Index.Proximal.localEulerAngles.y;
            leftY = leftY < 180 ? leftY : leftY - 360;
            rightY = rightY > 180 ? rightY - 360 : rightY;

            SliderL_Thumb.value = HandL.Thumb.Proximal.localEulerAngles.x / LeftFingerAngleT.x;
            SliderL_Index.value = HandL.Index.Proximal.localEulerAngles.z / LeftFingerAngle.z;
            SliderL_Middle.value = HandL.Middle.Proximal.localEulerAngles.z / LeftFingerAngle.z;
            SliderL_Ring.value = HandL.Ring.Proximal.localEulerAngles.z / LeftFingerAngle.z;
            SliderL_Little.value = HandL.Little.Proximal.localEulerAngles.z / LeftFingerAngle.z;
            SliderL_Piece.value = leftY / LeftFingerAngleP.y;

            SliderR_Thumb.value = HandR.Thumb.Proximal.localEulerAngles.x / RightFingerAngleT.x;
            SliderR_Index.value = -HandR.Index.Proximal.localEulerAngles.z / RightFingerAngle.z;
            SliderR_Middle.value = -HandR.Middle.Proximal.localEulerAngles.z / RightFingerAngle.z;
            SliderR_Ring.value = -HandR.Ring.Proximal.localEulerAngles.z / RightFingerAngle.z;
            SliderR_Little.value = -HandR.Little.Proximal.localEulerAngles.z / RightFingerAngle.z;
            SliderR_Piece.value = rightY / RightFingerAngleP.y;

            return;
        }

        if (Anim != null && !SelectVRM.IsFullBone)
        {
            SetFingerAngle(SliderL_Thumb.value, SliderL_Index.value, SliderL_Middle.value, SliderL_Ring.value, SliderL_Little.value, SliderL_Piece.value,
                HandL, LeftFingerAngle, LeftFingerAngleT, LeftFingerAngleP);
            SetFingerAngle(SliderR_Thumb.value, SliderR_Index.value, SliderR_Middle.value, SliderR_Ring.value, SliderR_Little.value, SliderR_Piece.value,
                HandR, RightFingerAngle, RightFingerAngleT, RightFingerAngleP);
        }

        HandUI.gameObject.SetActive(!SelectVRM.IsFullBone);
    }

    void SetFingerAngle(float thumb, float index, float middle, float ring, float little, float piece,
        Hand hand, Vector3 fingerAngle, Vector3 fingerAngleT, Vector3 fingerAngleP)
    {
        hand.Thumb.Proximal.localEulerAngles = fingerAngleT * thumb;
        hand.Thumb.Intermediate.localEulerAngles = fingerAngleT * thumb;
        hand.Thumb.Distal.localEulerAngles = fingerAngleT * thumb;

        hand.Index.Proximal.localEulerAngles = fingerAngle * index + fingerAngleP * piece;
        hand.Index.Intermediate.localEulerAngles = fingerAngle * index;
        hand.Index.Distal.localEulerAngles = fingerAngle * index;

        hand.Middle.Proximal.localEulerAngles = fingerAngle * middle - fingerAngleP * piece;
        hand.Middle.Intermediate.localEulerAngles = fingerAngle * middle;
        hand.Middle.Distal.localEulerAngles = fingerAngle * middle;

        hand.Ring.Proximal.localEulerAngles = fingerAngle * ring;
        hand.Ring.Intermediate.localEulerAngles = fingerAngle * ring;
        hand.Ring.Distal.localEulerAngles = fingerAngle * ring;

        hand.Little.Proximal.localEulerAngles = fingerAngle * little;
        hand.Little.Intermediate.localEulerAngles = fingerAngle * little;
        hand.Little.Distal.localEulerAngles = fingerAngle * little;
    }
}
