﻿Shader "Unlit/bubbles"
{
    Properties
    {

        [HDR]
        _HeightMap("heightmap", 2D) = "white" {}
        _DecayMap ("decay-map", 2D) = "white" {}
        _DecayAmount ("decay-amount", float) = 0.0
        _MaxHeight("max height", float) = 0.0
        _WaterSize("water size", float) = 0.0
        _PotCenter("center", Vector) = (0.0,0.0,0.0,0.0)
        _LightPos("light-position", Vector) = (0.0,0.0,0.0,0.0)
        _Color("color", Vector) = (1.0,1.0,1.0,0.0)
        _xRad("xRad", float) = 0.0
        _zRad("zRad", float) = 0.0//to do --use to implement oval pots
        _WaterLevel("water level", float) = 0.0

        _AmbientColor("Ambient Color", Color) = (0.0,0.0,0.0,1.0)
        _SpecularColor("Specular Color", Color) = (0.1,0.1,0.1,1)
        _Glossiness("Glossiness", Range(0, 100)) = 14

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        Tags {
            "LightMode" = "ForwardAdd"
        }
        Pass
        {
            CGPROGRAM
            #pragma target 3.0

            #include "cellShading.cginc"
            #include "pot-cull.cginc"

            #pragma multi_compile_shadowcaster
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #include "AutoLight.cginc"
            #include "UnityLightingCommon.cginc"

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

            sampler2D _HeightMap;
            float4 _HeightMap_ST;

            sampler2D _DecayMap;
            float4 _DecayMap_ST;

            uniform float4 _PotCenter;
            uniform float _WaterSize;
            uniform float _MaxHeight;
            uniform float _DecayAmount;
            uniform float4 _LightPos;
            uniform float4 _Color;
            uniform float _xRad;
            uniform float _zRad;
            uniform float waterSize;
            uniform float4 _Center;
            uniform float _WaterLevel;


            float _Glossiness;
            float4 _SpecularColor;
            float4 _RimColor;
            float _RimAmount;
            float4 _AmbientColor;


            v2f vert (appdata v)
            {
                v2f o;

                //make the bubble all wobbly
                v.vertex.x = v.vertex.x + sin(v.vertex.x*10 + _Time.y*15.0)/20;
                v.vertex.z = v.vertex.z + sin(v.vertex.z*10 + _Time.y*15.0)/20;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _DecayMap);
                o.worldNormal = (unity_ObjectToWorld, v.normal);
                o.wpos = mul (unity_ObjectToWorld, v.vertex);
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
                fixed4 col = _Color;
                col.a = 0.5;

                float4 shading = GetCellShading(i.wpos, _WorldSpaceLightPos0, i.worldNormal, i.viewDir, col, _LightColor0, _RimColor, _SpecularColor, _RimAmount, _Glossiness);

                //dont render bubbles outside of pot
                float alpha = getAlpha(i.wpos, _PotCenter, _xRad);
                shading.a = alpha;

                //give bubble popping effect
                fixed4 decay = tex2D(_DecayMap, float2(i.uv.x-0.75,i.uv.y+0.1)) + _DecayAmount;
                if (decay.r < 0.5) {
                    shading.a = 0.0;
                }

                //dont render bubble underwater
                float2 waterUV = getWaterUV(i);
                float waterHeight = tex2D(_HeightMap, waterUV);
                float waterLevel = 1.0 * waterHeight + _WaterLevel - _MaxHeight;
                if (i.wpos.y < waterLevel){
                    shading.a = 0.0;
                }

                float shadow = SHADOW_ATTENUATION(i);

                return col * shading * shadow;
            }

            ENDCG
        }
    }
}
