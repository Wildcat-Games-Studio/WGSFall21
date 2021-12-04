Shader "Custom/Water"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _NoiseMap ("Distortion (RGB)", 2D) = "white" {}
        _NormalMap ("Normal (RGB)", 2D) = "bump" {}
		_NormalPow("Normal Power", Range(0, 1)) = 1.0
		_DistortionPow("Distortion Power", Range(0, 1)) = 1.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alpha:blend

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
			float2 uv_NoiseMap;
			float2 uv_NormalMap;
        };

        half _Glossiness;
        half _DistortionPow;
        half _NormalPow;
        fixed4 _Color;
		sampler2D _NormalMap;
		sampler2D _NoiseMap;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

            fixed4 c =  _Color;
            o.Albedo = c.rgb;

			half distortion0 = tex2D(_NoiseMap, IN.uv_NoiseMap * 0.5 + float2(_Time.x, _Time.x)).r;
			half distortion1 = tex2D(_NoiseMap, IN.uv_NoiseMap - float2(_Time.x * 2.0, _Time.x * 3.0)).r;

			half comDist = lerp(distortion0, distortion1, 0.5);

			float2 dist_uv = IN.uv_NormalMap + comDist * _DistortionPow;

			o.Normal = UnpackNormal(tex2D(_NormalMap, dist_uv) * _NormalPow);

            // Metallic and smoothness come from slider variables
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
