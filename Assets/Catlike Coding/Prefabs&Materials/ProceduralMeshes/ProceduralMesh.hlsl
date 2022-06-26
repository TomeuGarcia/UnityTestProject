
void Ripple_float(float3 PositionIn, float3 Origin,
				  float Period, float Amplitude, float Speed,
				  out float3 PositionOut, out float3 NormalOut, out float3 TangentOut)
{
	// y = a * sin(2*pi*p * (d - s*t))
	// - a: Amplitude
	// - p: Period
	// - s: Speed
	// - d: distance from origin
	// - t: Time

	float3 p = PositionIn - Origin;
	float d = length(p);
	float f = Amplitude * sin(2.0 * PI * Period * (d - Speed * _Time.y));

	PositionOut = PositionIn + float3(0.0, f, 0.0);


	float2 derivatives = (2.0 * PI * Amplitude * Period * cos(f) / max(d, 0.0001)) * p.xz;

	TangentOut = float3(1.0, derivatives.x, 0.0);
	NormalOut = cross(float3(0.0, derivatives.y, 1.0), TangentOut);

}
