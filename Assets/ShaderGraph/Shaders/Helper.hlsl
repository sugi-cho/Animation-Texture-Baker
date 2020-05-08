uint vIdx :SV_VertexID;

void SampleTex_float(
	Texture2D posTex,
	Texture2D nmlTex,
	float2 uv,
	int frame,
	out float3 outPos,
	out float3 outNml
) {
	outPos = posTex.Load(int3(uv.x, frame,0));
	outNml = nmlTex.Load(int3(uv.x, frame,0));
}