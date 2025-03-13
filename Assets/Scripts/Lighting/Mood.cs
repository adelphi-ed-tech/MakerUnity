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
    
    [Header("Light Sources")]
    [Tooltip("Units are number of point lights / square meter")]
    [Range(0, 0.5f)]
    public float lightSourceDensity;
    public Color lightColor;
    [Range(0, 20f)]
    public float lightRadius;
    public bool castShadows;
    public GameObject lightFixture;

    [Header("Volumetric fog")] 
    public Color fogColor;
    
    [Header("Particles")]
    public GameObject particlePrefab;
    [Range(0, 4f)]
    [Tooltip("In units of emission / second / cube meter")]
    public float particleDensity;

    public void Setup(Color specColor, float smoothness, Color ambientColor, float lightSourceDensity, float lightRadius,
          Color lightColor, Color fogColor, GameObject particlePrefab, float particleDensity, bool castShadows)
    {
        this.specularColor = specColor;
        this.smoothness = smoothness;
        this.ambientColor = ambientColor;
        this.lightRadius = lightRadius;
        this.lightSourceDensity = lightSourceDensity;
        this.lightColor = lightColor;
        this.fogColor = fogColor;
        this.particlePrefab = particlePrefab;
        this.particleDensity = particleDensity;
        this.castShadows = castShadows;
    }
}
