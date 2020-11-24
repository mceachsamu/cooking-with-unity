// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/water"
{
    Properties
    {
        _Tex ("Texture", 2D) = "white" {}//the main texture-- used as the height map
        _RenderTex ("RenderTexture", 2D) = "white" {}
        _Texture ("tex", 2D) = "white" {}
        baseColor("base-color", Vector) = (0.99,0.0,0.3,0.0)
        xRad("xRad", float) = 0.0
        seperation("seperation", float) = 0.0
        totalSize("totalSize", float) = 0.0
        _MaxHeight("max height", float) = 0.0
        zRad("zRad", float) = 0.0//to do --use to implement oval pots
        center("center", Vector) = (0.0,0.0,0.0,0.0)//center of pot water


        [HDR]
        _AmbientColor("Ambient Color", Color) = (0.0,0.0,0.0,1.0)
        _SpecularColor("Specular Color", Color) = (0.1,0.1,0.1,1)
        _Glossiness("Glossiness", Range(0, 100)) = 14

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0, 1)) = 1.0
    }
    SubShader
    {

        Blend One One
        Tags {"RenderType"="Opaque"
            "LightMode"="ForwardAdd" }
        Lighting On
        LOD 200
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask RGB
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "cellShading.cginc"
            #include "pot-cull.cginc"
            #pragma multi_compile_fwdadd_fullshadows

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
                float4 vertex : SV_POSITION;
                float4 wpos : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float3 worldNormal : NORMAL;
                float3 viewDir : TEXCOORD3;
                float4 pos : TEXCOORD4;
                SHADOW_COORDS(5)
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
                    v.vertex.y += height.r - _MaxHeight;

                    normcalc n;
                    n.texStep = seperation / totalSize;
                    n.step = n.texStep;

                    n.uv = float2(v.uv.x, v.uv.y);
                    float3 norm = getNormal(n);

                    n.uv = float2(v.uv.x + n.step, v.uv.y);
                    float3 norm1 = getNormal(n);

                    n.uv = float2(v.uv.x - n.step, v.uv.y);
                    float3 norm2 = getNormal(n);

                    n.uv = float2(v.uv.x, v.uv.y + n.step);
                    float3 norm3 = getNormal(n);

                    n.uv = float2(v.uv.x, v.uv.y - n.step);
                    float3 norm4 = getNormal(n);

                    o.worldNormal = (norm + norm1 + norm2 + norm3 + norm4)/5.0;
                #endif

                o.pos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Tex);
                worldPos = mul (unity_ObjectToWorld, v.vertex);
                worldPos.y = worldPos.y;
                o.wpos = worldPos;
				o.screenPos = ComputeScreenPos(o.vertex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                TRANSFER_SHADOW(o);
                return o;
            }

            

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = baseColor;
                //check to see if we should render this fragment (if its inside the pot)
                float alpha = getAlpha(i.wpos, center, xRad);

                float4 shading = GetShading(i.wpos, i.vertex, _WorldSpaceLightPos0, i.worldNormal, i.viewDir, baseColor, _RimColor, _SpecularColor, _RimAmount, _Glossiness);
                //render the render texure relative to screen position
                fixed4 tex = tex2D(_RenderTex, float2(i.screenPos.x, i.screenPos.y + i.pos.y/1.5+0.3)/i.screenPos.w);

                fixed shadow = SHADOW_ATTENUATION(i);
                col = col*shading - tex * clamp(1.0 - tex.a,0.0,1.0) * 0.4;
                col.a = alpha;
                //col.xyz *= shadow;
                return  col;
            }
            ENDCG
        }

        Pass {
			Tags {
				"LightMode" = "ShadowCaster"
			}

			CGPROGRAM

			#pragma target 3.0

			#pragma multi_compile_shadowcaster

			#pragma vertex MyShadowVertexProgram
			#pragma fragment MyShadowFragmentProgram

            #if !defined(MY_SHADOWS_INCLUDED)
            #define MY_SHADOWS_INCLUDED

            #include "UnityCG.cginc"

            struct VertexData {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float3 uv : TEXCOORD0;
            };

            sampler2D _Tex;
            float4 _Tex_ST;

            float _MaxHeight;

            #if defined(SHADOWS_CUBE)
                struct Interpolators {
                    float4 position : SV_POSITION;
                    float3 lightVec : TEXCOORD0;
                };

                Interpolators MyShadowVertexProgram (VertexData v) {
                    Interpolators i;
                    i.position = UnityObjectToClipPos(v.position);
                    //i.uv = TRANSFORM_TEX(v.uv, _Tex);
                    #if !defined(SHADER_API_OPENGL)
                        float4 height = tex2Dlod (_Tex, float4(float2(v.uv.x,v.uv.y),0,0));
                        i.position.y += height.r - _MaxHeight;
                    #endif
                    i.lightVec = mul(unity_ObjectToWorld, i.position).xyz - _LightPositionRange.xyz;
                    return i;
                }

                float4 MyShadowFragmentProgram (Interpolators i) : SV_TARGET {
                    float depth = length(i.lightVec) + unity_LightShadowBias.x;
                    depth *= _LightPositionRange.w;
                    return UnityEncodeCubeShadowDepth(depth);
                }
            #else
                float4 MyShadowVertexProgram (VertexData v) : SV_POSITION {
                    float4 position =
                        UnityClipSpaceShadowCasterPos(v.position.xyz, v.normal);
                    return UnityApplyLinearShadowBias(position);
                }

                half4 MyShadowFragmentProgram () : SV_TARGET {
                    return 0;
                }
            #endif

            #endif

			ENDCG
		}

    }
}
