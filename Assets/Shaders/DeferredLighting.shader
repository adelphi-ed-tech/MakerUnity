// Copied from the built-in Unity shader "Hidden/Internal-DeferredShading"
// https://github.com/TwoTailsGames/Unity-Built-in-Shaders/blob/master/DefaultResourcesExtra/Internal-DeferredShading.shader
Shader "Hidden/DeferredLighting"
{
    Properties {
        _LightTexture0 ("", any) = "" {}
        _LightTextureB0 ("", 2D) = "" {}
        _ShadowMapTexture ("", any) = "" {}
        _SrcBlend ("", Float) = 1
        _DstBlend ("", Float) = 1
    }
    SubShader {

        // Pass 1: Lighting pass
        //  LDR case - Lighting encoded into a subtractive ARGB8 buffer
        //  HDR case - Lighting additively blended into floating point buffer
        Pass {
            ZWrite Off
            Blend [_SrcBlend] [_DstBlend]

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert_deferred
            #pragma fragment frag
            #pragma multi_compile_lightpass
            #pragma multi_compile ___ UNITY_HDR_ON

            #pragma exclude_renderers nomrt

            #include "UnityCG.cginc"
            #include "UnityDeferredLibrary.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardUtils.cginc"
            #include "UnityGBuffer.cginc"
            #include "UnityStandardBRDF.cginc"

            sampler2D _CameraGBufferTexture0;
            sampler2D _CameraGBufferTexture1;
            sampler2D _CameraGBufferTexture2;

            half4 CalculateLight (unity_v2f_deferred i)
            {
                float3 wpos;
                float2 uv;
                float atten, fadeDist;
                UnityLight light;
                float lightCode = _LightColor.a;
                if(lightCode < 1)
                {
                    //_LightColor.a = 1;
                }
                //return lightCode;
                UNITY_INITIALIZE_OUTPUT(UnityLight, light);
                UnityDeferredCalculateLightParams (i, wpos, uv, light.dir, atten, fadeDist);

                light.color = _LightColor.rgb * atten;
                //return float4(light.color, 1);
                //return _LightColor.a;
                //return 0;

                // unpack Gbuffer
                half4 gbuffer0 = tex2D (_CameraGBufferTexture0, uv);
                half4 gbuffer1 = tex2D (_CameraGBufferTexture1, uv);
                half4 gbuffer2 = tex2D (_CameraGBufferTexture2, uv);
                UnityStandardData data = UnityStandardDataFromGbuffer(gbuffer0, gbuffer1, gbuffer2);

                float matCode = data.occlusion;

                if(lightCode >= 1 && abs(matCode - 1) < 0.001)
                {
                    //global light continue
                }
                else if(lightCode < 1 && abs(matCode - lightCode) < 0.002)
                {
                    //room light continue
                }
                else
                {
                    return 0;
                }
                
                float3 eyeVec = normalize(wpos-_WorldSpaceCameraPos);
                half oneMinusReflectivity = 1 - SpecularStrength(data.specularColor.rgb);

                UnityIndirect ind;
                UNITY_INITIALIZE_OUTPUT(UnityIndirect, ind);
                ind.diffuse = 0.0;
                ind.specular = 0.0;

                half4 res = UNITY_BRDF_PBS (data.diffuseColor, data.specularColor, oneMinusReflectivity, data.smoothness, data.normalWorld, -eyeVec, light, ind);

                return res;
            }

            #ifdef UNITY_HDR_ON
            half4
            #else
            fixed4
            #endif
            frag (unity_v2f_deferred i) : SV_Target
            {
                half4 c = CalculateLight(i);
                #ifdef UNITY_HDR_ON
                return c;
                #else
                return exp2(-c);
                #endif
            }

            ENDCG
        }


        // Pass 2: Final decode pass.
        // Used only with HDR off, to decode the logarithmic buffer into the main RT
        Pass {
            ZTest Always Cull Off ZWrite Off
            Stencil {
                ref [_StencilNonBackground]
                readmask [_StencilNonBackground]
                // Normally just comp would be sufficient, but there's a bug and only front face stencil state is set (case 583207)
                compback equal
                compfront equal
            }

        CGPROGRAM
        #pragma target 3.0
        #pragma vertex vert
        #pragma fragment frag
        #pragma exclude_renderers nomrt

        #include "UnityCG.cginc"

        sampler2D _LightBuffer;
        struct v2f {
            float4 vertex : SV_POSITION;
            float2 texcoord : TEXCOORD0;
        };

        v2f vert (float4 vertex : POSITION, float2 texcoord : TEXCOORD0)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(vertex);
            o.texcoord = texcoord.xy;
        #ifdef UNITY_SINGLE_PASS_STEREO
            o.texcoord = TransformStereoScreenSpaceTex(o.texcoord, 1.0f);
        #endif
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            return -log2(tex2D(_LightBuffer, i.texcoord));
        }
        ENDCG
        }
    }
    Fallback Off
}
