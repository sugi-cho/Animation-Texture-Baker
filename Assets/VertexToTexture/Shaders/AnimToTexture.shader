Shader "Hidden/AnimToTexture"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Frame ("frame count", Int) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 position : TEXCOORD0;
				float4 normal : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			struct output {
				float4 out0 : SV_Target0;
				float4 position : SV_Target1;
				float4 normal : SV_Target2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Frame;

			v2f vert (appdata v, uint vid : SV_VertexID)
			{
				float3 wPos = mul(_Object2World, v.vertex).xyz;
				float3 wNom = UnityObjectToWorldNormal(v.normal);
				float x = (vid+0.5) * (_ScreenParams.z-1.0);
				float y = floor(_Frame) * (_ScreenParams.w-1.0);

				v2f o;
				o.vertex = float4(float2(x,y) - 0.5, 0, 0.5);
				o.position = float4(wPos, 1.0);
				o.normal = float4(wNom, 1.0);
				return o;
			}

			[maxvertexcount(3)]
			void geom(triangle v2f v[3], inout PointStream<v2f> pStream) {
				for (int i = 0; i < 3; i++)
					pStream.Append(v[i]);
			}
			
			output frag (v2f i)
			{
				output o;
				o.out0 = 0;
				o.position = i.position;
				o.normal = i.normal;
				return o;
			}
			ENDCG
		}
	}
}
