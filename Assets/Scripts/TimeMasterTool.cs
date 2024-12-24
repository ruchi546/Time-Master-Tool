using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class TimePreset
{
    public float timeOfDay;
    public float timeSpeed;
    public bool fogEnabled;
    public bool isRaining;
    public bool isSnowing;
    public bool cloudsActive;
    public string presetName;

    public TimePreset(string name, float timeOfDay, float timeSpeed, bool fogEnabled, bool isRaining, bool isSnowing, bool cloudsActive)
    {
        presetName = name;
        this.timeOfDay = timeOfDay;
        this.timeSpeed = timeSpeed;
        this.fogEnabled = fogEnabled;
        this.isRaining = isRaining;
        this.isSnowing = isSnowing;
        this.cloudsActive = cloudsActive;
    }
}


[System.Serializable]
public class TimePresetCollection
{
    public List<TimePreset> presets = new List<TimePreset>();
}

public class TimeMasterTool : EditorWindow
{
    private LightCycle lightCycle;
    private const string ConfigPath = "Assets/Configs/TimePresets.json";
    private TimePresetCollection presetCollection = new TimePresetCollection();
    private string newPresetName = "New Preset";

    [MenuItem("Tools/Time Master Tool")]
    public static void ShowWindow()
    {
        GetWindow<TimeMasterTool>("Time Master Tool");
    }
    
    private void OnEnable()
    {
        if (File.Exists(ConfigPath))
        {
            presetCollection = ConfigurationManager.Load<TimePresetCollection>(ConfigPath);
        }
    }
    
    private void OnFocus()
    {
        if (lightCycle == null)
        {
            lightCycle = FindFirstObjectByType<LightCycle>();
        }
    }
    
    private void OnGUI()
    {
        DrawHeader();
        if (!DrawLightCycleReference()) return;
        DrawLightingPresetSection();
        DrawFogSettingsSection();
        DrawWeatherSettingsSection();
        DrawSimulationSettingsSection();
        DrawPresetButtons();
        DrawCustomPresetSection();
    }
    
