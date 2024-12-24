using UnityEngine;

[CreateAssetMenu(fileName = "LightingPreset", menuName = "Scriptable Objects/LightingPreset")]
public class LightingPreset : ScriptableObject
{
    public Gradient AmbientColor;
    public Gradient DirectionalColor;
    public Gradient FogColor;
    public AnimationCurve FogDensity;
}
