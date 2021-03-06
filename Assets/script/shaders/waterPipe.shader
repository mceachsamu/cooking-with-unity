﻿Shader "Unlit/waterPipe"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseMap ("noise map", 2D) = "white" {}
        _Color ("Color", Color) = (0.0,0.0,0.0,0.0)

        _PipeStart("pipe start", Vector) = (0.0,0.0,0.0,0.0)
        _PipeEnd("pipe end", Vector) = (0.0,0.0,0.0,0.0)
        _PreviousEnd("pipe end previous", Vector) = (0.0,0.0,0.0,0.0)
        _Direction("direction", Vector) = (0.0,0.0,0.0,0.0)
        _DirectionPrev("Direction Previous", Vector) = (0.0,0.0,0.0,0.0)
        _PipeSize("pipe size", float) = 1.0
        _PipeLength("length", float) = 0.0
        _PipeRadius("radius", float) = 0.0
        _PipeSegmentsRound("number of segments round", float) = 0.0
        _PipeSegmentsLong("number of segments long", float) = 0.0
        _Exponent("exonential - spout curvature", float) = 0.0

        _BaseColor("base-color", Vector) = (0.99,0.0,0.3,0.0)

        [HDR]
        _AmbientColor("Ambient Color", Color) = (0.0,0.0,0.0,1.0)
        _SpecularColor("Specular Color", Color) = (0.0,0.0,0.0,1)
        _Glossiness("Glossiness", Range(0, 100)) = 14

        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimAmount("Rim Amount", Range(0, 1)) = 1.0
    }

    SubShader
    {

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "cellShading.cginc"

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 wpos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float3 normal : NORMAL;
            };

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform sampler2D _NoiseMap;
            uniform float4 _NoiseMap_ST;

            uniform float4 _Color;
            uniform float4 _PipeStart;
            uniform float4 _PipeEnd;
            uniform float4 _PreviousEnd;
            uniform float4 _Direction;
            uniform float4 _DirectionPrev;

            uniform float4 _BaseColor;
            uniform float _Glossiness;
            uniform float4 _SpecularColor;
            uniform float4 _RimColor;
            uniform float _RimAmount;
            uniform float4 _AmbientColor;

            uniform float _PipeLength;
            uniform float _PipeRadius;
            uniform float _PipeSegmentsRound;
            uniform float _PipeSegmentsLong;
            uniform float _PipeSize;
            uniform float _Exponent;

            float4 getVertexDistortion(float4 vertex, float2 uv){
                float mag = clamp(vertex.z,0,5.0);
                vertex.xy *= mag * _PipeSize;

                float4 start = mul(unity_WorldToObject,_PipeStart);
                float4 pipeEnd = mul(unity_WorldToObject,_PipeEnd);
                float4 prevEnd = mul(unity_WorldToObject,_PreviousEnd);
                float pipeLength = _PipeLength;

                float4 direction = normalize(start - pipeEnd);
                float4 directionPrev = normalize(start - prevEnd);

                float sway = (vertex.z / pipeLength);

                float4 end = (direction * (1.0-sway) + directionPrev * (sway));

                float adjusted = (length(end.z) / pipeLength);
                vertex.z *= adjusted*1.4;

                float z = length(vertex.z);
                float endZ = length(end.z);
                float a = (-end.y) / pow(endZ, _Exponent);

                float y = pow(z, _Exponent) * a;
                vertex.y += y;

                float ax = (end.xz) / (endZ*endZ);
                float x = z * z * ax;
                vertex.x -= x;

                #if !defined(SHADER_API_OPENGL)
                    float4 col = tex2Dlod(_NoiseMap, float4(float2(uv.x + _Time.y/1.5,uv.y - _Time.y),0,0));
                    float s = (col.r)*(sway);
                    vertex.x += ((s)*0.2*_PipeSize);
                    vertex.z += (s*0.1*_PipeSize);
                #endif
                return vertex;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.uv.z = v.vertex.z;

                v.vertex = getVertexDistortion(v.vertex, v.uv);
                // float PI = 3.14159265359;
                float4 worldPos = mul (unity_ObjectToWorld, v.vertex);
                // float4 start = normalize(_Direction) * v.vertex.z + _PipeStart;
                // float4 angle = PI * 2 / _PipeSegmentsRound;

                // float4 H = start - worldPos;
                // float y = cos(angle) * H;
                // float x = sin(angle) * H;
                // float4 next = float4(start.x + x, start.y + y, start.z, start.w);
                // float4 next2 = float4(start.x - x, start.y - y, start.z, start.w);
                // float zstep = 0.1;
                // float4 next3 = float4(start.x + x, start.y + y, start.z + zstep, start.w);
                // float4 next4 = float4(start.x - x, start.y - y, start.z + zstep, start.w);

                // next = normalize(getVertexDistortion(next, float2(v.uv.x + (1.0 / _PipeRadius),v.uv.y)));
                // next2 = normalize(getVertexDistortion(next2, float2(v.uv.x - (1.0 / _PipeRadius),v.uv.y)));

                // next3 = normalize(getVertexDistortion(next3, float2(v.uv.x + (1.0 / _PipeRadius),v.uv.y + zstep / (_PipeLength * v.vertex.z))));
                // next4 = normalize(getVertexDistortion(next4, float2(v.uv.x - (1.0 / _PipeRadius),v.uv.y + zstep / (_PipeLength * v.vertex.z))));

                // float normal = cross(next-next3,next-next4);

                o.normal = UnityObjectToWorldNormal(v.normal);
                o.wpos = worldPos;
                //v.vertex.x += sin(_Count/2 * v.vertex.y)/100;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv.xy, _MainTex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, float2(i.uv.x/2.0, i.uv.y - _Time.y * 2.0 )/2);

                float4 shading = GetShading(i.wpos, _WorldSpaceLightPos0.xyzw, i.normal, i.viewDir, col, _LightColor0, _RimColor, _SpecularColor, _RimAmount, _Glossiness);

                float dist = smoothstep(0,1.0,1.0/pow(length(_WorldSpaceLightPos0 - i.wpos),0.4))*1.0;
                col = _BaseColor + col.r;

                return col * dist;
            }
            ENDCG
        }
    }
}
