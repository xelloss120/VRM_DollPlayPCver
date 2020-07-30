using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reset : MonoBehaviour
{
    [SerializeField]
    List<Slider> Sliders;

    List<float> Default;

    void Start()
    {
        Default = new List<float>();

        foreach (Slider s in Sliders)
        {
            Default.Add(s.value);
        }
    }

    public void ResetSliders()
    {
        for (int i = 0; i < Sliders.Count; i++)
        {
            Sliders[i].value = Default[i];
        }
    }
}
