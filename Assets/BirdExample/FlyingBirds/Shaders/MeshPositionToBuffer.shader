Shader "Unlit/MeshPositionToBuffer"
{
	SubShader
	{
		Tags { "BufferWrite" = "WriteSource" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			RWStructuredBuffer<float3> _PosBuffer : register(u1);
			RWStructuredBuffer<float3> _NormBuffer : register(u2);

			void vert(float4 vertex : POSITION,float3 normal : NORMAL, uint vId : SV_VertexID)
			{
				float3 wPos = mul(unity_ObjectToWorld, vertex).xyz;
				float3 wNorm = UnityObjectToWorldNormal(normal);
				_PosBuffer[vId] = wPos;
				_NormBuffer[vId] = wNorm;
			}

			void frag() {}
			ENDCG
		}
	}
}
