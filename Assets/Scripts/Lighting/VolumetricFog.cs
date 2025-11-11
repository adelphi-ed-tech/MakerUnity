using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumetricFog : MonoBehaviour
{
    public Shader shader;
    private Material _mat;
    private Camera _cam;
    [Range(1, 16)]
    public int numSteps;

    private RenderTexture _fogTexture;

    void Start()
    {
        _fogTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        Shader.SetGlobalTexture("_FogTexture", _fogTexture);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_mat == null)
        {
            _mat = new Material(shader);
        }

        if (_cam == null)
        {
            _cam = GetComponent<Camera>();
        }
        
        if(Screen.width != _fogTexture.width || Screen.height != _fogTexture.height)
        {
            _fogTexture.Release();
            _fogTexture.width = Screen.width;
            _fogTexture.height = Screen.height;
            _fogTexture.Create();
            Shader.SetGlobalTexture("_FogTexture", _fogTexture);
        }
        
        RenderTexture fogTmp = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        RenderTexture fogTmp2 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        
        Matrix4x4 inverseView = _cam.cameraToWorldMatrix;
        _mat.SetMatrix("_InverseView", inverseView);
        _mat.SetInt("_NumSteps", numSteps);
        
        //render fog for this frame
        Graphics.Blit(null, fogTmp, _mat, 0);
        //accumulate fog into fog history buffer
        Graphics.Blit(fogTmp, fogTmp2, _mat, 1);
        //copy fog history buffer to _fogTexture
        Graphics.Blit(fogTmp2, _fogTexture);
        //combine fog with source image
        Graphics.Blit(src, dest, _mat, 2);
        
        RenderTexture.ReleaseTemporary(fogTmp);
        RenderTexture.ReleaseTemporary(fogTmp2);
    }
}
