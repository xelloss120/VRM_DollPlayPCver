using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRM;

public class LinkBlendShape : MonoBehaviour
{
    [SerializeField] SelectVRM SelectVRM;
    [SerializeField] GameObject Panel;
    [SerializeField] GameObject VRM;
    [SerializeField] GameObject VRMContent;
    [SerializeField] GameObject Full;
    [SerializeField] GameObject FullContent;
    [SerializeField] Toggle VRM_Toggle;
    [SerializeField] Toggle Full_Toggle;

    VRMBlendShapeProxy Proxy;
    List<Slider> VRMSliders = new List<Slider>();

    List<SkinnedMeshRenderer> Skinneds;
    List<Slider> FullSliders = new List<Slider>();

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
            var values = new Dictionary<BlendShapeKey, float>();
            var clips = Proxy.BlendShapeAvatar.Clips;
            for (int i = 0; i < clips.Count; i++)
            {
                values.Add(clips[i].Key, VRMSliders[i].value);
            }
            Proxy.SetValues(values);
        }
        else
        {
            int index = 0;
            foreach (var skinned in Skinneds)
            {
                for (int i = 0; i < skinned.sharedMesh.blendShapeCount; i++)
                {
                    var value = FullSliders[index].value;
                    skinned.SetBlendShapeWeight(i, value);
                    index++;
                }
            }
        }
    }

    public void GetBlendShape()
    {
        // VRM
        DestroyContent(VRMContent, VRMSliders);

        var clips = Proxy.BlendShapeAvatar.Clips;
        for (int i = 0; i < clips.Count; i++)
        {
            var panel = Instantiate(Panel);
            panel.transform.SetParent(VRMContent.transform);

            var text = panel.transform.GetChild(0).GetComponent<Text>();
            text.text = clips[i].BlendShapeName;

            var slider = panel.transform.GetChild(1).GetComponent<Slider>();
            slider.maxValue = 1;
            slider.value = Proxy.GetValue(clips[i].Key);
            VRMSliders.Add(slider);
        }

        SetSizeDelta(VRMContent, clips.Count);

        // FBX
        DestroyContent(FullContent, FullSliders);

        var count = 0;
        var skinneds = SelectVRM.RootMarker.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(var skinned in skinneds)
        {
            for (int i = 0; i < skinned.sharedMesh.blendShapeCount; i++)
            {
                var panel = Instantiate(Panel);
                panel.transform.SetParent(FullContent.transform);

                var text = panel.transform.GetChild(0).GetComponent<Text>();
                text.text = skinned.sharedMesh.GetBlendShapeName(i);

                var slider = panel.transform.GetChild(1).GetComponent<Slider>();
                slider.maxValue = 100;
                slider.value = skinned.GetBlendShapeWeight(i);
                FullSliders.Add(slider);

                count++;
            }
        }
        Skinneds = new List<SkinnedMeshRenderer>(skinneds);
        
        SetSizeDelta(FullContent, count);
    }

    void DestroyContent(GameObject content, List<Slider> sliders)
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
        sliders.Clear();
    }

    void SetSizeDelta(GameObject content, int count)
    {
        var rect = content.GetComponent<RectTransform>();
        var size = rect.sizeDelta;
        size.y = count * 20;
        rect.sizeDelta = size;
    }
}
