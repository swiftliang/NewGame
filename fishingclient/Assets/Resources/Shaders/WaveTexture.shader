﻿Shader "Unlit/WaveTexture"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_WaveSpeed ("WaveSpeed", Range(0, 100)) = 10
		_FrequencyX ("X Axis Frequency", Range(0, 100)) = 34
		_AmplitudeX ("X Axis Amplitude", Range(0, 100)) = 0.005
		_Alpha ("Alpha Value", Range(0, 1)) = 1.0
	}
	SubShader
	{
		Tags {"RenderType" = "Transparent"}
		LOD 100

		Pass
		{
            Blend SrcAlpha OneMinusSrcAlpha

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

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _FrequencyX;
			fixed _AmplitudeX;
			fixed _WaveSpeed;
			fixed _Alpha;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed2 uvs = i.uv;
				uvs.x += sin(uvs.y * _FrequencyX + _Time * _WaveSpeed) * _AmplitudeX;
				uvs.x = (uvs.x * 0.8) + 0.1;
				fixed4 col = tex2D(_MainTex, uvs);
				col.a = _Alpha;
				return col;
			}
			ENDCG
		}
	}
	FallBack "Unlit/Texture"
}