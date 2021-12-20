struct waterOutput{
    float4 vertex;
    float3 worldNorm;
};

struct normcalc{
    float2 uv;
    float step;
    float texStep;
};

inline float3 getNormal(normcalc v, sampler2D heightMap){

    float4 this = tex2Dlod (heightMap, float4(float2(v.uv.x, v.uv.y),0,0));
    float4 botLeft = tex2Dlod (heightMap, float4(float2(v.uv.x - v.texStep, v.uv.y - v.texStep),0,0));
    float4 botRight = tex2Dlod (heightMap, float4(float2(v.uv.x + v.texStep, v.uv.y - v.texStep),0,0));
    float4 topRight = tex2Dlod (heightMap, float4(float2(v.uv.x + v.texStep, v.uv.y + v.texStep),0,0));
    float4 topLeft = tex2Dlod (heightMap, float4(float2(v.uv.x - v.texStep, v.uv.y + v.texStep),0,0));

    float4 vec1 =  (float4(0, this.r,0,0) - float4(v.step, topRight.r, v.step,0));
    float4 vec2 =  (float4(0, this.r,0,0) - float4(v.step, botRight.r, -v.step,0));

    float4 vec3 =  (float4(0, this.r, 0,0) - float4(-v.step, topLeft.r, v.step,0));
    float4 vec4 =  (float4(0, this.r, 0,0) - float4(-v.step, botLeft.r, -v.step,0));

    float4 vec5 =  (float4(0, this.r, 0,0) - float4(v.step, topRight.r, v.step,0));
    float4 vec6 =  (float4(0, this.r, 0,0) - float4(v.step, topLeft.r, -v.step,0));

    float4 vec7 =  (float4(0, this.r, 0,0) - float4(-v.step, botLeft.r, -v.step,0));
    float4 vec8 =  (float4(0, this.r, 0,0) - float4(-v.step, botRight.r, v.step,0));

    float3 norm1 = normalize(cross(normalize(vec1),normalize(vec2)));
    float3 norm2 = normalize(cross(normalize(vec3),normalize(vec4)));
    float3 norm3 = normalize(cross(normalize(vec5),normalize(vec6)));
    float3 norm4 = normalize(cross(normalize(vec7),normalize(vec8)));
    return ((norm1 + norm2 + norm3 + norm4) / 4.0);
};

inline waterOutput GetWaterDistortion(sampler2D heightMap, float4 vertex, float2 uv, float seperation, float totalSize, float maxHeight){
    waterOutput o;
    o.vertex = vertex;

    float4 height = tex2Dlod (heightMap, float4(float2(uv.x,uv.y),0,0));
    o.vertex.y += height.r - maxHeight;

    normcalc n;
    n.texStep = seperation / totalSize;
    n.step =  n.texStep;

    n.uv = float2(uv.x, uv.y);
    float3 norm = getNormal(n, heightMap);

    //calculate neighbour normals
    n.uv = float2(uv.x + n.step, uv.y);
    float3 norm1 = getNormal(n, heightMap);

    n.uv = float2(uv.x - n.step, uv.y);
    float3 norm2 = getNormal(n, heightMap);

    n.uv = float2(uv.x, uv.y + n.step);
    float3 norm3 = getNormal(n, heightMap);

    n.uv = float2(uv.x, uv.y - n.step);
    float3 norm4 = getNormal(n, heightMap);

    n.uv = float2(uv.x + n.step, uv.y + n.step);
    float3 norm5 = getNormal(n, heightMap);

    n.uv = float2(uv.x - n.step, uv.y + n.step);
    float3 norm6 = getNormal(n, heightMap);

    n.uv = float2(uv.x + n.step, uv.y - n.step);
    float3 norm7 = getNormal(n, heightMap);

    n.uv = float2(uv.x - n.step, uv.y - n.step);
    float3 norm8 = getNormal(n, heightMap);

    o.worldNorm = normalize((norm + norm1 + norm2 + norm3 + norm4 + norm5 + norm6 + norm7 + norm8)/9.0);
    return o;
}