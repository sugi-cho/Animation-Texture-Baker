#ifndef QUATERNION_INCLUDED
#define QUATERNION_INCLUDED

#define PI2 6.28318530718
// Quaternion multiplication.
// http://mathworld.wolfram.com/Quaternion.html
float4 qmul(float4 q1, float4 q2)
{
	return float4(
		q2.xyz * q1.w + q1.xyz * q2.w + cross(q1.xyz, q2.xyz),
		q1.w * q2.w - dot(q1.xyz, q2.xyz)
		);
}

// Rotate a vector with a rotation quaternion.
// http://mathworld.wolfram.com/Quaternion.html
float3 rotateWithQuaternion(float3 v, float4 r)
{
	float4 r_c = r * float4(-1.0, -1.0, -1.0, 1.0);
	return qmul(r, qmul(float4(v, 0.0), r_c)).xyz;
}

float4 getAngleAxisRotation(float3 v, float3 axis, float angle) {
	axis = normalize(axis);
	float s, c;
	sincos(angle*0.5, s, c);
	return float4(axis.x*s, axis.y*s, axis.z*s, c);
}

float3 rotateAngleAxis(float3 v, float3 axis, float angle) {
	float4 q = getAngleAxisRotation(v, axis, angle);
	return rotateWithQuaternion(v, q);
}

float4 fromToRotation(float3 from, float3 to) {
	float3
		v1 = normalize(from),
		v2 = normalize(to),
		cr = cross(v1, v2);
	float4 q = float4(cr, 1 + dot(v1, v2));
	return normalize(q);
}
#endif