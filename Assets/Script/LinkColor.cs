using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Battlehub.RTHandles;

public class LinkColor : MonoBehaviour
{
    [SerializeField] RuntimeSceneComponent BSC;

    [SerializeField] GameObject UI;

    [SerializeField] Toggle Shadow;
    [SerializeField] Slider EmissionR;
    [SerializeField] Slider EmissionG;
    [SerializeField] Slider EmissionB;
    [SerializeField] Slider LitColorR;
    [SerializeField] Slider LitColorG;
    [SerializeField] Slider LitColorB;
    [SerializeField] Slider Alpha;

    GameObject PreActive;
    MeshRenderer Mesh;
    Material Mat;

    void Update()
    {
        Color Emission;
        Color LitColor;

        var active = BSC.Editor.Selection.activeGameObject;
        if (active != null && active != PreActive)
        {
            // 選択切り替え時の条件初期化
            PreActive = active;
            UI.SetActive(false);
            Mesh = null;
            Mat = null;

            var image = active.transform.root.GetComponent<ImageFile>();
            var renderer = active.transform.root.GetComponent<Renderer>();
            if ((image != null && renderer != null) || active.name == "Ground")
            {
                // UIの有効化
                UI.SetActive(true);
                Mat = renderer.material;

                // 影有無の設定
                Mesh = active.transform.root.GetComponent<MeshRenderer>();
                Shadow.isOn = Mesh.receiveShadows;

                // Emissionの取得とスライダーへの設定
                Emission = Mat.GetColor("_EmissionColor");
                EmissionR.value = Emission.r;
                EmissionG.value = Emission.g;
                EmissionB.value = Emission.b;

                // LitColorの取得とスライダーへの設定
                LitColor = Mat.GetColor("_Color");
                LitColorR.value = LitColor.r;
                LitColorG.value = LitColor.g;
                LitColorB.value = LitColor.b;
                Alpha.value = LitColor.a;
            }
        }
        else if (Mat != null && Mesh != null)
        {
            // 影有無の設定
            Mesh.receiveShadows = Shadow.isOn;
            Mesh.shadowCastingMode = Shadow.isOn ? ShadowCastingMode.On : ShadowCastingMode.Off;
            Mat.renderQueue = Shadow.isOn ? (int)RenderQueue.AlphaTest : (int)RenderQueue.Transparent;

            // Emissionの操作
            Emission.r = EmissionR.value;
            Emission.g = EmissionG.value;
            Emission.b = EmissionB.value;
            Emission.a = 1;
            Mat.SetColor("_EmissionColor", Emission);

            // LitColorの操作
            LitColor.r = LitColorR.value;
            LitColor.g = LitColorG.value;
            LitColor.b = LitColorB.value;
            LitColor.a = Alpha.value;
            Mat.SetColor("_Color", LitColor);

            // ShadeColorの操作
            Color32 color = LitColor;
            color.r = (byte)Mathf.Max(0, color.r - 0x2F);
            color.g = (byte)Mathf.Max(0, color.g - 0x1F);
            color.b = (byte)Mathf.Max(0, color.b - 0x0F);
            Mat.SetColor("_ShadeColor", color);
        }
    }
}
