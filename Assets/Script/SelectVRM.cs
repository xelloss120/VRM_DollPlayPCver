using UnityEngine;
using Battlehub.RTHandles;
using VRM;

public class SelectVRM : MonoBehaviour
{
    [SerializeField] RuntimeSceneComponent BSC;
    [SerializeField] GameObject UI;

    public bool IsActive;
    public bool IsFullBone;
    public bool IsFullShape;
    public GameObject RootMarker;
    public Marker Marker;
    public Animator Animator;
    public VRMBlendShapeProxy Proxy;

    void Update()
    {
        IsActive = false;
        RootMarker = null;
        Marker = null;
        Animator = null;
        Proxy = null;

        var active = BSC.Editor.Selection.activeGameObject;
        if (active != null)
        {
            var root = active.transform.root;
            RootMarker = root.gameObject;

            if (root.childCount > 0)
            {
                Marker = RootMarker.GetComponent<Marker>();
                Animator = root.GetChild(0).GetComponent<Animator>();
                Proxy = root.GetChild(0).GetComponent<VRMBlendShapeProxy>();

                if (Animator != null && Proxy != null && Marker != null)
                {
                    IsActive = true;
                }
            }

            var springs = RootMarker.GetComponentsInChildren<VRMSpringBone>();
            if (springs.Length > 0) IsFullBone = !springs[0].enabled;

            IsFullShape = !Proxy.enabled;
        }

        UI.SetActive(IsActive);
    }
}
