SAMPLER(custom_point_clamp_sampler);

void PixelUnifiedOffset_float(
    float2 uv,
    float2 offset0,
    float2 offset1,
    float2 offset2,
    float offsetIntensity,
    UnityTexture2D sourceTex,
    out float3 color
    )
{
    float2 pixelSize = sourceTex.texelSize.xy;
    color = SAMPLE_TEXTURE2D(sourceTex, custom_point_clamp_sampler, uv + offset0 * pixelSize * offsetIntensity) / 3;
    color += SAMPLE_TEXTURE2D(sourceTex, custom_point_clamp_sampler, uv + offset1 * pixelSize * offsetIntensity) / 3;
    color += SAMPLE_TEXTURE2D(sourceTex, custom_point_clamp_sampler, uv + offset2 * pixelSize * offsetIntensity) / 3;
}

void PixelMultiPassOffset_float(
    float2 uv,
    float2 offset0,
    float2 offset1,
    float2 offset2,
    float offsetIntensity,
    UnityTexture2D sourceTex,
    out float3 color
    )
{
    float2 pixelSize = sourceTex.texelSize.xy;
    color.r = SAMPLE_TEXTURE2D(sourceTex, custom_point_clamp_sampler, uv + offset0 * pixelSize * offsetIntensity).r;
    color.g = SAMPLE_TEXTURE2D(sourceTex, custom_point_clamp_sampler, uv + offset1 * pixelSize * offsetIntensity).g;
    color.b = SAMPLE_TEXTURE2D(sourceTex, custom_point_clamp_sampler, uv + offset2 * pixelSize * offsetIntensity).b;
}
