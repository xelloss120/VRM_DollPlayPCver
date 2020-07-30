using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRM;

public class LinkBlendShape : MonoBehaviour
{
    [SerializeField] SelectVRM SelectVRM;
    [SerializeField] Slider NEUTRAL;
    [SerializeField] Slider A;
    [SerializeField] Slider I;
    [SerializeField] Slider U;
    [SerializeField] Slider E;
    [SerializeField] Slider O;
    [SerializeField] Slider BLINK;
    [SerializeField] Slider JOY;
    [SerializeField] Slider ANGRY;
    [SerializeField] Slider SORROW;
    [SerializeField] Slider FUN;
    [SerializeField] Slider LOOKUP;
    [SerializeField] Slider LOOKDOWN;
    [SerializeField] Slider LOOKLEFT;
    [SerializeField] Slider LOOKRIGHT;
    [SerializeField] Slider BLINK_L;
    [SerializeField] Slider BLINK_R;
    [SerializeField] GameObject Content;
    [SerializeField] GameObject Panel;
    [SerializeField] GameObject VRM;
    [SerializeField] GameObject Full;
    [SerializeField] Toggle VRM_Toggle;
    [SerializeField] Toggle Full_Toggle;

    VRMBlendShapeProxy Proxy;

    List<SkinnedMeshRenderer> Skinneds;
    List<Slider> Sliders = new List<Slider>();

    public void SwitchingVRM()
    {
        VRM.SetActive(VRM_Toggle.isOn);
        Proxy.enabled = VRM_Toggle.isOn;
        GetBlendShape();
    }

    public void SwitchingFull()
    {
        Full.SetActive(Full_Toggle.isOn);
        Proxy.enabled = VRM_Toggle.isOn;
        GetBlendShape();
    }

    void Update()
    {
        if (!SelectVRM.IsActive)
        {
            Proxy = null;
            return;
        }

        if (Proxy != SelectVRM.Proxy)
        {
            Proxy = SelectVRM.Proxy;

            GetBlendShape();

            VRM_Toggle.SetIsOnWithoutNotify(Proxy.enabled);
            Full_Toggle.SetIsOnWithoutNotify(!Proxy.enabled);
            VRM.SetActive(Proxy.enabled);
            Full.SetActive(!Proxy.enabled);

            return;
        }

        if (Proxy.enabled)
        {
            Proxy.AccumulateValue(BlendShapePreset.Neutral, NEUTRAL.value);
            Proxy.AccumulateValue(BlendShapePreset.A, A.value);
            Proxy.AccumulateValue(BlendShapePreset.I, I.value);
            Proxy.AccumulateValue(BlendShapePreset.U, U.value);
            Proxy.AccumulateValue(BlendShapePreset.E, E.value);
            Proxy.AccumulateValue(BlendShapePreset.O, O.value);
            Proxy.AccumulateValue(BlendShapePreset.Blink, BLINK.value);
            Proxy.AccumulateValue(BlendShapePreset.Joy, JOY.value);
            Proxy.AccumulateValue(BlendShapePreset.Angry, ANGRY.value);
            Proxy.AccumulateValue(BlendShapePreset.Sorrow, SORROW.value);
            Proxy.AccumulateValue(BlendShapePreset.Fun, FUN.value);
            Proxy.AccumulateValue(BlendShapePreset.LookUp, LOOKUP.value);
            Proxy.AccumulateValue(BlendShapePreset.LookDown, LOOKDOWN.value);
            Proxy.AccumulateValue(BlendShapePreset.LookLeft, LOOKLEFT.value);
            Proxy.AccumulateValue(BlendShapePreset.LookRight, LOOKRIGHT.value);
            Proxy.AccumulateValue(BlendShapePreset.Blink_L, BLINK_L.value);
            Proxy.AccumulateValue(BlendShapePreset.Blink_R, BLINK_R.value);
            Proxy.Apply();
        }
        else
        {
            int index = 0;
            foreach (var skinned in Skinneds)
            {
                for (int i = 0; i < skinned.sharedMesh.blendShapeCount; i++)
                {
                    var value = Sliders[index].value;
                    skinned.SetBlendShapeWeight(i, value);
                    index++;
                }
            }
        }
    }

    void GetBlendShape()
    {
        // VRM
        NEUTRAL.value = Proxy.GetValue(BlendShapePreset.Neutral);
        A.value = Proxy.GetValue(BlendShapePreset.A);
        I.value = Proxy.GetValue(BlendShapePreset.I);
        U.value = Proxy.GetValue(BlendShapePreset.U);
        E.value = Proxy.GetValue(BlendShapePreset.E);
        O.value = Proxy.GetValue(BlendShapePreset.O);
        BLINK.value = Proxy.GetValue(BlendShapePreset.Blink);
        JOY.value = Proxy.GetValue(BlendShapePreset.Joy);
        ANGRY.value = Proxy.GetValue(BlendShapePreset.Angry);
        SORROW.value = Proxy.GetValue(BlendShapePreset.Sorrow);
        FUN.value = Proxy.GetValue(BlendShapePreset.Fun);
        LOOKUP.value = Proxy.GetValue(BlendShapePreset.LookUp);
        LOOKDOWN.value = Proxy.GetValue(BlendShapePreset.LookDown);
        LOOKLEFT.value = Proxy.GetValue(BlendShapePreset.LookLeft);
        LOOKRIGHT.value = Proxy.GetValue(BlendShapePreset.LookRight);
        BLINK_L.value = Proxy.GetValue(BlendShapePreset.Blink_L);
        BLINK_R.value = Proxy.GetValue(BlendShapePreset.Blink_R);

        // FBX
        for (int i = 0; i < Content.transform.childCount; i++)
        {
            Destroy(Content.transform.GetChild(i).gameObject);
        }
        Sliders.Clear();

        var count = 0;
        var skinneds = SelectVRM.RootMarker.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(var skinned in skinneds)
        {
            for (int i = 0; i < skinned.sharedMesh.blendShapeCount; i++)
            {
                var panel = Instantiate(Panel);
                panel.transform.SetParent(Content.transform);

                var text = panel.transform.GetChild(0).GetComponent<Text>();
                text.text = skinned.sharedMesh.GetBlendShapeName(i);

                var slider = panel.transform.GetChild(1).GetComponent<Slider>();
                slider.maxValue = 100;
                slider.value = skinned.GetBlendShapeWeight(i);
                Sliders.Add(slider);

                count++;
            }
        }
        Skinneds = new List<SkinnedMeshRenderer>(skinneds);

        var rect = Content.GetComponent<RectTransform>();
        var size = rect.sizeDelta;
        size.y = count * 20;
        rect.sizeDelta = size;
    }
}
