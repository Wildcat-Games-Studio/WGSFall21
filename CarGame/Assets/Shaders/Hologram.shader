Shader "Unlit/Hologram"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
		[HDR] _MainColor("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_LineCount("Line Count", Float) = 1.0
		_LineWidth("Line Width", Float) = 1.0
		_LineSpacing("Line Spacing", Float) = 1.0
		_LinePower("Line Power", Float) = 1.0
		_Speed("Speed", Float) = 1.0
    }
    SubShader
    {
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

		ZWrite Off
		Blend SrcAlpha One // Additive blending
		//Blend SrcAlpha OneMinusSrcAlpha // Normal blending

        Pass
        {
			Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
				UNITY_FOG_COORDS(1)
				float4 globalPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

			fixed4 _MainColor;
			fixed _LineCount;
			fixed _LineWidth;
			fixed _LineSpacing;
			fixed _LinePower;
			fixed _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.globalPos = mul(unity_ObjectToWorld, v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = _MainColor;
				
				float posMod = (i.globalPos.y * _LineCount + _Time.y * _Speed) % _LineSpacing;
				posMod = abs(posMod);
				col.a = posMod < _LineWidth ? pow(posMod, _LinePower) : 0.0;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
