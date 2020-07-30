using UnityEngine;
using UnityEngine.UI;

public class LinkCamera : MonoBehaviour
{
    [SerializeField] Camera Camera;
    [SerializeField] Slider SliderFoV;
    [SerializeField] Slider SliderR;
    [SerializeField] Slider SliderG;
    [SerializeField] Slider SliderB;
    [SerializeField] Slider SliderA;
    [SerializeField] Slider SliderZ;

    void Update()
    {
        var color = Camera.backgroundColor;
        color.r = SliderR.value;
        color.g = SliderG.value;
        color.b = SliderB.value;
        color.a = SliderA.value;
        Camera.backgroundColor = color;

        Camera.fieldOfView = SliderFoV.value;

        var angle = Camera.gameObject.transform.eulerAngles;
        angle.z = -SliderZ.value;
        Camera.gameObject.transform.eulerAngles = angle;
    }
}
