Shader "Custom/RoomSurface"
{
    Properties
    {
        [HideInInspector]_MainTex ("Albedo (RGB)", 2D) = "white" {}
        [HideInInspector]_MoodIndex ("Mood Index", Int) = 0
        [HideInInspector]_RoomIndex ("Room Index", Int) = 0
        
    }
    SubShader
    {
        Tags { "RenderType"="Room" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;

        float4 _AmbientColors[32];
        float4 _SpecularColors[32];
        float _GlossinessValues[32];
        
        int _MoodIndex;
        int _RoomIndex;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Specular = _SpecularColors[_MoodIndex].rgb;
            o.Smoothness = _GlossinessValues[_MoodIndex];
            o.Occlusion = float(_RoomIndex) / 255.0;
            float4 ambientCol = _AmbientColors[_MoodIndex];
            o.Emission = o.Albedo.rgb * ambientCol.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