    private void DrawHeader()
    {
        GUILayout.Label("Time Master Tool", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Manage time and lighting presets dynamically in your scene.", MessageType.Info);
    }
    
    private bool DrawLightCycleReference()
    {
        lightCycle = (LightCycle)EditorGUILayout.ObjectField("Light Cycle Script", lightCycle, typeof(LightCycle), true);
        if (lightCycle == null)
        {
            EditorGUILayout.HelpBox("Please assign a LightCycle script to use this tool.", MessageType.Warning);
            return false;
        }

        return true;
    }
    
    private void DrawLightingPresetSection()
    {
        GUILayout.Space(10);
        GUILayout.Label("Lighting Preset", EditorStyles.boldLabel);
        
        lightCycle.preset = (LightingPreset)EditorGUILayout.ObjectField("Preset", lightCycle.preset, typeof(LightingPreset), false);
        if (lightCycle.preset != null)
        {
            GUILayout.Label("Edit Preset Gradients", EditorStyles.boldLabel);
            SerializedObject serializedPreset = new SerializedObject(lightCycle.preset);

            EditorGUILayout.PropertyField(serializedPreset.FindProperty("AmbientColor"));
            EditorGUILayout.PropertyField(serializedPreset.FindProperty("FogColor"));
            EditorGUILayout.PropertyField(serializedPreset.FindProperty("DirectionalColor"));

            serializedPreset.ApplyModifiedProperties();
        }
        else
        {
            EditorGUILayout.HelpBox("Assign a Lighting Preset to enable editing.", MessageType.Info);
        }
    }
    
    private void DrawFogSettingsSection()
    {
        GUILayout.Space(10);
        GUILayout.Label("Fog Settings", EditorStyles.boldLabel);
        
        lightCycle.fogEnabled = EditorGUILayout.Toggle("Enable Fog", lightCycle.fogEnabled);
        if (lightCycle.fogEnabled && lightCycle.preset != null)
        {
            SerializedObject serializedPreset = new SerializedObject(lightCycle.preset);

            EditorGUILayout.PropertyField(serializedPreset.FindProperty("FogColor"));
            
            EditorGUILayout.PropertyField(serializedPreset.FindProperty("FogDensity"), new GUIContent("Fog Density", "Recommended range: 0.0 - 0.1"));
            
            serializedPreset.ApplyModifiedProperties();
        }
    }

    private void DrawWeatherSettingsSection()
    {
        GUILayout.Space(10);
        GUILayout.Label("Weather Settings", EditorStyles.boldLabel);
        
        // Rain and Snow Toggles
        bool newRainValue = EditorGUILayout.Toggle("Enable Rain", lightCycle.isRaining);
        if (newRainValue != lightCycle.isRaining)
        {
            Undo.RecordObject(lightCycle, "Toggled Rain");
            lightCycle.SetRaining(newRainValue);
        }

        bool newSnowValue = EditorGUILayout.Toggle("Enable Snow", lightCycle.isSnowing);
        if (newSnowValue != lightCycle.isSnowing)
        {
            Undo.RecordObject(lightCycle, "Toggled Snow");
            lightCycle.SetSnowing(newSnowValue);
        }
        
        if (lightCycle.cloudObject == null || lightCycle.rainObject == null || lightCycle.snowObject == null)
        {
            EditorGUILayout.HelpBox("Assign Cloud, Rain, and Snow GameObjects in the LightCycle script.", MessageType.Warning);
        }
    }
    
    private void DrawSimulationSettingsSection()
    {
        GUILayout.Space(10);
        GUILayout.Label("Simulation Settings", EditorStyles.boldLabel);
        
        // Time of Day Slider
        float currentSliderValue = lightCycle.timeOfDay;
        float newSliderValue = EditorGUILayout.Slider(new GUIContent("Time of Day", "Set the time of day in 24-hour format."), currentSliderValue, 0f, 24f);

        if (Mathf.Abs(newSliderValue - currentSliderValue) > 0.01f)
        {
            Undo.RecordObject(lightCycle, "Changed Time of Day");
            lightCycle.UpdateTimeFromEditor(newSliderValue);
        }
        
        // Time Speed Slider
        lightCycle.timeSpeed = EditorGUILayout.Slider("Time Speed", lightCycle.timeSpeed, 0.0f, 5f);
    }
    
    private void DrawPresetButtons()
    {
        GUILayout.Space(10);
        GUILayout.Label("Quick Presets", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Mystic Dawn (Foggy Rain)"))
        {
            Undo.RecordObject(lightCycle, "Set Mystic Dawn (Foggy Rain)");
            lightCycle.UpdateTimeFromEditor(6f);
            lightCycle.timeSpeed = 0.5f;
            lightCycle.fogEnabled = true;
            lightCycle.SetRaining(false);
            lightCycle.SetSnowing(false);
            lightCycle.cloudObject.SetActive(true);
        }
        
        if (GUILayout.Button("Radiant Midday (Clear Skies)"))
        {
            Undo.RecordObject(lightCycle, "Set Radiant Midday (Clear Skies)");
            lightCycle.UpdateTimeFromEditor(12f);
            lightCycle.timeSpeed = 0.5f;
            lightCycle.fogEnabled = false;
            lightCycle.SetRaining(false);
            lightCycle.SetSnowing(false);
            lightCycle.cloudObject.SetActive(false);
        }

        if (GUILayout.Button("Serene Dust"))
        {
            Undo.RecordObject(lightCycle, "Set Serene Dust");
            lightCycle.UpdateTimeFromEditor(18f);
            lightCycle.timeSpeed = 0.2f;
            lightCycle.fogEnabled = false;
            lightCycle.SetRaining(false);
            lightCycle.SetSnowing(false);
            lightCycle.cloudObject.SetActive(false);
        }

        if (GUILayout.Button("Set Frosty Night"))
        {
            Undo.RecordObject(lightCycle, "Set Frosty Night");
            lightCycle.UpdateTimeFromEditor(22f);
            lightCycle.timeSpeed = 0.2f;
            lightCycle.fogEnabled = false;
            lightCycle.SetRaining(false);
            lightCycle.SetSnowing(true);
            lightCycle.cloudObject.SetActive(true);
        }
    }
    
    private void DrawCustomPresetSection()
    {
        GUILayout.Space(10);
        GUILayout.Label("Custom Presets", EditorStyles.boldLabel);
        
        newPresetName = EditorGUILayout.TextField("Preset Name", newPresetName);
        if (GUILayout.Button("Save Current as Preset"))
        {
            if (!string.IsNullOrEmpty(newPresetName))
            {
                SavePreset(newPresetName);
            }
            else
            {
                Debug.LogWarning("Preset name cannot be empty!");
            }
        }

        if (presetCollection.presets.Count > 0)
        {
            GUILayout.Label("Saved Presets:", EditorStyles.boldLabel);

            List<TimePreset> presetsToDelete = new List<TimePreset>();
            foreach (var preset in presetCollection.presets)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(preset.presetName);

                if (GUILayout.Button("Load"))
                {
                    LoadPreset(preset);
                }

                if (GUILayout.Button("Delete"))
                {
                    presetsToDelete.Add(preset);
                }
                GUILayout.EndHorizontal();
            }

            foreach (var preset in presetsToDelete)
            {
                presetCollection.presets.Remove(preset);
            }

            if (presetsToDelete.Count > 0)
            {
                ConfigurationManager.Save(presetCollection, ConfigPath);
            }
        }
        else
        {
            GUILayout.Label("No presets saved.", EditorStyles.label);
        }
    }
    
    private void SavePreset(string presetName)
    {
        TimePreset newPreset = new TimePreset(
            presetName,
            lightCycle.timeOfDay,
            lightCycle.timeSpeed,
            lightCycle.fogEnabled,
            lightCycle.isRaining,
            lightCycle.isSnowing,
            lightCycle.cloudObject != null && (lightCycle.isRaining || lightCycle.isSnowing) // Nubes activas solo si llueve o nieva
        );

        presetCollection.presets.Add(newPreset);
        ConfigurationManager.Save(presetCollection, ConfigPath);
        Debug.Log($"Preset '{presetName}' saved!");
    }

    private void LoadPreset(TimePreset preset)
    {
        lightCycle.UpdateTimeFromEditor(preset.timeOfDay);
        lightCycle.timeSpeed = preset.timeSpeed;
        lightCycle.fogEnabled = preset.fogEnabled;
        lightCycle.SetRaining(preset.isRaining);
        lightCycle.SetSnowing(preset.isSnowing);
        lightCycle.cloudObject.SetActive(preset.cloudsActive);

        Debug.Log($"Preset '{preset.presetName}' loaded!");
    }
}
