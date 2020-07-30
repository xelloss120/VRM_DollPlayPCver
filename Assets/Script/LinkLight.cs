using UnityEngine;
using UnityEngine.UI;

public class LinkLight : MonoBehaviour
{
    [SerializeField] Light Light;
    [SerializeField] Slider Slider;
    [SerializeField] Slider SliderR;
    [SerializeField] Slider SliderG;
    [SerializeField] Slider SliderB;

    void Update()
    {
        var color = Light.color;
        color.r = SliderR.value;
        color.g = SliderG.value;
        color.b = SliderB.value;
        Light.color = color;

        Light.intensity = Slider.value;
    }
}
