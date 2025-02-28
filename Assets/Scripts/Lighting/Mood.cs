using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mood")]
public class Mood : ScriptableObject
{
    [Header("Surface")]
    public Color specularColor;
    [Range(0, 1)]
    public float smoothness;
    
    [Header("Ambient lighting")]
    public Color ambientColor;
    
    [Header("Point lights")]
    [Tooltip("Units are number of point lights / square meter")]
    [Range(0, 0.5f)]
    public float lightSourceDensity;
    public Color lightColor;
    [Range(0, 20f)]
    public float lightRadius;

    [Header("Volumetric fog")] 
    public Color fogColor;
    
    [Header("Particles")]
    public GameObject particlePrefab;
    [Range(0, 2f)]
    [Tooltip("In units of emission / second / cube meter")]
    public float particleDensity;
}
