using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessingDebug : MonoBehaviour
{
    public Shader shader;
    private Material _mat;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_mat == null)
        {
            _mat = new Material(shader);
        }
        
        Graphics.Blit(src, dest, _mat);
    }
}
