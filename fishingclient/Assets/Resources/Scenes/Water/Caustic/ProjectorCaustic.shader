// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'

Shader "Custom/ProjectorCaustic" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_FadeColor ("Fade Color", Color) = (1,1,1,1)
		_ShadowTex ("Cookie", 2D) = "" {}
	}
	
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			
			//BlendOp Add
			Blend One One
			//Blend SrcColor Zero
			//Blend DstColor One
			Offset -1, -1
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			//float4x4 _ProjectorClip;

			fixed4 _Color;
			float4 _FadeColor;
			sampler2D _ShadowTex;
			float4 _ShadowTex_ST;
			
			v2f vert (float4 vertex : POSITION, float3 normal : NORMAL)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, vertex);
				o.uvShadow = mul (unity_Projector, vertex);
				o.uvShadow.xy *= _ShadowTex_ST.xy;

				//float3 worldNormal = mul(normal, _Object2World);
				float3 worldNormal = mul(unity_ObjectToWorld, float4(normal, 0)).xyz;
				worldNormal = normalize(worldNormal); 
				o.uvShadow.z = dot(worldNormal, float3(0, 1, 0)); 
				o.uvShadow.z = clamp(o.uvShadow.z, 0, 1);

				return o;
			}
						
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 texColor = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				///_Color.a stores intensity.
				texColor.rgb *= _Color.rgb * _Color.a * 2.5;	
				texColor.rgb = lerp(_FadeColor, texColor.rgb, i.uvShadow.z);

				return texColor;
			}
			ENDCG
		}
	}
}
