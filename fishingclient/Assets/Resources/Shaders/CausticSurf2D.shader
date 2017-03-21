// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/CausticSurf2D" {
	
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base", 2D) = "white" {}
	_CausticTex ("Caustic Texture", 2D) = "white" {}
	_Intensity("Intensity", float) = 1.0
	_Scale("Caustic Scale", float) = 2.0
}

SubShader {
       
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }
	LOD 200	

	Pass 
	{
		Tags { "LightMode" = "ForwardBase" }  

		//Cull Off
        //Lighting Off
        //ZWrite Off
		//ColorMask RGB 

		//BlendOp Add
		Blend SrcAlpha OneMinusSrcAlpha
		//Blend One One

		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		#pragma multi_compile_fwdbase

		#include "UnityCG.cginc"		

		float4 _Color;
		float _Intensity;
		float _Scale;
		
		sampler2D _MainTex;
		sampler2D _CausticTex;

		struct VertexData {
			float4 pos : POSITION;
			float3 normal : NORMAL;
			float4 uv : TEXCOORD0;
		};

		struct VertexToFrag
		{
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			half4 causticUV : TEXCOORD1;	
		};
	
		VertexToFrag vert (VertexData v)    
		{
			VertexToFrag o = (VertexToFrag)0;
			
			//float4 pos = v.pos * _Scale;
			//pos.w = 1;
			//o.pos = mul(UNITY_MATRIX_MVP, pos);

			float4 worldPos = mul(unity_ObjectToWorld, v.pos);

			o.pos = mul(UNITY_MATRIX_MVP, v.pos);
			o.uv = v.uv;

			o.causticUV.xy = float2(worldPos.x + fmod(_Time.x * 4, 100), worldPos.y);
			o.causticUV.xy *= _Scale;

			o.causticUV.z = 1;
			return o; 
		}
				
		fixed4 frag (VertexToFrag i) : COLOR0 
		{
			fixed4 tex = tex2D (_MainTex, i.uv);
			//fixed4 causticTex = tex2Dproj (_CausticTex, UNITY_PROJ_COORD(i.causticUV));
			fixed4 causticTex = tex2D (_CausticTex, i.causticUV);

			//return float4(i.causticUV.x / 3, 0, 0, 1);
			float4 finalColor = tex + ( tex * causticTex * _Color * _Intensity * i.causticUV.z);
			//clip(finalColor.a - 0.8);
			return finalColor;
		}	
			
		ENDCG
	}
} 

FallBack off

}


