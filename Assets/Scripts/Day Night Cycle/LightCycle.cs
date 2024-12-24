using TMPro;
using UnityEngine;

[ExecuteAlways]
public class LightCycle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light directionalLight;
    [SerializeField] public LightingPreset preset;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Material snowMaterial;
    
    [HideInInspector] public float timeSpeed = 1f;
    [HideInInspector] public float timeOfDay;
    [HideInInspector] public bool fogEnabled = true;
    [HideInInspector] public bool isPlaying = false;
    
    [Header("Weather Settings")]
    [SerializeField] public GameObject cloudObject;
    [SerializeField] public GameObject rainObject;
    [SerializeField] public GameObject snowObject;

    [HideInInspector] public bool isRaining = false;
    [HideInInspector] public bool isSnowing = false;
    
    private void Update()
    {
        if (preset == null) return;

        if (Application.isPlaying)
        {
            UpdateTime();
        }
        UpdateLighting(timeOfDay / 24f);
    }
    
    public void UpdateTimeFromEditor(float newTime)
    {
        timeOfDay = newTime % 24f;
        UpdateLighting(timeOfDay / 24f);
        UpdateTimeText();
    }
    
    private void UpdateTime()
    {
        timeOfDay = (timeOfDay + Time.deltaTime * timeSpeed) % 24f;
        UpdateTimeText();
    }
    
    private void UpdateTimeText()
    {
        if (timeText != null)
        {
            int hours = Mathf.FloorToInt(timeOfDay);
            int minutes = Mathf.FloorToInt((timeOfDay % 1) * 60);
            timeText.text = $"{hours}:{minutes:00}h";
        }
    }
    
    private void UpdateWeather()
    {
        if (cloudObject != null)
            cloudObject.SetActive(isRaining || isSnowing);

        if (rainObject != null)
            rainObject.SetActive(isRaining);

        if (snowObject != null)
            snowObject.SetActive(isSnowing);
    }

    private void UpdateLighting(float timePercent)
    {
        // Update ambient and fog settings
        RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fog = fogEnabled;
        if (fogEnabled)
        {
            RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);
            if (preset.FogDensity != null)
                RenderSettings.fogDensity = preset.FogDensity.Evaluate(timePercent);
        }

        // Update directional light settings
        if (directionalLight != null)
        {
            directionalLight.color = preset.DirectionalColor.Evaluate(timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
            directionalLight.intensity = (isRaining || isSnowing) ? 0.3f : 1.6f;
        }
    }
    
    public void SetRaining(bool raining)
    {
        isRaining = raining;
        if (raining) isSnowing = false;
        UpdateWeather();
    }
    
    public void SetSnowing(bool snowing)
    {
        isSnowing = snowing;
        if (snowing)
        {
            isRaining = false;
            UpdateShaderSnowOpacity(1f);
        }
        else
        {
            UpdateShaderSnowOpacity(0f);
        }
        UpdateWeather();
    }

    private void UpdateShaderSnowOpacity(float opacity)
    {
        if (snowMaterial != null)
        {
            snowMaterial.SetFloat("_SnowOpacity", opacity);
        }
        else
        {
            Debug.LogWarning("The snow material is not assigned in the LightCycle script.");
        }
    }
}