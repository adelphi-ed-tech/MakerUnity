using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Compilation;

[CustomEditor(typeof(LightingHelper))]
public class LightingHelperInspector : Editor
{
    private float sliderValue;
    private Mood[] _prevMoods;
    private CustomLight[] _prevLights;
    
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
            bool moodsChanged = false;
            if (_prevMoods == null)
            {
                moodsChanged = true;

            }
            else if (_prevMoods.Length != lightingHelper.moods.Count)
            {
                moodsChanged = true;
            }
            else
            {
                for(int i = 0; i < lightingHelper.moods.Count; i++)
                {
                    if (lightingHelper.moods[i] != _prevMoods[i])
                    {
                        moodsChanged = true;
                        break;
                    }
                }
            }

            if (moodsChanged)
            {
                UpdateEnum(lightingHelper.moods, "Moods", "#mood", "#endmood");
                
                _prevMoods = new Mood[lightingHelper.moods.Count];
                for (int i = 0; i < lightingHelper.moods.Count; i++)
                {
                    _prevMoods[i] = lightingHelper.moods[i];
                }
            }
            
            bool lightsChanged = false;
            if (_prevLights == null)
            {
                lightsChanged = true;

            }
            else if (_prevLights.Length != lightingHelper.lights.Count)
            {
                lightsChanged = true;
            }
            else
            {
                for(int i = 0; i < lightingHelper.lights.Count; i++)
                {
                    if (lightingHelper.lights[i] != _prevLights[i])
                    {
                        lightsChanged = true;
                        break;
                    }
                }
            }

            if(lightsChanged)
            {
                UpdateEnum(lightingHelper.lights, "Lights", "#light", "#endlight");
                
                _prevLights = new CustomLight[lightingHelper.lights.Count];
                for (int i = 0; i < lightingHelper.lights.Count; i++)
                {
                    _prevLights[i] = lightingHelper.lights[i];
                }
            }
        }
    }

    private void UpdateEnum<T>(List<T> list, string enumName, string startHook, string endHook) where T : ScriptableObject
    {
        string path = Application.dataPath + "/Scripts/Lighting/LightingHelper.cs";

        if (File.Exists(path))
        {
            List<string> lines = new List<string>(File.ReadAllLines(path));
            int hookLine = -1;
            int hookEnd = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                if(lines[i].Contains(startHook))
                {
                    string enumString = "\tpublic enum "+enumName+"\n\t{\n";
                    for (int j = 0; j < list.Count; j++)
                    {
                        enumString += $"\t\t{list[j].name},\n";
                    }
                    enumString += "\t}\n";
                    enumString += "\t//"+endHook;
                    lines[i] += "\n" + enumString;
                    hookLine = i;
                }
            }
            for(int i = hookLine + 1; i < lines.Count; i++)
            {
                if (lines[i].Contains(endHook))
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
#if UNITY_2019_3_OR_NEWER
        CompilationPipeline.RequestScriptCompilation();
#elif UNITY_2017_1_OR_NEWER
        var editorAssembly = Assembly.GetAssembly(typeof(Editor));
        var editorCompilationInterfaceType = editorAssembly.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface");
        var dirtyAllScriptsMethod = editorCompilationInterfaceType.GetMethod("DirtyAllScripts", BindingFlags.Static | BindingFlags.Public);
        dirtyAllScriptsMethod.Invoke(editorCompilationInterfaceType, null);
#endif
        
    }
}
