// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/water"
{
    Properties
    {
        _Tex ("Texture", 2D) = "white" {}//the main texture-- used as the height map
        _RenderTex ("RenderTexture", 2D) = "white" {}
        baseColor("base-color", Vector) = (0.99,0.0,0.3,0.0)
        secondaryColor("secondary-color", Vector) = (1.0,0.44,0.0,0.0)
        xRad("xRad", float) = 0.0
        seperation("seperation", float) = 0.0
        totalSize("totalSize", float) = 0.0
        zRad("zRad", float) = 0.0//to do --use to implement oval pots
        center("center", Vector) = (0.0,0.0,0.0,0.0)//center of pot water
        _LightPos("light-position", Vector) = (0.0,0.0,0.0,0.0)
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct normcalc
            {
                float2 uv : TEXCOORD0;
                float step;
                float texStep;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 wpos : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float3 worldNormal : NORMAL;
            };

            sampler2D _Tex;
            float4 _Tex_ST;

            sampler2D _RenderTex;
            float4 _RenderTex_ST;

            uniform float seperation;
            uniform float totalSize;
            uniform float xRad;
            uniform float zRad;
            uniform float4 center;

            uniform float4 baseColor;

            uniform float4 _LightPos;
            uniform float4 secondaryColor;

            float3 getNormal(normcalc v)
            {
                    float4 botLeft = tex2Dlod (_Tex, float4(float2(v.uv.x - v.texStep,v.uv.y-v.texStep),0,0));

                    float4 botRight = tex2Dlod (_Tex, float4(float2(v.uv.x + v.texStep,v.uv.y-v.texStep),0,0));

                    float4 topRight = tex2Dlod (_Tex, float4(float2(v.uv.x + v.texStep,v.uv.y + v.texStep),0,0));

                    float4 topLeft = tex2Dlod (_Tex, float4(float2(v.uv.x - v.texStep,v.uv.y + v.texStep),0,0));

                    float4 vec1 =  float4(-v.step,topLeft.r,v.step,0) - float4(-v.step,botLeft.r, -v.step,0);
                    float4 vec2 =  float4(v.step,topRight.r,v.step,0) - float4(-v.step,botLeft.r, -v.step,0);

                    float4 vec3 =  float4(-v.step,topLeft.r,v.step,0) - float4(-v.step,botLeft.r, -v.step,0);
                    float4 vec4 =  float4(v.step,botRight.r,v.step,0) - float4(-v.step,botLeft.r, -v.step,0);

                    float3 norm1 = normalize(cross(vec1,vec2));
                    float3 norm2 = normalize(cross(vec3,vec4));
                    return (norm1 + norm2)/ 2.0;
            }


            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Tex);
                //get world position from object position
                float4 worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.wpos = worldPos;
                // sample the texture
                #if !defined(SHADER_API_OPENGL)
                    float4 height = tex2Dlod (_Tex, float4(float2(v.uv.x,v.uv.y),0,0));
                    v.vertex.y = height.r;

                    normcalc n;
                    n.texStep = seperation / totalSize;
                    n.step = n.texStep;

                    n.uv = float2(v.uv.x + n.step, v.uv.y);
                    float3 norm1 = getNormal(n);

                    n.uv = float2(v.uv.x - n.step, v.uv.y);
                    float3 norm2 = getNormal(n);

                    n.uv = float2(v.uv.x, v.uv.y + n.step);
                    float3 norm3 = getNormal(n);

                    n.uv = float2(v.uv.x, v.uv.y - n.step);
                    float3 norm4 = getNormal(n);

                    o.worldNormal = (norm1 + norm2 + norm3 + norm4)/4.0;

                #endif


                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Tex);
                worldPos = mul (unity_ObjectToWorld, v.vertex);
                worldPos.y = worldPos.y;
                o.wpos = worldPos;
				o.screenPos = ComputeScreenPos(o.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float getAlpha (v2f i)
            {
                //get the distance between this fragment and this center of the pot
                float worldCenterDistance = length(i.wpos.xz - center.xz);
                //t is a constant we need to calculate to get the equation
                //fot the vector that goes from the camera to the fragment
                float t = (center.y - _WorldSpaceCameraPos.y) / (i.wpos.y - _WorldSpaceCameraPos.y);
                //use this constant to calculate the intercept between a vector going from the center to the
                //pot to the vector going from the camera to the fragment
                float x = _WorldSpaceCameraPos.x + t*(i.wpos.x - _WorldSpaceCameraPos.x);
                float z = _WorldSpaceCameraPos.z + t*(i.wpos.z - _WorldSpaceCameraPos.z);
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

            //based on blinn phong shading
            float getShading (v2f i)
            {
                float3 lightDir = normalize(_LightPos - i.wpos);
                float NdotL = dot(i.worldNormal, lightDir);
                float intensity = saturate(NdotL);
                float3 camDir = normalize(_WorldSpaceCameraPos - i.wpos);

                float3 H = normalize(lightDir + camDir);
                float NdotH = dot(i.worldNormal, H);
                float specIntensity = saturate(NdotH);

                float overall = intensity + specIntensity;
                return overall;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = baseColor;

                //check to see if we should render this fragment (if its inside the pot)
                float alpha = getAlpha(i);

                float shading = getShading(i);
                if(shading < 0.98){
                    col = secondaryColor;
                }
                else{
                    col = col* 1.0;
                }
                fixed4 tex = tex2D(_RenderTex, float2(i.screenPos.x + (shading-1.5)/120.0, i.screenPos.y + (shading-1.5)/5.0)/i.screenPos.w);
                //col = col * shading;
                col.a = alpha;
                tex.a = alpha;
                if (tex.rbg.r > 0.5){
                    tex.a = 0.0;
                }
                UNITY_APPLY_FOG(i.fogCoord, col);
                return tex/2.0 + col;
            }
            ENDCG
        }
    }
}
