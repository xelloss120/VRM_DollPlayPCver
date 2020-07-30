using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class LinkPPS : MonoBehaviour
{
    [SerializeField] PostProcessProfile PPP;
    [SerializeField] Slider Bloom;
    [SerializeField] Slider Chromatic;
    [SerializeField] Slider Temp;
    [SerializeField] Slider Hue;
    [SerializeField] Slider Saturation;
    [SerializeField] Slider Contrast;
    [SerializeField] Slider Depth;
    [SerializeField] Slider Vignette;

    void Update()
    {
        PPP.GetSetting<Bloom>().intensity.value = Bloom.value;
        PPP.GetSetting<ChromaticAberration>().intensity.value = Chromatic.value;
        PPP.GetSetting<ColorGrading>().temperature.value = Temp.value;
        PPP.GetSetting<ColorGrading>().hueShift.value = Hue.value;
        PPP.GetSetting<ColorGrading>().saturation.value = Saturation.value;
        PPP.GetSetting<ColorGrading>().contrast.value = Contrast.value;
        PPP.GetSetting<DepthOfField>().active = Depth.value != 0;
        PPP.GetSetting<DepthOfField>().focusDistance.value = Depth.value;
        PPP.GetSetting<Vignette>().intensity.value = Vignette.value;
    }
}
