using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bloom : MonoBehaviour
{
    public Shader bloomShader;
    private Material _bloomMat;
    [Range(0.1f, 1f)]
    public float bloomScale;
    [Range(1f, 10f)] 
    public float threshold;

    [Range(1, 10)]
    public int bloomRadius;
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if(_bloomMat == null)
        {
            _bloomMat = new Material(bloomShader);
        }
        
        RenderTexture tmp = RenderTexture.GetTemporary((int)(src.width * bloomScale), (int)(src.height * bloomScale));
        RenderTexture tmp2 = RenderTexture.GetTemporary(tmp.width, tmp.height);
        _bloomMat.SetInt("_BloomRadius", bloomRadius);
        _bloomMat.SetFloat("_Threshold", threshold);
        Graphics.Blit(src, tmp, _bloomMat, 0);
        Graphics.Blit(tmp, tmp2, _bloomMat, 1);
        Graphics.Blit(tmp2, tmp, _bloomMat, 2);
        _bloomMat.SetTexture("_BloomTex", tmp);
        Graphics.Blit(src, dest, _bloomMat, 3);
        
        RenderTexture.ReleaseTemporary(tmp);
        RenderTexture.ReleaseTemporary(tmp2);
    }
}
