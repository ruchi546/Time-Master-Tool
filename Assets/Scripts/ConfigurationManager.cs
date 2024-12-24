using System.IO;
using UnityEngine;

public static class ConfigurationManager
{
    /// <summary>
    /// Save a serializable object to a JSON file.
    /// </summary>
    public static void Save<T>(T config, string filePath)
    {
        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"Configuration saved to {filePath}");
    }

    /// <summary>
    /// Load a serializable object from a JSON file.
    /// </summary>
    public static T Load<T>(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Configuration file not found: {filePath}");
            return default;
        }

        string json = File.ReadAllText(filePath);
        T config = JsonUtility.FromJson<T>(json);
        Debug.Log($"Configuration loaded from {filePath}");
        return config;
    }
}