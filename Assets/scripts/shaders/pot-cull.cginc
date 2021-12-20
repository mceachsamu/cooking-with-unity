
inline float getAlpha (float3 wpos, float4 center, float xRad)
{
    //get the distance between this fragment and this center of the pot
    float worldCenterDistance = length(wpos.xz - center.xz);

    //t is a constant we need to calculate to get the equation
    //fot the vector that goes from the camera to the fragment
    float t = (center.y - _WorldSpaceCameraPos.y) / (wpos.y - _WorldSpaceCameraPos.y);

    //use this constant to calculate the intercept between a vector going from the center to the
    //pot to the vector going from the camera to the fragment
    float x = _WorldSpaceCameraPos.x + t*(wpos.x - _WorldSpaceCameraPos.x);
    float z = _WorldSpaceCameraPos.z + t*(wpos.z - _WorldSpaceCameraPos.z);
    float3 intercept = float3(x, center.y,z);
    float3 centerToIntercept = center.xyz - intercept;

    //get the distance for this vector
    //distance the fragmant is from the center of the circle
    float distFromCenter = length(centerToIntercept);

    //declare our alpha value so by default the texture is opaque
    float alpha = 1.0;

    //if the distance from the center to the camera vector is greater than the radius
    //of the circle, make the fragmant invisible.
    //TODO condider oval shapes (use zRadius)
    if (distFromCenter > xRad){
        alpha = 0.0;
    }

    return alpha;
}