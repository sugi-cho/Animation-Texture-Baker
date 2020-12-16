// Wrote by Joe Rozek
// Commercial use -- yes
// Modification -- yes
// Distribution -- yes
// Private use -- yes
// YusufuCote@gmail.com

Shader "Universal Render Pipeline/TextureAnimPlayer_Unlit_Diff_GpuInstance"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_PosTex("position texture", 2D) = "black"{}
		_DT("delta time", float) = 0
		_Length("animation length", Float) = 1
		[Toggle(ANIM_LOOP)] _Loop("loop", Float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			HLSLPROGRAM
			#pragma target 4.5
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON
			//#include "UnityCG.cginc"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#pragma multi_compile ___ ANIM_LOOP
			//#pragma target 3.0

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			sampler2D _MainTex, _PosTex;
			UNITY_INSTANCING_BUFFER_START(Props)
				CBUFFER_START(UnityPerMaterial)
				UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
				UNITY_DEFINE_INSTANCED_PROP(float4, _PosTex_TexelSize)
				UNITY_DEFINE_INSTANCED_PROP(float,_Length)
				UNITY_DEFINE_INSTANCED_PROP(float, _DT)
				CBUFFER_END
			UNITY_INSTANCING_BUFFER_END(Props)

			//This is a replacement for the old 'UnityObjectToClipPos()'
			float4 ObjectToClipPos(float3 pos)
			{
				return mul(UNITY_MATRIX_VP, mul(UNITY_MATRIX_M, float4 (pos, 1)));
			}
			
			v2f vert (appdata v, uint vid : SV_VertexID)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float t = (_Time.y - UNITY_ACCESS_INSTANCED_PROP(Props, _DT)) / UNITY_ACCESS_INSTANCED_PROP(Props, _Length);
#if ANIM_LOOP
				t = fmod(t, 1.0);
#else
				t = saturate(t);
#endif			
				float x = (vid + 0.5) * UNITY_ACCESS_INSTANCED_PROP(Props, _PosTex_TexelSize.x);
				float y = t;
				float4 pos = tex2Dlod(_PosTex, float4(x, y, 0, 0));

				o.vertex = ObjectToClipPos(float3(pos.x, pos.y, pos.z));
				o.uv = TRANSFORM_TEX(v.uv, UNITY_ACCESS_INSTANCED_PROP(Props, _MainTex));
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				half4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDHLSL
		}
	}
	//FallBack "Unlit/TExtureAnimPlayer_Unlit_Diff"
}
