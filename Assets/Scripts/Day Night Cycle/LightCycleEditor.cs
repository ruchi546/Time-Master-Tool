using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightCycle))]
public class LightCycleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LightCycle lightCycle = (LightCycle)target;

        if (GUILayout.Button("Open Time Master Tool"))
        {
            TimeMasterTool.ShowWindow();
        }
    }
}