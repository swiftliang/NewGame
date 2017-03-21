Shader "Custom/WaterEdgeSurf" {
	
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base", 2D) = "white" {}
	_Amplitude("Wave amplitude", float) = 1.0
	_WaveOffset("Wave offset", float) = 0.0
	_WaveHeight("Wave height",float) = 0.025
	_WaveSpeed("Wave speed",float) = 1.0
}

SubShader {
       
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }
	LOD 200	

	Pass 
	{
		Tags { "LightMode" = "ForwardBase" }

		ZWrite Off
		ColorMask RGB 
			
		//BlendOp Add
		//Blend SrcAlpha OneMinusSrcAlpha
		Blend DstColor Zero
		//Blend One One

		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		#pragma multi_compile_fwdbase

		#include "UnityCG.cginc"		
		#include "WaterShaderUtils.cginc"

		float4 _Color;
		float _WaveHeight;
		float _WaveSpeed;
		float _Amplitude;
		float _WaveOffset;
		sampler2D _MainTex;

		struct vertexData {
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
			//half2 normalScrollUv : TEXCOORD1;	
			//half2 screen : TEXCOORD2;
			//half2 fakeRefl : TEXCOORD3;
		};
	
		VertexToFrag vert (vertexData v)    
		{
			float waveTime = fmod(_Time.x * 10, 20);

			// Calculate small wave
			float4 pos = v.pos;

			float fScale = 0.06972299 * 1 / 0.767013;
			pos.x += CalculateBigWave(v.pos.y, waveTime, _WaveOffset, _Amplitude) * fScale;

			float fWave = CalculateWave(v.pos.y, 0.25 * fScale, 46, 0.88);
			pos.x += fWave;

			VertexToFrag o;
			o.pos = mul (UNITY_MATRIX_MVP, pos);
			o.uv = v.uv;
					
			return o; 
		}
				
		fixed4 frag (VertexToFrag i) : COLOR0 
		{
			fixed4 tex = tex2D (_MainTex, i.uv);
			
			float3 tmpColor = tex.rgb * _Color;
			float3 destColor = lerp(tmpColor, tex, tex.a);
			return float4(destColor, 1);
		}	
			
		ENDCG
	}
} 

FallBack off
}
