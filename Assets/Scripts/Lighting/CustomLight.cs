using UnityEngine;

[CreateAssetMenu(menuName = "Custom Light")]
public class CustomLight : ScriptableObject
{
    public GameObject fixture;
    public LightType type;
    public Color color;
    public float range;
    public bool castsShadows;
    [Header("Spot light only")] 
    [Range(0f, 180f)]
    public float spotAngle;
}
