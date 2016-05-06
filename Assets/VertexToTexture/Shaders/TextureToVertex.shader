Shader "Unlit/TextureToVertex"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_PosTex("position,normal(a)", 2D) = "black"{}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			#define ts _PosTex_TexelSize

			struct appdata
			{
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex,_PosTex;
			float4 _PosTex_TexelSize;
			
			//float val to normal(x,y,z)
			half3 unpack(float f) {
				half3 color;
				color.b = floor(f / 256.0 / 256.0);
				color.g = floor((f - color.b*256.0*256.0) / 256.0);
				color.r = floor(f - color.b * 256.0*256.0 - color.g*256.0);
				color /= 255.0;
				return color * 2 - 1.0;
			}

			v2f vert (appdata v, uint vid : SV_VertexID)
			{
				float x = frac((vid + 0.5) * ts.x);
				float y = (floor(vid * ts.x)+0.5) * ts.y;
				float4 tex = tex2Dlod(_PosTex, float4(x, 1-y, 0, 0));

				float4 pos = float4(tex.xyz,1.0);
				float3 normal = unpack(tex.a);

				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, pos);
				o.normal = UnityObjectToWorldNormal(normal);
				o.uv = v.uv;
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				half diff = dot(i.normal, float3(0,1,0))*0.5 + 0.5;
				half4 col = tex2D(_MainTex, i.uv);
				return diff * col;
			}
			ENDCG
		}
	}
}
