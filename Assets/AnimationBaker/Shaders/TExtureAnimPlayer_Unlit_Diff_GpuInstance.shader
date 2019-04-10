// Wrote by Joe Rozek
// Commercial use -- yes
// Modification -- yes
// Distribution -- yes
// Private use -- yes
// YusufuCote@gmail.com

Shader "Unlit/TExtureAnimPlayer_Unlit_Diff_GpuInstance"
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
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

			#pragma multi_compile ___ ANIM_LOOP
			#pragma target 3.0

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
				UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
				UNITY_DEFINE_INSTANCED_PROP(float4, _PosTex_TexelSize)
				UNITY_DEFINE_INSTANCED_PROP(float,_Length)
				UNITY_DEFINE_INSTANCED_PROP(float, _DT)
			UNITY_INSTANCING_BUFFER_END(Props)

			
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

				o.vertex = UnityObjectToClipPos(pos);
				o.uv = TRANSFORM_TEX(v.uv, UNITY_ACCESS_INSTANCED_PROP(Props, _MainTex));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
	FallBack "Unlit/TExtureAnimPlayer_Unlit_Diff"
}
