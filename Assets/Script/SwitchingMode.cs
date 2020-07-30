using UnityEngine;
using UnityEngine.UI;
using VRM;
using RootMotion.FinalIK;

public class SwitchingMode : MonoBehaviour
{
    [SerializeField] SelectVRM SelectVRM;
    [SerializeField] Slider Slider;

    public void Switching()
    {
        if (SelectVRM.Marker == null) return;

        // ラグドール解除
        var joints = SelectVRM.Animator.gameObject.GetComponentsInChildren<ConfigurableJoint>();
        foreach (var joint in joints)
        {
            Destroy(joint);
        }
        var rigids = SelectVRM.Animator.gameObject.GetComponentsInChildren<Rigidbody>();
        foreach (var rigid in rigids)
        {
            rigid.useGravity = false;
            if (!rigid.isKinematic)
            {
                Destroy(rigid);
            }
        }
        var capsules = SelectVRM.Animator.gameObject.GetComponentsInChildren<CapsuleCollider>();
        foreach (var capsule in capsules)
        {
            Destroy(capsule);
        }
        var boxs = SelectVRM.Animator.gameObject.GetComponentsInChildren<BoxCollider>();
        foreach (var box in boxs)
        {
            Destroy(box);
        }
        var meshs = SelectVRM.Animator.gameObject.GetComponentsInChildren<MeshCollider>();
        foreach (var mesh in meshs)
        {
            Destroy(mesh);
        }

        // モードの判定
        bool HumanoidMode = true;
        bool FullMode = true;
        foreach (var marker in SelectVRM.Marker.List)
        {
            var primal = marker.GetComponent<LinkRotation>().IsPrimal;
            if (primal && !marker.gameObject.activeInHierarchy)
            {
                HumanoidMode = false;
            }
            if (marker.name != "VRIK_Target" && !marker.gameObject.activeInHierarchy)
            {
                FullMode = false;
            }
        }
        if (FullMode)
        {
            HumanoidMode = false;
        }

        // モードの切り替え
        if (HumanoidMode)
        {
            // Humanoid > VRIK
            foreach (var marker in SelectVRM.Marker.List)
            {
                marker.gameObject.SetActive(marker.name == "VRIK_Target" ? true : false);
            }
            var count = SelectVRM.Marker.List.Count;
            SelectVRM.Marker.List[count - 10].position = SelectVRM.Marker.List[0].position;
            SelectVRM.Marker.List[count - 10].rotation = SelectVRM.Marker.List[0].GetComponent<LinkRotation>().Target.rotation;
            SelectVRM.Marker.List[count - 9].position = SelectVRM.Marker.List[2].position;
            SelectVRM.Marker.List[count - 9].rotation = SelectVRM.Marker.List[2].GetComponent<LinkRotation>().Target.rotation;
            SelectVRM.Marker.List[count - 8].position = SelectVRM.Marker.List[8].position;
            SelectVRM.Marker.List[count - 8].rotation = SelectVRM.Marker.List[8].GetComponent<LinkRotation>().Target.rotation;
            SelectVRM.Marker.List[count - 7].position = SelectVRM.Marker.List[11].position;
            SelectVRM.Marker.List[count - 7].rotation = SelectVRM.Marker.List[11].GetComponent<LinkRotation>().Target.rotation;
            var leftToes = SelectVRM.Animator.GetBoneTransform(HumanBodyBones.LeftToes);
            if (leftToes != null)
            {
                // FootだとズレるのでToesがあれば使う
                SelectVRM.Marker.List[count - 6].position = leftToes.position;
                SelectVRM.Marker.List[count - 6].rotation = leftToes.rotation;
            }
            else
            {
                SelectVRM.Marker.List[count - 6].position = SelectVRM.Marker.List[14].position;
                SelectVRM.Marker.List[count - 6].rotation = SelectVRM.Marker.List[14].GetComponent<LinkRotation>().Target.rotation;
            }
            var rightToes = SelectVRM.Animator.GetBoneTransform(HumanBodyBones.RightToes);
            if (rightToes != null)
            {
                // FootだとズレるのでToesがあれば使う
                SelectVRM.Marker.List[count - 5].position = rightToes.position;
                SelectVRM.Marker.List[count - 5].rotation = rightToes.rotation;
            }
            else
            {
                SelectVRM.Marker.List[count - 5].position = SelectVRM.Marker.List[17].position;
                SelectVRM.Marker.List[count - 5].rotation = SelectVRM.Marker.List[17].GetComponent<LinkRotation>().Target.rotation;
            }
            SelectVRM.Animator.gameObject.GetComponent<VRIK>().enabled = true;
        }
        else if (!FullMode)
        {
            // VRIK > Full
            foreach (var marker in SelectVRM.Marker.List)
            {
                marker.gameObject.SetActive(marker.name != "VRIK_Target" ? true : false);
                marker.rotation = marker.GetComponent<LinkRotation>().Target.rotation;
            }
            SelectVRM.Animator.gameObject.GetComponent<VRIK>().enabled = false;
            Slider.value = 0.02f;
        }
        else
        {
            // Full > Humanoid
            foreach (var marker in SelectVRM.Marker.List)
            {
                var primal = marker.GetComponent<LinkRotation>().IsPrimal;
                marker.gameObject.SetActive(primal ? true : false);
            }
            Slider.value = 0.1f;
        }

        // 揺れ物の有効と無効を切り替え
        var springs = SelectVRM.RootMarker.GetComponentsInChildren<VRMSpringBone>();
        foreach (var spring in springs)
        {
            spring.enabled = HumanoidMode || FullMode ? true : false;
        }
    }
}
