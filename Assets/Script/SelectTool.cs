using UnityEngine;
using UnityEngine.UI;
using Battlehub.RTHandles;

public class SelectTool : MonoBehaviour
{
    [SerializeField] RuntimeSceneComponent BSC;
    [SerializeField] Toggle Move;
    [SerializeField] Toggle Rotate;

    void Update()
    {
        var active = BSC.Editor.Selection.activeGameObject;
        if (active == null)
        {
            return;
        }

        if (active.name == "TriLib_Root" ||
            active.name == "GLB_Root" ||
            active.name == "VRIK_Target")
        {
            return;
        }

        var alpha = active.GetComponent<LinkMarkerAlpha>();
        if (alpha == null)
        {
            return;
        }

        var size = active.GetComponent<LinkMarkerSize>();
        if (size == null)
        {
            Move.isOn = true;
        }
        else
        {
            Rotate.isOn = true;
        }
    }
}
