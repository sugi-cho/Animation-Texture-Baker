Shader "Unlit/VertexToTexture"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
				float4 color : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float packNormal(float3 normal) {
				normal = normal*0.5 + 0.5;
				normal = floor(normal * 255.0);
				return normal.x + normal.y*256.0 + normal.z*256.0*256.0;
			}
			
			v2f vert (appdata v, uint vid : SV_VertexID)
			{
				float3 wPos = mul(_Object2World, v.vertex).xyz;
				float3 wNom = UnityObjectToWorldNormal(v.normal);
				float x = frac((vid+0.5) * (_ScreenParams.z-1.0));
				float y = floor(vid * (_ScreenParams.z-1.0)) * (_ScreenParams.w-1.0);
				v2f o;
				o.vertex = float4(float2(x,y) - 0.5, 0, 0.5);
				o.color = float4(wPos, packNormal(wNom));
				return o;
			}

			[maxvertexcount(3)]
			void geom(triangle v2f v[3], inout PointStream<v2f> pStream) {
				for (int i = 0; i < 3; i++)
					pStream.Append(v[i]);
			}
			
			float4 frag (v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}
	}
}
