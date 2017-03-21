// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/WaterSurface" {
	
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base", 2D) = "white" {}
	_Normal("Normal", 2D) = "bump" {}
	_Reflect("Reflection", 2D) = "black" {}
	_TexAtlasTiling("XY: Normal Dir, ZW: Normal Tiling", Vector) = (2.0,2.0, 2.0,2.0)	
	_Shine("Shine",float) = 0.3
	_WaveOffset("Wave offset", float) = 0.0
	_WaveHeight("Wave height",float) = 0.025
	_WaveSpeed("X:Normal speed, Y:Reflect speed", Vector) = (1.0, 1.0, 1.0, 1.0)
	_Amplitude("Wave amplitude", float) = 1.0
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
		Blend SrcAlpha OneMinusSrcAlpha
		//Blend Off
		//Blend One One

		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		#pragma multi_compile_fwdbase

		#include "UnityCG.cginc"		
		#include "WaterShaderUtils.cginc"

		float4 _MainTex_ST;
		float4 _Color;

		half4 _DirectionUv;
		half4 _TexAtlasTiling;

		float _Shine;
		float _WaveHeight;
		float4 _WaveSpeed;
		float _WaveOffset;
		float _Amplitude;

		sampler2D _MainTex;
		sampler2D _Normal;		
		sampler2D _Reflect;

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
			fixed4 normalUV : TEXCOORD1;	
			half4 screenUV : TEXCOORD2;
			float4 reflectUV : TEXCOORD3;
		};
	

		half2 EthansFakeReflection (half4 vtx) 
		{
			half3 worldSpace = mul(unity_ObjectToWorld, vtx).xyz; 
			//worldSpace = (-_WorldSpaceCameraPos * 0.6 + worldSpace) * 0.07;
			worldSpace *= 0.07;
			return worldSpace.xz;
		}

		VertexToFrag vert (VertexData v)    
		{
			float waveTime = fmod(_Time.x * 10, 20);

			// Calculate small wave
			float4 pos = v.pos;

			float fScale = 0.0697; 
			pos.z += CalculateBigWave(v.pos.y, waveTime, _WaveOffset, _Amplitude) * fScale * v.color.r;

			/// Samll wave
			float fWave = CalculateWave(v.pos.y, 0.25 * fScale, 46, 0.08);
			pos.z += fWave * v.color.r;

			VertexToFrag o = (VertexToFrag)0;
			o.pos = mul (UNITY_MATRIX_MVP, pos);
			o.uv.xy = TRANSFORM_TEX(v.uv,_MainTex);
			
			o.normalUV.xy = v.uv * _TexAtlasTiling.zw +  (waveTime * _WaveSpeed.x * _TexAtlasTiling.xy);
			o.reflectUV.xy = v.uv * float2(1.5, 1) + float2(15, -10) * waveTime * _WaveSpeed.y;
			/// Pass vertex alpha to fragment
			o.reflectUV.z = 0.15 + (1 - v.color.g);
			o.reflectUV.w = v.color.r;
			o.screenUV = ComputeScreenPos(o.pos);

			return o; 
		}
				
		fixed4 frag (VertexToFrag i) : COLOR0 
		{
			half3 normal = UnpackNormal(tex2D(_Normal, i.normalUV.xy ));
											
			fixed4 reflectColor = tex2D(_Reflect, i.reflectUV.xy + normal.xy * 0.05) * _Color * _Shine;
			fixed4 bgColor = tex2D (_MainTex, i.screenUV.xy + normal.xy * _WaveHeight);

			float4 finalColor = bgColor * (1-normal.r * 0.25) + reflectColor;
			finalColor.a = i.reflectUV.z;
			return finalColor;
			//return float4((1-normal.r * 1.2), 0, 0, 1);
			//return float4(i.reflectUV.w, 0, 0, 1);
			//return float4(normal, 1);
			//return reflectColor;
			//return bgColor;
			//return i.reflectUV;
		}	
			
		ENDCG
	}
} 

FallBack off
}
