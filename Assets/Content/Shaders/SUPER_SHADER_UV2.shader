Shader "Custom/SUPER_SHADER_UV2"
{
    Properties
    {
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _BumpMap("Normal", 2D) = "white" {}
        _SpecGlossMap("Specular", 2D) = "white" {}
        _OcclusionMap("UV2 AO", 2D) = "white" {}
        _BumpMapShared("UV2 Normal", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _SpecGlossMap;
        sampler2D _OcclusionMap;
        sampler2D _BumpMapShared;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_SpecGlossMap;
            float2 uv2_MainTex;
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandardSpecular o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 color = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 spec = tex2D(_SpecGlossMap, IN.uv_SpecGlossMap);
            fixed4 ao = tex2D(_OcclusionMap, IN.uv2_MainTex);

            fixed3 bumpShared = UnpackNormal(tex2D(_BumpMapShared, IN.uv2_MainTex));
            fixed3 bump = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

            fixed2 swizzleBS = bumpShared.xy;
            fixed2 swizzleB = bump.xy;

            fixed2 swizzleSum = swizzleBS + swizzleB;

            fixed3 combined = fixed3(swizzleSum.x, swizzleSum.y, 1);

            // Metallic and smoothness come from slider variables
            o.Albedo = color.rgb;
            o.Normal = combined;
            o.Occlusion = ao;
            o.Smoothness = 0.5f;
            o.Specular = spec.xyz;
            
        }
        ENDCG
    }
    FallBack "Diffuse"
}
