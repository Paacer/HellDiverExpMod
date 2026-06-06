float DirectToAngle_float(in float2 dir)
{
    return atan2(dir.y, dir.x) * 180 / 3.14159 - 90;
}
