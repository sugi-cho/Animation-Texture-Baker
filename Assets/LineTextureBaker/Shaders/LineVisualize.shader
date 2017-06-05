Shader "Unlit/LineVisualize"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LineTex("line tex", 2D) = "vlack"{}
		_LCount ("num lines", Float) = 5000
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
			#define ts _LineTex_TexelSize

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex, _LineTex;
			float4 _LineTex_TexelSize;
			float _LCount;
			
			v2f vert (appdata v)
			{
				float count = frac(v.uv.y + _Time.x*0.001) * _LCount;

				float x = (floor(count) + 0.5) * ts.x;
				float y = frac(count);
				
				v.vertex = tex2Dlod(_LineTex, float4(x,y,0,0));

				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return 1;
			}
			ENDCG
		}
	}
}
