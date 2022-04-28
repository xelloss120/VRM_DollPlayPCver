using UnityEngine;
using UnityEngine.UI;
using Battlehub.RTHandles;

public class LinkLight : MonoBehaviour
{
    public Light Light;

    [SerializeField] Slider Slider;
    [SerializeField] Slider SliderR;
    [SerializeField] Slider SliderG;
    [SerializeField] Slider SliderB;

    [SerializeField] RuntimeSceneComponent BSC;
    [SerializeField] GameObject UI;
    [SerializeField] Slider SliderRange;
    [SerializeField] Slider SliderIntensity;
    [SerializeField] Slider SliderColorR;
    [SerializeField] Slider SliderColorG;
    [SerializeField] Slider SliderColorB;
    [SerializeField] GameObject Marker;

    GameObject Active;
    Light PointLight;
    Color PointLightColor;

    void Update()
    {
        var color = Light.color;
        color.r = SliderR.value;
        color.g = SliderG.value;
        color.b = SliderB.value;
        Light.color = color;

        Light.intensity = Slider.value;

        // 選択状態判定で処理分岐
        var active = BSC.Editor.Selection.activeGameObject;
        if (active != Active)
        {
            // 選択状態が切り替わった場合（選択状態が前フレームと違う場合）
            Active = active;

            if (active != null && active.name == "PointLight")
            {
                // 選択状態が点光源に切り替わった場合
                UI.SetActive(true);

                PointLight = active.GetComponent<Light>();
                SliderRange.value = PointLight.range;
                SliderIntensity.value = PointLight.intensity;
                SliderColorR.value = PointLight.color.r;
                SliderColorG.value = PointLight.color.g;
                SliderColorB.value = PointLight.color.b;
            }
            else
            {
                // 選択状態が点光源以外に切り替わった場合（未選択を含む）
                UI.SetActive(false);
            }
        }
        if (active != null && active.name == "PointLight")
        {
            // 選択状態が点光源の場合
            PointLight.range = SliderRange.value;
            PointLight.intensity = SliderIntensity.value;

            PointLightColor.r = SliderColorR.value;
            PointLightColor.g = SliderColorG.value;
            PointLightColor.b = SliderColorB.value;
            PointLight.color = PointLightColor;
        }
    }

    public void AddPointLight()
    {
        AddPointLight(new Vector3(0, 0.5f, 0), 1, 10, Color.white);
    }

    public void AddPointLight(Vector3 position, float intensity, float range, Color color)
    {
        var markerM = Instantiate(Marker).transform;
        markerM.name = "PointLight";
        markerM.position = position;
        markerM.gameObject.AddComponent<SaveSceneTarget>().Path = "PointLight";

        var light = markerM.gameObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.shadows = LightShadows.Soft;
        light.shadowNearPlane = 0.1f;

        light.intensity = intensity;
        light.range = range;
        light.color = color;
    }

    public void SetDirectionalLight(float intensity, float r, float g, float b)
    {
        Slider.value = intensity;
        SliderR.value = r;
        SliderG.value = g;
        SliderB.value = b;
    }
}
