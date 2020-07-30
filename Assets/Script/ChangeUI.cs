using System.Collections.Generic;
using UnityEngine;

public class ChangeUI : MonoBehaviour
{
    [SerializeField] List<GameObject> UI_Object;

    public void Change()
    {
        foreach(GameObject ui in UI_Object)
        {
            ui.SetActive(!ui.activeInHierarchy);
        }
    }
}
