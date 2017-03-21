Shader "Custom/WaterEdgeFar" {
	
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base", 2D) = "white" {}
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

		BlendOp Add
		//Blend SrcAlpha OneMinusSrcAlpha
		Blend One One

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
			half2 texCoord1 : TEXCOORD1;	
		};
	
		VertexToFrag vert (VertexData v)    
		{
			float fWave = CalculateWave(v.uv.x, 0.035, 0.13, 0.43);

			float4 pos = v.pos;
			pos.x += fWave;

			VertexToFrag o;
			o.pos = mul (UNITY_MATRIX_MVP, pos);
			o.uv = v.uv;
			o.texCoord1.x = clamp(0.8 + abs(fWave) * 3, 0, 1);
			o.texCoord1.y = 1;
				
			return o; 
		}
				
		fixed4 frag (VertexToFrag i) : COLOR0 
		{
			fixed4 tex = tex2D (_MainTex, i.uv);
			return tex * _Color * i.texCoord1.x;
			//return float4(i.texCoord1.x, 0, 0, 1);
		}	
			
		ENDCG
	}
} 

FallBack off
}
