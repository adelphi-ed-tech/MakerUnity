Shader "Hidden/VolumetricFog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 rayDir : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                o.rayDir = mul(unity_CameraInvProjection, float4(v.uv * 2 - 1, 1, 1)).xyz;
                
                return o;
            }

            
            sampler2D _CameraDepthTexture;
            sampler2D _MainTex;
            sampler2D _FogMap;

            float4x4 _InverseView;
            
            float4 _FogColors[32];

            int _NumSteps;

            float4 _FogBounds;
            
            int getMoodCode(float3 worldPos)
            {
                float2 uv = (worldPos.xz - _FogBounds.xy) / _FogBounds.z;
                if(uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
                {
                    return -1;
                }
                else if(worldPos.y < -0.36 || worldPos.y > 2.4)
                {
                    return -1;
                }
                else
                {
                    float modeCodeFrac = tex2Dlod(_FogMap, float4(uv, 0, 0)).r;
                    if(modeCodeFrac >= 1)
                    {
                        return -1;
                    }
                    return int(round(modeCodeFrac * 255));
                }
            }
            
            float rand(float2 co){
                return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed noise = rand(i.uv + float2(_Time.z, _Time.z * 2));
                noise = noise * 2 - 1;

                float z = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                
                float3 viewPos = i.rayDir * LinearEyeDepth(z);

                float3 worldPos = mul(_InverseView, float4(viewPos, 1)).xyz;
                
                float2 uv = (worldPos.xz - _FogBounds.xy) / _FogBounds.z;
                
                float3 rayStart = _WorldSpaceCameraPos;
                float3 rayEnd = worldPos;
                float3 rayDir = normalize(rayEnd - rayStart);

                float4 fog = 0;
                float rayLength = length(rayEnd - rayStart);
                float stepSize = rayLength / _NumSteps;
                //offset ray a bit so we aren't starting at the camera position
                rayStart += rayDir * stepSize * 0.5;
                rayStart += noise * rayDir * stepSize;
                for(int i = 0; i < _NumSteps; i++)
                {
                    float t = i / float(_NumSteps);
                    float3 samplePos = lerp(rayStart, rayEnd, t);//rayStart + rayDir * t;
                    int mood = getMoodCode(samplePos);

                    //float isFog = rc >= 0 ? 1 : 0;
                    float fogdelta = mood >= 0 ? 1 : 0;
                    //temp assume there's fog everywhere
                    float4 fogCol = _FogColors[mood];
                    fog += fogCol * 0.5 * fogdelta * stepSize;
                }
                fog = saturate(fog);
                return fog;
            }
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }


            sampler2D _MainTex;
            sampler2D _FogTexture;
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 curFog = tex2D(_MainTex, i.uv);
                fixed4 fog = tex2D(_FogTexture, i.uv);
                return lerp(curFog, fog, 0.8);
            }
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }


            sampler2D _MainTex;
            sampler2D _FogTexture;
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 fog = tex2D(_FogTexture, i.uv);
                col.rgb = lerp(col.rgb, fog.rgb, fog.a);
                return col;
            }
            ENDCG
        }
    }
}
