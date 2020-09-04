// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/water"
{
    Properties
    {
        _Tex ("Texture", 2D) = "white" {}//the main texture-- used as the height map
        _RenderTex ("RenderTexture", 2D) = "white" {}
        _Texture ("tex", 2D) = "white" {}
        baseColor("base-color", Vector) = (0.99,0.0,0.3,0.0)
        secondaryColor("secondary-color", Vector) = (1.0,0.44,0.0,0.0)
        xRad("xRad", float) = 0.0
        seperation("seperation", float) = 0.0
        totalSize("totalSize", float) = 0.0
        _MaxHeight("max height", float) = 0.0
        zRad("zRad", float) = 0.0//to do --use to implement oval pots
        center("center", Vector) = (0.0,0.0,0.0,0.0)//center of pot water
        _LightPos("light-position", Vector) = (0.0,0.0,0.0,0.0)


        [HDR]
        _AmbientColor("Ambient Color", Color) = (0.0,0.0,0.0,1.0)
        _SpecularColor("Specular Color", Color) = (0.1,0.1,0.1,1)
        _Glossiness("Glossiness", Range(0, 100)) = 14

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 200
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
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
                float2 uv;
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
                float3 viewDir : TEXCOORD3;
            };

            sampler2D _Tex;
            float4 _Tex_ST;

            sampler2D _Texture;
            float4 _Texture_ST;

            sampler2D _RenderTex;
            float4 _RenderTex_ST;

            uniform float seperation;
            uniform float totalSize;
            uniform float xRad;
            uniform float zRad;
            uniform float _MaxHeight;

            uniform float4 center;
            uniform float4 baseColor;
            uniform float4 _LightPos;
            uniform float4 secondaryColor;

            float _Glossiness;
            float4 _SpecularColor;
            float4 _RimColor;
            float _RimAmount;
            float4 _AmbientColor;

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
                    float4 height2 = tex2Dlod (_Texture, float4(float2(v.uv.x,v.uv.y),0,0));
                    v.vertex.y += height.r - _MaxHeight;

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
                o.viewDir = WorldSpaceViewDir(v.vertex);
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
            float4 getShading (v2f i)
            {
                fixed4 col = baseColor;
                float3 lightDir = normalize(_LightPos - i.wpos);
                float NdotL = dot(i.worldNormal , lightDir);
                float intensity = smoothstep(0, 0.1, NdotL);
                float3 viewDir = i.viewDir;

                float3 H = normalize(_LightPos + viewDir);
                float NdotH = dot(i.worldNormal, H);
                float specIntensity = pow(NdotH * intensity, _Glossiness * _Glossiness);

                float specularIntensitySmooth = smoothstep(0.005, 0.01, specIntensity);
                float4 specular = specularIntensitySmooth * _SpecularColor;

                float4 rimDot = 1 - dot(viewDir, i.worldNormal);

                float rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimDot);
                float4 rim = rimIntensity * _RimColor;
                float overall = intensity;

                if (overall < 0.0){
                    overall = 0.5;
                }
                if (overall > 0.0){
                    overall = 1.0;
                }

                float4 finalColor = (baseColor + overall + specular + rim);
                finalColor.a = NdotL;
                return finalColor;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = baseColor;
                //check to see if we should render this fragment (if its inside the pot)
                float alpha = getAlpha(i);

                float4 shading = getShading(i);
                //render the render texure relative to screen position
                fixed4 tex = tex2D(_RenderTex, float2(i.screenPos.x, i.screenPos.y + shading.a/10 - 0.07)/i.screenPos.w);
                UNITY_APPLY_FOG(i.fogCoord, col);
                col = col*shading +  tex * abs(1.0 - tex.r);
                col.a = alpha;
                return col;
            }
            ENDCG
        }
    }
}
