Shader "Custom/WaterEdgeLight"
 {
	
Properties 
{
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base", 2D) = "white" {}
	_Amplitude("Wave amplitude", float) = 1.0
	_WaveOffset("Wave offset", float) = 0.0
	_WaveSpeed("Wave speed", float) = 1.0
}

/*
Currently, the water surface model cannot be scaled, and 

*/
SubShader 
{
       
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }
	LOD 200	

	Pass 
	{
		Tags { "LightMode" = "ForwardBase" }

		ZWrite Off
		ColorMask RGB 
			
		Blend One One

		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		#pragma multi_compile_fwdbase

		#include "UnityCG.cginc"		
		#include "WaterShaderUtils.cginc"

		float4 _Color;
		float _Amplitude;
		float _WaveOffset;
		float _WaveSpeed;
		sampler2D _MainTex;

		struct VertexData {
			float4 pos : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float4 uv : TEXCOORD0;
			fixed4 color : COLOR;
		};

		struct VertexToFrag
		{
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			half2 uv1 : TEXCOORD1;	
		};

		VertexToFrag vert (VertexData v)    
		{
			float waveTime = fmod(_Time.x * 10, 20);

			// Calculate small wave
			float4 pos = v.pos;	
			pos.x += CalculateBigWave(v.pos.y, waveTime, _WaveOffset, _Amplitude);

			float fWave = CalculateWave(v.pos.y, 0.25, 46, 0.08);
			pos.x += fWave;

			VertexToFrag o;
			o.pos = mul (UNITY_MATRIX_MVP, pos);
			o.uv = v.uv;

			/// Make color variational
			//o.uv1.x = clamp(0.5 + abs(fWave) * 3.5, 0, 1);
			o.uv1.x = 0.2 + abs(fWave);
			o.uv1.y = 1;

			/// y is used for debug
			//o.uv1.y = fFreqFadeByDistance;

			return o; 
		}
				
		fixed4 frag (VertexToFrag i) : COLOR0 
		{
			fixed4 tex = tex2D (_MainTex, i.uv);
			return tex * _Color * i.uv1.x;
			//return float4(i.uv1.x, 0, 0, 1);
			//return float4(i.uv1.y, 0, 0, 1);
		}	
			
		ENDCG
	}
} 

FallBack off
}
