using UnityEngine;

[CreateAssetMenu(menuName = "Custom Light")]
public class CustomLight : ScriptableObject
{
    public GameObject fixture;
    public LightType type;
    public Color color;
    public float range;
    public bool castsShadows;
}
