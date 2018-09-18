Shader "Unlit/Bird"
{
	Properties
	{
		_Color("color", Color) = (1,1,1,1)
		_Spec ("spec color", Color) = (0.5,0.5,0.5,0.5)
		_Emission ("emit color", Color) = (0,0,0,0)
		_Smooth ("smooth", Range(0,1)) = 0

		_PosTex("position texture", 2D) = "black"{}
		_NmlTex("normal texture", 2D) = "white"{}
		_DT ("delta time", float) = 0
		_Length ("animation length", Float) = 1
		[Toggle(ANIM_LOOP)] _Loop("loop", Float) = 0
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "UnityGBuffer.cginc"
	#include "Quaternion.cginc"

	#define ts _PosTex_TexelSize
			
	struct bird
	{
		float3 pos;
		float3 vel;
		float4 rot;
	};

	struct v2f
	{
		float3 normal : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	StructuredBuffer<bird> _Bird;
	StructuredBuffer<float3> _PosBuffer;
	StructuredBuffer<float3> _NormBuffer;
	sampler2D _PosTex, _NmlTex;
	half4 _Color, _Spec, _Emission;
	half4 _PosTex_TexelSize;
	float _Smooth,_Length, _DT;

	float rand(float2 co) {
		return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
	}

	v2f vert (uint vid : SV_VertexID, uint iid : SV_InstanceID)
	{
		float t = (_Time.y - _DT) / _Length + rand(iid*0.01);
#if ANIM_LOOP
		t = fmod(t, 1.0);
#else
		t = saturate(t);
#endif
		float x = (vid + 0.5) * ts.x;
		float y = t;

		float3 pos = tex2Dlod(_PosTex, float4(x, y, 0, 0)).xyz;
		float3 normal = tex2Dlod(_NmlTex, float4(x, y, 0, 0));

		bird b = _Bird[iid];
		float3 wPos = b.pos;
		float4 rot = b.rot;

		pos = rotateWithQuaternion(pos, rot);
		pos += wPos;
		normal = rotateWithQuaternion(normal, rot);

		v2f o;
		o.vertex = UnityWorldToClipPos(pos);
		o.normal = UnityObjectToWorldNormal(normal);
		return o;
	}
			
	void frag(
		v2f i,
		out half4 outGBuffer0 : SV_Target0,
		out half4 outGBuffer1 : SV_Target1,
		out half4 outGBuffer2 : SV_Target2,
		out half4 outEmission : SV_Target3)
	{
		half diff = dot(i.normal, float3(0,1,0))*0.5 + 0.5;
		half4 col = _Color;

		UnityStandardData data;
		data.diffuseColor = _Color;
		data.occlusion = 1;
		data.specularColor = _Spec;
		data.smoothness = _Smooth;
		data.normalWorld = i.normal;

		UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);
		outEmission = _Emission;
	}
	half4 shadow_cast(v2f i) : SV_Target
	{
		return 0;
	}

	ENDCG
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
			
		Pass
		{
			Tags{ "LightMode" = "Deferred" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile ___ ANIM_LOOP
			ENDCG
		}
		Pass
		{
			Tags{ "LightMode" = "ShadowCaster" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment shadow_cast
			#pragma multi_compile ___ ANIM_LOOP
			ENDCG
		}
	}
}
