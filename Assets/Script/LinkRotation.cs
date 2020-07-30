using UnityEngine;

public class LinkRotation : MonoBehaviour
{
    public bool IsPrimal;
    public Transform Target;

    void Update()
    {
        Target.rotation = transform.rotation;
        transform.position = Target.position;
    }
}
