void AlphaPassBlend_float(
    UnityTexture2D texA,
    UnityTexture2D texB,
    float2 uv,
    float3 baseColorA,
    float3 baseColorB,
    out float3 raw,
    out float3 remaped,
    out float remapedMask
){
    float3 a0 = SAMPLE_TEXTURE2D(texA, texA.samplerstate, uv).a;
    float3 a1 = SAMPLE_TEXTURE2D(texB, texB.samplerstate, uv).a;
    raw = a0 * baseColorA + a1 * baseColorB;
    remaped = raw * 0.5;
    remapedMask = (a0 + a1) * 0.5;
}
