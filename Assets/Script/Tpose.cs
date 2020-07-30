using UnityEngine;

public class Tpose : MonoBehaviour
{
    [SerializeField] SelectVRM SelectVRM;

    public void Set()
    {
        if (!SelectVRM.IsActive || SelectVRM.Marker == null)
        {
            return;
        }

        SelectVRM.RootMarker.transform.rotation = Quaternion.identity;

        foreach (Transform t in SelectVRM.Marker.List)
        {
            var check = t.gameObject.GetComponent<LinkSwivelOffset>();
            if (!check)
            {
                t.localRotation = Quaternion.identity;
                t.gameObject.GetComponent<LinkRotation>().Target.localRotation = Quaternion.identity;
            }
        }
    }
}
