inline float4 GetShading (float4 wpos, float4 lightPos, float3 wNorm, float3 viewDir, float4 baseColor, float4 lightCol, float4 _RimColor, float4 _SpecularColor, float _RimAmount, float _Glossiness)
{
    float4 col = baseColor;

    float4 _LightPos =  lightPos;

    //reduce overall shading when light source is further away
    float dist = smoothstep(0,1.0,1.0/pow(length(_LightPos - wpos),0.5)) * 2;

    //computer the over light intensity
    float3 lightDir = normalize(_LightPos - wpos);
    float NdotL = dot(wNorm , lightDir);
    float intensity = smoothstep(0, 1.0, NdotL);
    float overall = intensity;

    //calculate the specular intensity
    float3 H = normalize(_LightPos + viewDir);
    float NdotH = dot(wNorm, H);
    float specIntensity = pow(NdotH * intensity, _Glossiness * _Glossiness);

    float specularIntensitySmooth = smoothstep(0.0, 1.0, specIntensity);
    float4 specular = specularIntensitySmooth * _SpecularColor;

    //calculate the rim intentity
    float4 rimDot = 1 - dot(viewDir, wNorm);
    float rimIntensity = smoothstep(0.0, 1.0, rimDot);
    float4 rim = rimIntensity * _RimColor;

    //calculate backlighting
    float3 FragToLight = wpos - lightPos;
    if (lightPos.w == 0){
        //directional lighting
        FragToLight = - lightPos;
    }

    float backLighting = dot(normalize(viewDir), -normalize(lightDir - wNorm * 0.5));
    float smoothBack = smoothstep(0.0,1.0,backLighting);

    float4 finalColor = (baseColor + overall + specular + rim + backLighting/2.0) * dist * lightCol;
    //we arent using the alpha channel for our final shading, so pass
    //through the NdotL value so we can use it for calculating underwater distortion
    finalColor.a = NdotL;
    return finalColor;
}


inline float4 GetCellShading (float4 wpos, float4 lightPos, float3 wNorm, float3 viewDir, float4 baseColor, float4 lightCol, float4 _RimColor, float4 _SpecularColor, float _RimAmount, float _Glossiness)
{
    float4 col = baseColor;

    float4 _LightPos =  lightPos;

    //reduce overall shading when light source is further away
    float dist = smoothstep(0,1.0,1.0/pow(length(_LightPos - wpos),0.4))*1.0;

    //computer the over light intensity
    float3 lightDir = normalize(_LightPos - wpos);
    float NdotL = dot(wNorm , lightDir);
    float intensity = smoothstep(0, 0.1, NdotL);
    float overall = intensity;
    //use hard cuttoffs so we get cell effect
    if (overall < 0.0){
        overall = 0.5;
    }
    if (overall > 0.0){
        overall = 1.0;
    }

    //calculate the specular intensity
    float3 H = normalize(_LightPos + viewDir);
    float NdotH = dot(wNorm, H);
    float specIntensity = pow(NdotH * intensity, _Glossiness * _Glossiness);

    float specularIntensitySmooth = smoothstep(0.005, 0.01, specIntensity);
    float4 specular = specularIntensitySmooth * _SpecularColor;

    //calculate the rim intentity
    float4 rimDot = 1 - dot(viewDir, wNorm);
    float rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimDot);
    float4 rim = rimIntensity * _RimColor;

    //calculate backlighting
    float3 FragToLight = wpos - lightPos;
    if (lightPos.w == 0){
        //directional lighting
        FragToLight = - lightPos;
    }

    float backLighting = dot(normalize(viewDir), -normalize(lightDir - wNorm * 0.1));
    float smoothBack = smoothstep(0.0,0.5,backLighting);

    float4 finalColor = (baseColor + overall + specular + rim + backLighting/1.0) * dist * lightCol;

    //we arent using the alpha channel for our final shading, so pass
    //through the NdotL value so we can use it for calculating underwater distortion
    //this is so we get smooth values for distortion

    finalColor.a = NdotL;
    return finalColor;
}