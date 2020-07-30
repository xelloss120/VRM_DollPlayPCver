using UnityEngine;

public class Delete : MonoBehaviour
{
    [SerializeField] SelectVRM SelectVRM;

    public void DeleteObject()
    {
        if (SelectVRM.RootMarker != null && SelectVRM.RootMarker.name != "Light")
        {
            Destroy(SelectVRM.RootMarker);
        }
    }
}
