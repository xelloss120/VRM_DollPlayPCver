using UnityEngine;
using UnityEngine.UI;

public class LinkMarkerAlpha : MonoBehaviour
{
    [SerializeField] Slider Slider;

    void Update()
    {
        var color = GetComponent<Renderer>().material.color;
        color.a = Slider.value;
        GetComponent<Renderer>().material.color = color;
    }
}
