Shader "Unlit/debug_visualizeBuffer"
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
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float3 normal : NORMAL;
				float4 vertex : SV_POSITION;
			};

			uniform StructuredBuffer<float3> _PosBuffer;
			uniform StructuredBuffer<float3> _NormBuffer;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (uint vId : SV_VertexID)
			{
				float3 position = _PosBuffer[vId];
				float3 normal = _NormBuffer[vId];
				v2f o;
				o.vertex = UnityObjectToClipPos(position);
				o.normal = UnityObjectToWorldNormal(normal);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = half4(i.normal*0.5 + 0.5,1);
				return col;
			}
			ENDCG
		}
	}
}
