# Time Master Tool

## Overview
The **Time Master Tool** is a custom Unity Editor extension that facilitates dynamic control of time, lighting, and weather effects within a Unity scene. It provides a user-friendly interface for artists and designers to manage environmental settings such as time of day, fog, rain, snow, and cloud states. The tool also enables saving and loading presets for consistent environmental setups.

**YouTube Video demonstrating the tool:**

[![Time Master Tool Demo](https://img.youtube.com/vi/0yUVGae7ons/0.jpg)](https://youtu.be/0yUVGae7ons)

---

## Features
- **Dynamic Time-of-Day Control:** Adjust the time of day using a slider in real-time.
- **Weather Settings:** Enable rain, snow, or both with automatic cloud activation.
- **Fog Control:** Toggle and configure fog intensity and color dynamically.
- **Lighting Presets:** Edit ambient, fog, and directional light gradients directly in the editor.
- **Preset Management:** Save, load, and delete time and weather presets for reuse.
- **Custom Shaders Integration:** Adjust `SnowOpacity` on materials globally to simulate snow accumulation.

---

## Technical Details

### 1. Core Scripts
- **TimeMasterTool.cs**
  - Editor window script that provides the user interface for managing time and weather settings.
  - Handles saving/loading presets and interactions with the `LightCycle` component.

- **LightCycle.cs**
  - Manages in-game lighting and environmental state updates.
  - Updates material properties for snow effects and handles time progression.

### 2. Shader Integration
The tool modifies the `SnowOpacity` property of shaders used in scene materials to simulate snow effects dynamically. It does this by iterating through all materials using the shader and adjusting the slider value.

```csharp
public void SetSnowOpacity(float opacity)
{
    Shader.SetGlobalFloat("_SnowOpacity", opacity);
}
```

Ensure that the shader used in materials supports a `_SnowOpacity` float property.

### 3. Preset System
Presets are stored as JSON files in the project directory, enabling persistence between sessions. Each preset saves the following:
- Time of Day
- Time Speed
- Fog State and Settings
- Weather States (Rain, Snow, Clouds)
- Preset Name

**File Path:** `Assets/Configs/TimePresets.json`

### 4. UI Components
- **Time Controls:** Slider for `Time of Day` and `Time Speed`.
- **Lighting Presets:** Assign and modify lighting gradients.
- **Weather Toggles:** Rain and Snow toggle buttons with validation.
- **Fog Settings:** Enable/disable fog and configure density/color.
- **Preset Buttons:** Save, load, and delete custom presets.

### 5. Dependencies
- `LightingPreset` ScriptableObject
  - Stores lighting gradients for ambient light, fog color, and directional light.
- `LightCycle` MonoBehaviour
  - Component required in the scene to enable dynamic updates.

---

## Usage Instructions

### 1. Setup
1. Attach the `LightCycle` component to a GameObject in the scene.
2. Assign the `LightingPreset` ScriptableObject to the `LightCycle` component.
3. Open the **Time Master Tool** window from `Tools > Time Master Tool` in the Unity Editor.

### 2. Adjusting Time and Weather
- Use the **Time of Day** slider to modify the time visually.
- Enable/disable rain, snow, and fog with toggles.
- Adjust fog density and color for atmospheric effects.

### 3. Managing Presets
- **Save Preset:** Enter a name and click "Save Current as Preset" to store the current configuration.
- **Load Preset:** Click "Load" next to a saved preset to apply its settings.
- **Delete Preset:** Click "Delete" next to a preset to remove it permanently.

### 4. Snow Shader Integration
Ensure all materials using snow effects have a shader with a `_SnowOpacity` property. The tool will manage this globally when snow is enabled.

---

## Known Limitations
- Requires the `LightCycle` component and associated GameObjects (cloud, rain, snow) to be pre-configured in the scene.
- Custom shaders must include a `_SnowOpacity` property to integrate with the snow effect system.

---
## Credits
- **Unity Asset:** The tool utilizes assets from the Low Poly Nature Pack Lite available on the Unity Asset Store. https://assetstore.unity.com/packages/3d/environments/landscapes/low-poly-nature-pack-lite-288596
- **Day-Night Cycle Inspired by Probably Spoonie:** https://www.youtube.com/watch?v=m9hj9PdO328
