void KawaseBlurPass_float(
    UnityTexture2D sourceTex,
    UnitySamplerState ss,
    float2 uv,
    float offset,
    out float4 color
)
{
    float i = offset;
    float2 texelSize = sourceTex.texelSize;

    color = float4(0, 0, 0, 1);
    color.rgb = SAMPLE_TEXTURE2D(sourceTex, ss, uv).rgb;
    color.rgb += SAMPLE_TEXTURE2D(sourceTex, ss, uv + float2(i, i) * texelSize).rgb;
    color.rgb += SAMPLE_TEXTURE2D(sourceTex, ss, uv + float2(i, -i) * texelSize).rgb;
    color.rgb += SAMPLE_TEXTURE2D(sourceTex, ss, uv + float2(-i, i) * texelSize).rgb;
    color.rgb += SAMPLE_TEXTURE2D(sourceTex, ss, uv + float2(-i, -i) * texelSize).rgb;
    color.rgb /= 5.0f;
}
