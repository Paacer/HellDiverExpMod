void CircleMask_float(
    float2 uv,
    float clearRange,
    float smooth,
    out float mask
){
    float filed = distance(uv, float2(0.5, 0.5));
    mask = 1.0 - smoothstep(clearRange - smooth, clearRange, filed);
}

void RoundedBoxMask_float(
    float2 uv,
    float2 size,
    float roundness,   // 0 = rectangle, 1 = circle
    float smooth,
    out float mask
)
{
    uv -= 0.5;
    size *= 0.5;
    float radius = roundness * min(size.x, size.y);
    float2 d = abs(uv) - (size - radius);
    float2 insideCorner = min(d, 0.0);
    float2 outsideCorner = max(d, 0.0);
    float sdf = length(outsideCorner)
        + min(max(insideCorner.x, insideCorner.y), 0.0)
        - radius;

    mask = 1.0 - smoothstep(0.0, smooth, sdf);
}
