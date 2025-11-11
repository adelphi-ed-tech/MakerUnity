using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomLight))]
public class CustomLightEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying && GUI.changed)
        {
            LightingHelper.Instance.UpdateLightingDataFromInspectorGuiChange();
        }
    }
}
