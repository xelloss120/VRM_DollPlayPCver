using UnityEngine;
using UnityEngine.UI;
using Battlehub.RTHandles;

public class LinkMarkerSize : MonoBehaviour
{
    [SerializeField] Slider Slider;
    [SerializeField] RuntimeSceneComponent BSC;

    void Update()
    {
        if (transform.parent != null)
        {
            transform.localScale = Vector3.one * Slider.value / transform.parent.localScale.x;
        }
        else
        {
            transform.localScale = Vector3.one * Slider.value;
        }
    }
}
