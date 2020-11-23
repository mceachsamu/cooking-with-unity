Shader "Unlit/underwater"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _HeightMap("heightmap", 2D) = "white" {}
        _Caustics("caustics", 2D) = "white" {}
        _NoiseMap("noise map", 2D) = "white" {}
        
        _MaxHeight("max height", float) = 0.0
        _WaterSize("water size", float) = 0.0
        _PotCenter("center", Vector) = (0.0,0.0,0.0,0.0)
        _WaterOpaqueness("water opaqueness", float) = 0.0
        _WaterLevel("water level", float) = 0.0
        _CullAboveWater("cull above water", int) = 0
        _Count("count", float) = 0.0
        _Angle("angle", float) = 0.0

        [HDR]
        _AmbientColor("Ambient Color", Color) = (0.0,0.0,0.0,1.0)
        _SpecularColor("Specular Color", Color) = (0.0,0.0,0.0,1)
        _Glossiness("Glossiness", Range(0, 100)) = 14

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0, 1)) = 1.0
        
        adjust("Adjust", Range(-1, 1)) = 0.0
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent" "LightMode" = "ForwardAdd"}
        LOD 200
        ColorMask RGBA
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma target 3.0

            #include "cellShading.cginc"

            #pragma multi_compile_shadowcaster
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 wpos : TEXCOORD1;
                float3 worldNormal : NORMAL;
                float3 viewDir : TEXCOORD2;
                SHADOW_COORDS(3)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _HeightMap;
            float4 _HeightMap_ST;

            sampler2D _Caustics;
            float4 _Caustics_ST;

            sampler2D _NoiseMap;
            float4 _NoiseMap_ST;

            uniform float _WaterSize;
            uniform float _MaxHeight;
            uniform float _WaterOpaqueness;
            uniform float _WaterLevel;

            uniform float4 _PotCenter;

            uniform float _Count;

            uniform float _Glossiness;
            uniform float4 _SpecularColor;
            uniform float4 _RimColor;
            uniform float _RimAmount;
            uniform float4 _AmbientColor;

            uniform float adjust;
            uniform int _CullAboveWater;
            uniform float _Angle;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = v.normal;
                o.wpos = mul(unity_ObjectToWorld, v.vertex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                TRANSFER_SHADOW(o);
                return o;
            }

            float2 getWaterUV(v2f i){
                return ((i.wpos.xz - _PotCenter.xz + _WaterSize/2.0)/_WaterSize);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float2 waterUV = getWaterUV(i);
                float waterHeight = tex2D(_HeightMap, waterUV);

                fixed4 noise = tex2D(_NoiseMap, float2(i.uv.x, i.uv.y - _Count / 300.0));

                fixed4 caustic1 = tex2D(_Caustics, float2(i.uv.x+ noise.r/15.0 + waterHeight / 6.0 + _Angle / 50000.0,i.uv.y + noise.r/5.0 - _Count / 300.0 + waterHeight.r/4.0 )*3.0);
                fixed4 caustic2 = tex2D(_Caustics, float2(i.uv.x+ noise.r/15.0  + waterHeight / 6.0+ _Angle / 50000.0,i.uv.y + noise.r/4.0 + _Count / 350.0  + waterHeight.r/4.0)*2.0);

                //get shading
                float4 shading = GetShading(i.wpos, i.vertex, _WorldSpaceLightPos0.xyzw, i.worldNormal, i.viewDir, col, _RimColor, _SpecularColor, _RimAmount, _Glossiness);

                col = shading;

                float waterLevel = waterHeight + _WaterLevel - _MaxHeight;

                col = col * pow(shading,0.4);

                if (i.wpos.y < waterLevel+0.05){
                    col.a = (2.0 - pow(abs(i.wpos.y - _WaterLevel),0.5) * _WaterOpaqueness);
                }else if (_CullAboveWater == 0){
                    col.a = (2.0 - pow(abs(i.wpos.y - _WaterLevel),0.5) * _WaterOpaqueness);
                }else{
                    col.a = 0.0;
                }

                float shadow = SHADOW_ATTENUATION(i);
                col.xyz /= shadow;
                return col + (caustic1.r + caustic2.r)/1.0 * (col.a);
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

			#pragma vertex vert
			#pragma fragment frag

            #if !defined(MY_SHADOWS_INCLUDED)
            #define MY_SHADOWS_INCLUDED

            #include "UnityCG.cginc"

            struct VertexData {
                float4 position : POSITION;
                float3 normal : NORMAL;
            };

            #if defined(SHADOWS_CUBE)
                struct Interpolators {
                    float4 position : SV_POSITION;
                    float3 lightVec : TEXCOORD0;
                };

                Interpolators vert (VertexData v) {
                    Interpolators i;
                    i.position = UnityObjectToClipPos(v.position);
                    i.lightVec = mul(unity_ObjectToWorld, i.position).xyz - _LightPositionRange.xyz;
                    return i;
                }

                float4 frag (Interpolators i) : SV_TARGET {
                    float depth = length(i.lightVec) + unity_LightShadowBias.x;
                    depth *= _LightPositionRange.w;
                    if (i.position.x > 0.5){
                        return 0.0;
                    }
                    return UnityEncodeCubeShadowDepth(depth);
                }
            #else
                float4 vert (VertexData v) : SV_POSITION {
                    float4 position =
                        UnityClipSpaceShadowCasterPos(v.position.xyz, v.normal);
                    return UnityApplyLinearShadowBias(position);
                }

                half4 frag () : SV_TARGET {
                    return 0;
                }
            #endif

            #endif
			ENDCG
		}
    }
}
