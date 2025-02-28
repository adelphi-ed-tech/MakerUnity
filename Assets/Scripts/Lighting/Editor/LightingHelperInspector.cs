using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(LightingHelper))]
public class LightingHelperInspector : Editor
{
    private float sliderValue;
    private Mood[] _prevMoods;
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LightingHelper lightingHelper = (LightingHelper)target;

        if (lightingHelper.moods.Count == 0 || lightingHelper.moods[0] == null)
        {
            EditorGUILayout.HelpBox("No moods found. Please create a mood and assign it to LightingHelper.", MessageType.Error);
        }

        // detect changes in the mood list to regenerate moods enum
        if (GUI.changed)
        {
            UpdateMoodsEnum(lightingHelper.moods);
        }
    }

    private void UpdateMoodsEnum(List<Mood> moods)
    {
        string path = Application.dataPath + "/Scripts/Lighting/LightingHelper.cs";

        if (File.Exists(path))
        {
            List<string> lines = new List<string>(File.ReadAllLines(path));
            int hookLine = -1;
            int hookEnd = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                if(lines[i].Contains("#hook"))
                {
                    string enumString = "\tpublic enum Moods\n\t{\n";
                    for (int j = 0; j < moods.Count; j++)
                    {
                        enumString += $"\t\t{moods[j].name},\n";
                    }
                    enumString += "\t}\n";
                    enumString += "\t//#endhook";
                    lines[i] += "\n" + enumString;
                    hookLine = i;
                }
            }
            for(int i = hookLine + 1; i < lines.Count; i++)
            {
                if (lines[i].Contains("#endhook"))
                {
                    hookEnd = i;
                    break;
                }
            }
            
            if (hookLine != -1 && hookEnd != -1)
            {
                lines.RemoveRange(hookLine + 1, hookEnd - hookLine);
            }
            
            File.WriteAllLines(path, lines);
        }
    }
}
