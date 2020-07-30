using UnityEngine;

public class SetRenTex : MonoBehaviour
{
    [SerializeField] Camera Camera;
    [SerializeField] RenderTexture RenTex;

    public void Set()
    {
        Camera.targetTexture = RenTex;
    }
}
